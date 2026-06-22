using RentalHouse.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Application.Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<List<Conversation>> GetConversationsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}