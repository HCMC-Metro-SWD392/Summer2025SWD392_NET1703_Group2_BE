using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái thanh toán
    public enum PaymentStatus
    {
        Unpaid = 0,       // Chưa thanh toán
        Paid = 1,      // Đã thanh toán
        Refunded = 2 // Đã hoàn tiền
    }
}
