using System.Text.Json.Serialization;

namespace ApiRequests.Amqp.Standard.Dto
{
    public class RabbitResponseDto<T>
    {
        [JsonPropertyName("response")]
        public RabbitResponseDataDto<T> Response { get; set; }
        
        [JsonPropertyName("isDisposed")]
        public bool IsDisposed { get; set; }
    }
}