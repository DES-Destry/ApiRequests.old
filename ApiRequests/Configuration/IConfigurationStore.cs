namespace ApiRequests.Configuration
{
    public interface IConfigurationStore<out T> where T : IConfiguration
    {
        T GetConfiguration(ServerEnvironment environment);
    }   
}