namespace MetroTicketBE.Domain.DTO.StaffSchedule;

public class CreateStaffScheduleDTO
{ 
        public Guid StaffId { get; set; } 
        public Guid ShiftId { get; set; }
        public DateOnly WorkingDate { get; set; }
        public Guid WorkingStationId { get; set; }
}