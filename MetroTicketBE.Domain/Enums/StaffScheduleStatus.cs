using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái lịch làm việc của nhân viên
    public enum StaffScheduleStatus
    {
        Doing = 0,       // Đang làm việc
        SickLeave = 1,   // Nghỉ ốm
        Vacation = 2,    // Nghỉ phép
        Unavailable = 3,  // Không có mặt
    }
}
