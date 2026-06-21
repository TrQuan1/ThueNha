using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RentalHouse.Application.Features.Bookings.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalHouse.API.Workers;

public class BookingStatusUpdaterWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingStatusUpdaterWorker> _logger;

    public BookingStatusUpdaterWorker(IServiceProvider serviceProvider, ILogger<BookingStatusUpdaterWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🤖 Người thợ cập nhật trạng thái chuyến đi đã khởi động!");

        // Vòng lặp vô hạn này sẽ chạy liên tục chừng nào API của bạn còn đang mở
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Vì BackgroundService là dạng Singleton (sống mãi), còn MediatR là Scoped (dùng xong vứt), 
                // nên phải tạo một Scope giả lập vòng đời của 1 request để gọi nó
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    // Phát lệnh quét và cập nhật
                    await mediator.Send(new UpdateCompletedBookingsCommand(), stoppingToken);

                    _logger.LogInformation($"[OK] Đã quét xong đơn quá hạn lúc: {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi tự động cập nhật chuyến đi.");
            }

            // ⚠️ QUAN TRỌNG: 
            // - Đang test demo thì mình để 1 phút (hoặc 30 giây) quét 1 lần cho nhanh thấy kết quả.
            // - Sau khi báo cáo xong hoặc đưa lên thực tế, bạn sửa số 1 thành 60 (quét mỗi giờ) hoặc quét 1 lần/ngày.
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}