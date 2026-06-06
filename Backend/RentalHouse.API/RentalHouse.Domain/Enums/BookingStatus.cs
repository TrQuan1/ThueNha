namespace RentalHouse.Domain.Enums;

public enum BookingStatus
{
    Pending = 1,    // Chờ duyệt
    Approved = 2,   // Đã duyệt (Thêm trạng thái này)
    Rejected = 3,   // Đã từ chối (Thêm trạng thái này)
    Cancelled = 4,  // Đã hủy
    Completed = 5   // Đã hoàn thành
}