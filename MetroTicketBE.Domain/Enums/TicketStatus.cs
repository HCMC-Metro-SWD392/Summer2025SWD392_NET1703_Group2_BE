using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum TicketStatus
    {
        Inactive = 0,     // Chưa sử dụng  
        Active = 1,       // Đang sử dụng  
        Used = 2,         // Đã sử dụng  
        InActiveOverStation = 3, // Chưa sử dụng vượt trạm
        ActiveOverStation = 4, // Đang sử dụng vượt trạm
    }
}
