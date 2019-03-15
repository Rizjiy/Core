namespace Core.Services
{
    public interface IDataConnectionFactory
    {
        IDataConnection GetDataConnection(string configurationString);
    }
}