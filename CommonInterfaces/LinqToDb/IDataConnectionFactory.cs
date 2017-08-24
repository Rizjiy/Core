namespace CommonInterfaces.LinqToDB
{
    public interface IDataConnectionFactory
    {
        IDataConnection GetDataConnection(string configurationString);
    }
}