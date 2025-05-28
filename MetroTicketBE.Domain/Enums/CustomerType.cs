using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum CustomerType
    {
        Normal = 0, // Khách hàng bình thường
        Student = 1, // Khách hàng học sinh, sinh viên
        OlderPerson = 2, // Khách hàng người cao tuổi
        Other = 3 // Khách hàng khác
    }
}
