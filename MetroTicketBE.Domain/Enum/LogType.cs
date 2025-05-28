using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Xác định log
    public enum LogType
    {
        Update = 0, // Cập nhật
        Delete = 1, // Xóa
        Create = 2 // Tạo mới
    }
}
