using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enum
{
    // Trạng thái đơn xác nhận sinh viên/học sinh
    public enum FormStatus
    {
        WaitingForApproval = 0, // Chờ duyệt
        Approved = 1,           // Đã duyệt
        Rejected = 2,           // Bị từ chối
        Cancelled = 3           // Hủy
    }
}
