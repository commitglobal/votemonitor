using Refit;
using Vote.Monitor.Module.Notifications.Expo.Models;

namespace Vote.Monitor.Module.Notifications.Expo;

public interface IExpoApi
{
    [Post("/send")]
    Task<PushReceiptResponse> SendNotificationAsync([Body] PushTicketRequest request);

    [Post("/getReceipts")]
    Task<IApiResponse<PushTicketResponse>> GetReceiptsAsync([Body] PushReceiptRequest request);
}
