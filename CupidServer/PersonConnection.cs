public class PersonConnection
{
    public Person Person { get; set; } = null!;
    public string ConnectionId { get; set; } = null!;
    public bool IsReadingMail { get; set; } = false;
    public List<string> BlockedUsers { get; set; } = [];

    public PersonConnection(Person person, string connectionId)
    {
        Person = person;
        ConnectionId = connectionId;
    }
}