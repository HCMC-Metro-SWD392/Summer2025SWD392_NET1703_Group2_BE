using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum MetroLineStatus
    {
        Normal = 0, // hoạt động bình thường
        Faulty = 1, // bị lỗi
        Delayed = 2, // bị chậm
    }
}
