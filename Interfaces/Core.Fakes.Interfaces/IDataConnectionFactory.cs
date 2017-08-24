namespace Core.LinqToDB.Interfaces
{
    public interface IDataConnectionFactory
    {
        IDataConnection GetDataConnection(string configurationString);
    }
}