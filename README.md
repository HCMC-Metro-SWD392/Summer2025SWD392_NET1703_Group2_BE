# MetroTicketBE – HCMC Metro Ticketing Backend (CV-style overview)

A production-ready .NET 8 Web API that powers metro ticketing for Ho Chi Minh City. The platform manages a multi-line metro network (8 lines), stations, staff scheduling, train timetables, ticket purchase and validation, QR-based check-in/checkout with over-station handling, real-time notifications, payments via PayOS, content (news/forms), and system audit logs.

— Project Scope
- Domain: Public transport ticketing for a multi-line metro network (8 lines) with interchanges.
- Users: Customers, Staff, Managers, Admins.
- Outcomes: End-to-end ticketing workflows (search routes, buy tickets, validate via QR, handle over-station payment), operations tooling (scheduling staff/trains), and operator observability (logs, alerts).

— Role & Contributions
- Designed and implemented core ticketing workflows (QR-based check-in/checkout, over-station detection and settlement).
- Built a multi-line routing engine (Dijkstra on StationGraph) across all active metro lines and interchanges.
- Implemented staff rostering at stations with shift/overlap validation and assignment tooling.
- Integrated PayOS for payments, promotions, and transaction records.
- Added real-time user notifications via SignalR for check-in/checkout and over-station events.
- Secured APIs with Identity + JWT (access/refresh), Redis-backed session, QR short-lived tokens.
- Established a repository/unit-of-work data layer on EF Core/PostgreSQL; authored AutoMapper profiles.
- Operationalized with Swagger, CORS, Kestrel, and GitHub Actions CI/CD deploy to AWS EC2.

— Tech Stack
- Language/Framework: C#, .NET 8, ASP.NET Core Web API
- AuthN/Z: ASP.NET Core Identity, JWT Bearer (Access/Refresh)
- Data: Entity Framework Core 8, PostgreSQL (Npgsql)
- Caching/Session: Redis (StackExchange.Redis)
- Realtime: SignalR (user-targeted notifications)
- Cloud/Integrations: AWS S3 (storage), AWS SES (email), PayOS (payments)
- Mapping/Docs: AutoMapper, Swagger (Swashbuckle)
- Hosting/DevOps: Kestrel, CORS, GitHub Actions CI/CD → AWS EC2

— Architecture
- Layered/Clean-ish:
  - Domain: Entities, Enums, DTOs
  - Application: Business services, SignalR hubs, integrations (Redis/AWS/PayOS), AutoMapper profiles
  - Infrastructure: EF Core DbContext, repositories, unit of work, PostgreSQL access
  - WebAPI: Controllers, DI, authentication, Swagger setup, SignalR endpoints
- Notable modules:
  - StationGraph: graph over all active MetroLines, weighted by segment distances, with zero-cost links for interchange stations; Dijkstra’s shortest path.

— Core Business Capabilities
- User & Security
  - Identity roles: Admin, Manager, Staff, Customer
  - JWT access/refresh with remember-me; refresh tokens stored per user in Redis
  - Email verification/reset via AWS SES; account lockout policies
- Customer
  - Profile, membership points, customer types (e.g., student), ownership checks on ticket QR retrieval
- Staff Scheduling
  - Create/assign daily station shifts; list schedules by station/day or by staff/date-range
  - Find unassigned staff; prevent duplicates/past dates; overlap checks supported at repo level
- Stations & Metro Lines (multi-line, 8 lines)
  - Station CRUD and activation flags; paged/sorted listings
  - MetroLine CRUD and activation; start/end stations, ordered stops with distances, line status (Normal/Faulty)
  - MetroLineStation re-order on deletion; ensure uniqueness of MetroLineNumber
- Routing & Line Health
  - SearchTicketRoad: return journey segmented by metro lines with station sequences; option to exclude Faulty lines
  - CheckMetroLineErrorInPath: detect Faulty lines along a planned route
- Train Schedules
  - Generate, update, cancel schedules; filter/sort (by metro line, station, time) with pagination
- Tickets & Pricing
  - Ticket supports either a TicketRoute (single-ride) or a SubscriptionTicket (period pass)
  - TicketRoute creation from two stations; distance via StationGraph; pricing via FareRule
  - SubscriptionTicket types; start/end stations; shared pathfinding
  - Promotions: percentage or fixed-amount discounts
