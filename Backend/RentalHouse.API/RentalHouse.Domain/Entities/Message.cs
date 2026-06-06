using RentalHouse.Domain.Common;
using RentalHouse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalHouse.Domain.Entities
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }

        public Conversation Conversation { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }
}
