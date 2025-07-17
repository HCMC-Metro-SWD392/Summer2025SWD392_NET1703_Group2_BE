using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTicketBE.Domain.Enums
{
    public enum NewsStatus
    {
        Pending = 0, // Đang chờ duyệt
        Published = 1, // Đã xuất bản
        Rejected = 2, // Đã từ chối
        Updated = 3, // Đã cập nhật
    }
}
