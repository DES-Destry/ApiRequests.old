using System.Text.Json.Serialization;

namespace ApiRequests.Amqp.Standard.Dto
{
    public class RabbitResponseDataDto<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
        
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }
}