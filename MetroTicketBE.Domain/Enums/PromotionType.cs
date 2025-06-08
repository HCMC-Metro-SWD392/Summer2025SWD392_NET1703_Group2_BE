using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum PromotionType
    {
        Percentage, // Giảm giá dựa trên tỷ lệ phần trăm
        FixedAmount, // Giảm giá theo số tiền cố định
    }
}
