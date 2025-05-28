using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Loại yêu cầu biểu mẫu
    public enum FormRequestType
    {
        StudentProof = 0, // Xác nhận học sinh/sinh viên
        SeniorCitizenProof = 1, // Xác nhận người cao tuổi
        Other = 2 // Yêu cầu khác
    }
}
