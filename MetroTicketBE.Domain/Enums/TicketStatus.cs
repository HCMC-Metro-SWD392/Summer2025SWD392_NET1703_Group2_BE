using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái vé
    public enum TicketStatus
    {
        Pending = 0,     // Chưa sử dụng
        Active = 1,      // Đang sử dụng
        Expired = 2,     // Hết hạn
        Cancelled = 3    // Hủy bỏ
    }
}
