namespace ApiRequests.Amqp.Standard.Dto
{
    public class RabbitResponseDataDto<T>
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
    }
}