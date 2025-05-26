
namespace MetroTicketBE.Domain.Constant
{
    // Trạng thái vé
    public enum TicketStatus
    {
        Pending = 0,     // Chưa sử dụng
        Active = 1,      // Đang sử dụng
        Expired = 2,     // Hết hạn
        Cancelled = 3    // Hủy bỏ
    }

    // Trạng thái đơn xác nhận sinh viên/học sinh
    public enum FormStatus
    {
        WaitingForApproval = 0, // Chờ duyệt
        Approved = 1,           // Đã duyệt
        Rejected = 2,           // Bị từ chối
        Cancelled = 3           // Hủy
    }

    // Loại file xác nhận sinh viên
    public enum FileType
    {
        StudentProof = 0, // Giấy xác nhận sinh viên
        IdentityCard = 1, // CCCD/Hộ chiếu
        Other = 2
    }

    // Vai trò người dùng
    public enum UserRole
    {
        Admin = 0,
        Staff = 1,
        Manager = 2
    }

    // Trạng thái lịch trình tàu
    public enum ScheduleStatus
    {
        Running = 0,     // Đang chạy
        Completed = 1,   // Hoàn tất
        Cancelled = 2    // Bị hủy
    }

    // Xác định log
    public enum LogLevel
    {
        Update = 0, // Cập nhật
        Delete = 1, // Xóa
        Create = 2 // Tạo mới
    }

    // Trạng thái email gửi tự động
    public enum EmailStatus
    {
        Pending = 0,     // Chưa gửi
        Sent = 1,        // Đã gửi
        Failed = 2       // Gửi thất bại
    }

    // Loại vé
    public enum TicketType
    {
        OneWay = 0,       // Vé 1 lượt
        Monthly = 1,      // Vé tháng
        Student = 2       // Vé ưu đãi HSSV
    }

    // Trạng thái thanh toán (nếu có)
    public enum PaymentStatus
    {
        Unpaid = 0,       // Chưa thanh toán
        Paid = 1,      // Đã thanh toán
        Refunded = 2 // Đã hoàn tiền
    }

}
