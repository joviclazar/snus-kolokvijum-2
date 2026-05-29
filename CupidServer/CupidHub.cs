using Microsoft.AspNetCore.SignalR;

public class CupidHub : Hub
{

    private readonly InMemoryDataStore _dataStore;

    public CupidHub(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task InitSinglePerson(Person personDto)
    {
        string currentConnectionId = Context.ConnectionId;
        var personConnection = new PersonConnection(personDto, currentConnectionId);
        try
        {
            _dataStore.AddPersonConnection(personConnection);
            await Clients.Caller.SendAsync("InitSuccess");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("InitError", ex.Message);
        }
    }

    public void BlockUser(string username)
    {
        string currentConnectionId = Context.ConnectionId;

        var blocker = _dataStore.GetPersonConnectionByConnectionId(currentConnectionId);

        if (blocker != null)
        {
            _dataStore.BlockUser(blocker.Person.Username, username);
            Console.WriteLine($"User {blocker.Person.Username} blocked user {username}");
        }
    }

    public void AcceptRequest()
    {
        string currentConnectionId = Context.ConnectionId;
        
        var connection = _dataStore.GetPersonConnectionByConnectionId(currentConnectionId);
        
        if (connection != null)
        {
            connection.IsReadingMail = false;
            Console.WriteLine($"User {connection.Person.Username} confirmed he received a message.");
        }
    }
}