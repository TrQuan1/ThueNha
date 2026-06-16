namespace RentalHouse.Domain.Enums;

public enum PropertyStatus
{
    Pending = 1,    // Chờ duyệt đăng bài
    Active = 2,     // Đang hoạt động (Thêm trạng thái này)
    Inactive = 3,   // Tạm khóa/Ngừng cho thuê
    Maintenance = 4, // Đang bảo trì
    Rejected = 5      // Bị từ chối
}