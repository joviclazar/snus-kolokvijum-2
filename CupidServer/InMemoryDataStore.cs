using System.Collections.Concurrent;

public class InMemoryDataStore
{
    public ConcurrentDictionary<string, PersonConnection> PersonConnections { get; set; } = [];

    public void AddPersonConnection(PersonConnection personConnection)
    {
        if (PersonConnections.Any(pc => pc.Value.Person.Username == personConnection.Person.Username))
        {
            throw new Exception($"User with username {personConnection.Person.Username} is already connected.");
        }
        PersonConnections[personConnection.ConnectionId] = personConnection;
    }

    public void RemovePersonConnection(string connectionId)
    {
        PersonConnections.TryRemove(connectionId, out _);
    }

    public PersonConnection? GetPersonConnectionByUsername(string username)
    {
        return PersonConnections.FirstOrDefault(pc => pc.Value.Person.Username == username).Value;
    }

    public PersonConnection? GetPersonConnectionByConnectionId(string connectionId)
    {
        return PersonConnections.GetValueOrDefault(connectionId);
    }

    public void BlockUser(string blockerUsername, string blockedUsername)
    {
        var blockerConnection = GetPersonConnectionByUsername(blockerUsername);
        if (blockerConnection != null)
        {
            var blockedConnection = GetPersonConnectionByUsername(blockedUsername);
            if (blockedConnection != null)
            {
                blockerConnection.BlockedUsers.Add(blockedConnection.Person.Username);
            }
        }
    }

    public List<PersonConnection> GetAllNonWaitingPersonConnections()
    {
        return PersonConnections.Where(connection => !connection.Value.IsReadingMail)
                                .Select(connection => connection.Value).ToList();
    }

}