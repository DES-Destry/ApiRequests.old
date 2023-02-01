using ApiRequests.Configuration;

namespace ApiRequests.Grpc.Controller;

public interface IGrpcController<TConf> where TConf : IConfiguration
{
    TConf Configuration { get; }
    
    void SetEnvironment(ServerEnvironment environment);
}