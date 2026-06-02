public class PersonConnection
{
    public Person Person { get; set; } = null!;
    public string ConnectionId { get; set; } = null!;
    public bool IsReadingMail { get; set; } = false;
    public HashSet<string> BlockedUsers { get; set; } = [];
    private readonly object _sync = new();

    public PersonConnection(Person person, string connectionId)
    {
        Person = person;
        ConnectionId = connectionId;
    }

    public bool HasBlocked(string username)
    {
        lock (_sync)
        {
            return BlockedUsers.Contains(username);
        }
    }

    public void BlockUser(string username)
    {
        lock (_sync)
        {
            BlockedUsers.Add(username);
        }
    }

    public void SetReadingMail(bool isReadingMail)
    {
        lock (_sync)
        {
            IsReadingMail = isReadingMail;
        }
    }

    public bool IsReadingMailSafe()
    {
        lock (_sync)
        {
            return IsReadingMail;
        }
    }
}