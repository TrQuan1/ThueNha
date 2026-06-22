using MediatR;
using RentalHouse.Application.DTOs.Chat;
using System.Collections.Generic;

namespace RentalHouse.Application.Features.Chat.Queries;

public class GetMessagesQuery : IRequest<List<MessageDto>>
{
    public int ConversationId { get; set; }
}