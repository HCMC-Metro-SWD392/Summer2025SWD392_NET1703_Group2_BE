using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum TicketRoutStatus
    {
        Active = 0,       // Đang hoạt động
        Inactive = 1,     // Không hoạt động
        Deleted = 2,      // Đã xóa
    }
}
