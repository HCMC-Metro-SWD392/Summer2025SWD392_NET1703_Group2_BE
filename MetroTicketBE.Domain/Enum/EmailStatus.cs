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
        Pending = 0,     // Chưa gửi
        Sent = 1,        // Đã gửi
        Failed = 2       // Gửi thất bại
    }
}
