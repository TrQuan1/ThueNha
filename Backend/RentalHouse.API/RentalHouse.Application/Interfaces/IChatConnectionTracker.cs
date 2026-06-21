public interface IChatConnectionTracker
{
    void AddConnection(int userId, string connectionId);
    void RemoveConnection(int userId, string connectionId);
    IEnumerable<string> GetConnections(int userId);
}