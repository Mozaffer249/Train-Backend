using System.ComponentModel.DataAnnotations;
using Sudan_Train.MessagingApi.Models.Enums;

namespace Sudan_Train.MessagingApi.Models.Requests
{
    public class SendSmsRequest
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(1600)]
        public string Content { get; set; } = string.Empty;

        public string? CountryCode { get; set; }

        public SendingStrategy Strategy { get; set; } = SendingStrategy.Fallback;
    }
}
