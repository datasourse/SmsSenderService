using System.Collections.Generic;

namespace WebApplication.MessageSender.Models
{
    public class SmsDto : IMessageDto
    {
        public List<string> Phones { get; set; }
        public string Text { get; set; }
    }
}