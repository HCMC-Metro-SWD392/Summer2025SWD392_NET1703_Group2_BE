using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Seeding
{
    public static class ApplicationDBContextSeed
    {
        public static void SeedEmailTemplate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailTemplate>().HasData(
                new
                {
                    Id = Guid.NewGuid(),
                    TemplateName = "SendVerifyEmail",
                    SenderName = "Metro HCMC",
                    SenderEmail = "hoangtuzami@gmail.com",
                    Category = "Verify",
                    SubjectLine = "Xác minh địa chỉ Email của bạn",
                    PreHeaderText = "Tài khoản của bạn đang chờ xác minh",
                    PersonalizationTags = "{Login}",
                    BodyContent = @"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <title>Xác minh Email</title>
</head>
<body style='font-family: Segoe UI, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 40px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='background-color: #007bff; padding: 20px; text-align: center; color: white;'>
                            <h2 style='margin: 0;'>Metro HCMC</h2>
                            <p style='margin: 0;'>Xác minh tài khoản của bạn</p>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 30px;'>
                            <p style='font-size: 16px;'>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Metro HCMC</strong>.</p>
                            <p style='font-size: 16px;'>Để hoàn tất quá trình đăng ký, vui lòng nhấn vào nút bên dưới để xác minh địa chỉ email của bạn:</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{Login}' style='background-color: #007bff; color: white; padding: 14px 28px; text-decoration: none; font-size: 16px; border-radius: 6px; display: inline-block;'>Xác minh ngay</a>
                            </div>
                            <p style='font-size: 14px; color: #666;'>Nếu bạn không yêu cầu đăng ký tài khoản này, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi nếu có bất kỳ thắc mắc nào.</p>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f1f1f1; padding: 20px; text-align: center; font-size: 13px; color: #999;'>
                            <p style='margin: 0;'>Trân trọng,</p>
                            <p style='margin: 0;'>Đội ngũ <strong>Metro HCMC</strong></p>
                            <p style='margin: 0;'>Mọi thắc mắc xin liên hệ: <a href='https://metrohcmc.vn' style='color: #007bff; text-decoration: none;'>metrohcmc.vn</a></p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>",
                    FooterContent = "",
                    CallToAction = "<a href=\"{Login}\">Xác minh tài khoản</a>",
                    Language = "Vietnamese",
                    RecipientType = "Customer",
                    CreateBy = "System",
                    CreateTime = DateTime.Now,
                    UpdateBy = "Admin",
                    UpdateTime = DateTime.Now,
                    Status = EmailStatus.Active
                });
        }
    }
}
