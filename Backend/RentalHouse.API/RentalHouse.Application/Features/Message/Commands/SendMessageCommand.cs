using MediatR;
using RentalHouse.Application.DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Application.Features.Message.Commands
{
    public class SendMessageCommand : IRequest<MessageDto>
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
