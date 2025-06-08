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
        Normal = 0,  // Không có vấn đề xảy ra  
        OnLeave = 1,   // Nghỉ có phép  
        AbsentWithoutLeave = 2,  // Nghỉ không phép  
    }
}
