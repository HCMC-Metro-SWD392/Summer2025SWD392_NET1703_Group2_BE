using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái email gửi tự động
    public enum EmailStatus
    {
        Active = 0,      // Đã kích hoạt
        Inactive = 1 // Chưa kích hoạt
    }
}
