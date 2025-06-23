using System.ComponentModel.DataAnnotations;

namespace MetroTicketBE.Domain.DTO.Auth
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*(),.?""{}|<>])(?=.*\d).{8,}$",
            ErrorMessage = "Mật khẩu mới phải có ít nhất 1 ký tự đặc biệt, 1 chữ in hoa, 1 số và tối thiểu 8 ký tự")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public required string ConfirmPassword { get; set; }
    }
}
