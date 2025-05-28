using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Loại vé
    public enum TicketType
    {
        OneWay = 0,       // Vé 1 lượt
        Monthly = 1,      // Vé tháng
        Student = 2       // Vé ưu đãi HSSV
    }
}
