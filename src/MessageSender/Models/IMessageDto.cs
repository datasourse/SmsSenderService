using System.Collections.Generic;

namespace WebApplication.MessageSender.Models
{
    public interface IMessageDto
    {
        public List<string> Phones { get; set; }
        public string Text { get; set; }
    }
}