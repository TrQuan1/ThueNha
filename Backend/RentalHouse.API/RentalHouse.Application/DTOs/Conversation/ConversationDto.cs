using System;

namespace RentalHouse.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }

        // Thông tin nhà để hiển thị chữ: "Chat về căn: Chung cư Mini Cầu Giấy..."
        public string PropertyTitle { get; set; } = string.Empty;

        // Id của người đang chat cùng (Nếu mình là Tenant thì đây là HostId và ngược lại)
        public int PartnerId { get; set; }

        // Tên của người đang chat cùng
        public string PartnerName { get; set; } = string.Empty;

        // Tin nhắn cuối cùng để hiện ở danh sách rút gọn
        public string? LastMessage { get; set; }

        // Thời gian nhắn tin cuối cùng (để sort các đoạn chat mới nhất lên đầu)
        public DateTime LastUpdatedAt { get; set; }

        // Đếm số lượng tin nhắn chưa đọc để hiện chấm đỏ (Badge) trên UI
        public int UnreadCount { get; set; }
    }
}