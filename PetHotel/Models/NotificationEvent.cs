using System.Text.Json.Serialization;

namespace PetHotel.Models
{
    // for sending to notification service
    public class NotificationCloudEvent
    {
        public NotificationEvent Data { get; set; }
    }

    public class NotificationEvent
    {
        public NotificationType Type { get; set; }

        public string Message { get; set; }

        public string? EmailAddresss { get; set; }

        public string? PhoneNumber { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationType
    {
        Email,
        Sms
    }
}
