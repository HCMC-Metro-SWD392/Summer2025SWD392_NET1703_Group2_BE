using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Constants
{
    public static class StaticUserRole
    {
        public const string Admin = "ADMIN";
        public const string Customer = "CUSTOMER";
        public const string Staff = "STAFF";
        public const string Manager = "MANAGER";

        public const string StaffManagerAdmin = "STAFF, MANAGER, ADMIN";
    }
}
