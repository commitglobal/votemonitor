using Refit;
using Vote.Monitor.Module.Notifications.Expo.Models;

namespace Vote.Monitor.Module.Notifications.Expo;

public interface IExpoApi
{
    [Post("/send")]
    Task<ApiResponse<PushTicketResponse>> SendNotificationAsync([Body] PushTicketRequest request);

    [Post("/getReceipts")]
    Task<IApiResponse<PushReceiptResponse>> GetReceiptsAsync([Body] PushReceiptRequest request);
}
