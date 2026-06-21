using System.Collections.Concurrent;

public class ChatConnectionTracker : IChatConnectionTracker
{
    // Dictionary map UserId -> List các ConnectionIds (Vì 1 user có thể login trên đt và máy tính cùng lúc)
    private readonly ConcurrentDictionary<int, List<string>> _onlineUsers = new();

    public void AddConnection(int userId, string connectionId)
    {
        _onlineUsers.AddOrUpdate(
            userId,
            new List<string> { connectionId },
            (key, existingConnections) =>
            {
                lock (existingConnections)
                {
                    if (!existingConnections.Contains(connectionId))
                        existingConnections.Add(connectionId);
                }
                return existingConnections;
            });
    }

    public void RemoveConnection(int userId, string connectionId)
    {
        if (_onlineUsers.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _onlineUsers.TryRemove(userId, out _);
                }
            }
        }
    }

    public IEnumerable<string> GetConnections(int userId)
    {
        if (_onlineUsers.TryGetValue(userId, out var connections))
        {
            return connections;
        }
        return Enumerable.Empty<string>();
    }
}