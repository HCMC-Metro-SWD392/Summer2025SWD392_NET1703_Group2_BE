using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum TicketRoutStatus
    {
        Inactive = 0,     // Không hoạt động
        Active = 1,       // Đang hoạt động
        Deleted = 2,      // Đã xóa
    }
}