- QR Check-in/Checkout (flagship)
  - Endpoints:
    - PUT api/Ticket/check-in-ticket-process
    - PUT api/Ticket/check-out-ticket-process
    - GET api/Ticket/get-qr-code/{ticketId}
  - QR strategy:
    - Single-ride: generate and persist QR on the ticket
    - Subscription/dynamic: short-lived QR (1-minute TTL) in Redis with reverse mapping qrCode → ticketId
  - Validation flow:
    - Resolve ticket by QR (direct or via Redis), validate expiry/state
    - Build StationGraph from all active metro lines
    - Check-in: station must be on allowed path → set TicketRtStatus=Active; persist TicketProcess=Checkin; notify via SignalR (“NotifyCheckinCheckout”)
    - Check-out:
      - Single-ride: set Used; TicketProcess=Checkout
      - Subscription: set Inactive; TicketProcess=Checkout
    - If outside allowed path: trigger “over-station” flow (see below)
  - History:
    - TicketProcess stores station, timestamp, and status (Checkin/Checkout); fetch all by ticket ID
- Over-Station Handling & Payments
  - Determine supplemental path from current station to permitted end; ensure TicketRoute exists
  - Recompute distance/price; apply promotions
  - Create PayOS payment link; record PaymentTransaction
  - Optionally attach IntegratedTicket serial in TicketProcess (checkout)
- Content & Requests
  - News linked to Staff; list with StaffName
  - FormRequest + attachments; sender/reviewer relationships
- Audit Logging
  - Create/Update/Delete actions, associated user, paginated queries
  - Used on station/line status changes and other sensitive operations
- Realtime Notifications
  - SignalR user-channel messages for check-in/checkout and over-station events
  - Channels: “NotifyCheckinCheckout”, “NotifyOverStation”

— Data Design (Selected Entities)
- Transport: Station, MetroLine, MetroLineStation, TrainSchedule, Train
- Ticketing: Ticket, TicketRoute, SubscriptionTicket, SubscriptionTicketType, TicketProcess
- Pricing/Payment: FareRule, Promotion, PaymentTransaction, PaymentMethod, PayOSMethod
- People: ApplicationUser (Identity), Customer, Staff, Membership, StaffShift, StaffSchedule
- Content/Ops: News, FormRequest, FormAttachment, Log

— Algorithms & Implementation Details
- Multi-line route finding:
  - StationGraph built from all active lines
  - Weighted edges between consecutive stations; zero-weight edges for interchange transfers
  - Dijkstra for shortest path across lines and transfers
- QR life cycle:
  - Subscription tickets use short-lived QR with Redis key pairs:
    - ticketId:{id}-QRCode and qrCode:{qr}-ticketId (1-minute TTL)
  - Single-ride tickets persist QR directly on the ticket
- Path-constrained validation:
  - Check-in/out allowed only if the station belongs to the computed valid path between start and end
  - Outside-path attempts trigger over-station pricing/settlement

— Security & Reliability
- JWT access/refresh; per-user refresh token store in Redis; forced session invalidation supported
- Role-based authorization on sensitive endpoints (e.g., staff-only ticket validation)
- TicketProcess and PaymentTransaction relationships configured with restrictive deletes
- Defensive checks: ticket ownership on QR retrieval, expiry, status transitions

— DevOps & Delivery
- Swagger UI for API discovery
- CORS configured for allowed front-end origins
- GitHub Actions CI/CD:
  - Build, publish, zip artifact
  - SSH to EC2, rolling replacement of app files, Systemd service restart
- Hosting via Kestrel; environment-driven configuration (DB/JWT/Redis/PayOS/AWS)

— Representative API Surface
- Ticket:
  - GET api/Ticket/get-ticket/{serial}
  - PUT api/Ticket/change-ticket-route-status/{ticketId}
  - PUT api/Ticket/check-in-ticket-process
  - PUT api/Ticket/check-out-ticket-process
  - GET api/Ticket/get-qr-code/{ticketId}
  - GET api/Ticket/check-exist-ticket-range?startStaionId=&endStationId=
- TicketProcess:
  - GET api/TicketProcess/GetAllTicketProcessByTicketId/{ticketId}
- StaffSchedule:
  - GET api/StaffSchedule/schedules
  - POST api/StaffSchedule/create
  - GET api/StaffSchedule/schedules-by-station
  - GET api/StaffSchedule/schedules-by-staff
  - GET api/StaffSchedule/get-unscheduled-staff
  - PUT api/StaffSchedule/assign-staff
- Stations/Lines/Schedules:
  - CRUD/activate-deactivate; search routes; detect faulty lines; create/update/cancel train schedules

— Notes for Reviewers
- The routing, QR, and over-station flows are the core differentiators:
  - Graph-based multi-line routing scales across 8 lines and interchange stations
  - Path-constrained validation reduces fraud and improves UX
  - Short-lived QR codes for subscriptions improve security while supporting real-time validation
- The solution is modular, testable, and production-oriented with clear separation of concerns and infra adapters.

If you need a tailored one-pager or slide for a hiring panel, I can condense this into a visual architecture + key flows diagram with talking points.
