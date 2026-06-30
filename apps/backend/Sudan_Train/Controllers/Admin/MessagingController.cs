using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Controllers.Admin
{
    [ApiController]
    [Route(Router.Admin + "/Messaging")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public class MessagingController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public MessagingController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Send a test email via MessagingApi (Direct SMTP — no queue wait).
        /// </summary>
        [HttpPost("TestEmail")]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.To))
                return BadRequest(new { success = false, message = "Recipient email is required." });

            var subject = "اختبار البريد — قطارات السودان";
            var body = """
                <div dir="rtl" style="font-family:Arial,sans-serif;line-height:1.6">
                  <p>مرحباً،</p>
                  <p>هذه رسالة اختبار من منصة قطارات السودان.</p>
                  <p>إذا وصلتك هذه الرسالة، فإعداد Hostinger SMTP يعمل بشكل صحيح.</p>
                </div>
                """;

            await _emailService.SendEmailAsync(
                request.To.Trim(),
                subject,
                body,
                EmailSendingStrategy.Direct);

            return Ok(new
            {
                success = true,
                message = "Test email sent",
                to = request.To.Trim(),
            });
        }
    }

    public class TestEmailRequest
    {
        public string To { get; set; } = string.Empty;
    }
}
