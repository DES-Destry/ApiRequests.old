namespace ApiRequests.Amqp.Standard.Dto
{
    public class RabbitResponseDto<T>
    {
        public RabbitResponseDataDto<T> Response { get; set; }
        public bool IsDisposed { get; set; }
    }
}