using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.Infrastructure.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Conversation>> GetConversationsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            // Dùng Include để gom toàn bộ dữ liệu Nhà, Host, Tenant và Tin nhắn trong đúng 1 lần gọi DB (Tối ưu 100% Performance)
            return await _context.Conversations
                .Include(c => c.Property)
                .Include(c => c.Host)
                .Include(c => c.Tenant)
                .Include(c => c.Messages)
                .Where(c => c.TenantId == userId || c.HostId == userId)
                .OrderByDescending(c => c.LastUpdatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}