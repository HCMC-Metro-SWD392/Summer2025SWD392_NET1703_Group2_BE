using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Loại file xác nhận sinh viên
    public enum FileType
    {
        StudentProof = 0, // Giấy xác nhận sinh viên
        IdentityCard = 1, // CCCD/Hộ chiếu
        Other = 2
    }
}
