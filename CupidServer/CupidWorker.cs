using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using Microsoft.AspNetCore.SignalR;

public class CupidWorker : BackgroundService
{
    private readonly InMemoryDataStore _dataStore;
    private readonly IHubContext<CupidHub> _hubContext;
    private const int PERIOD_SECONDS = 20;
    private string[] MESSAGES = { "Radujem se našem susretu!", "Želim da se upoznamo.", "Nisam zainteresovan/a za upoznavanje." };
    private int NOT_INTERESTED_MESSAGE_INDEX = 2;

    public CupidWorker(InMemoryDataStore dataStore, IHubContext<CupidHub> hubContext)
    {
        _dataStore = dataStore;
        _hubContext = hubContext;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(PERIOD_SECONDS));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CupidonWork();
        }
    }

    private async Task CupidonWork()
    {
        List<PersonConnection> personConnections = _dataStore.GetAllNonWaitingPersonConnections();
        if (personConnections.Count < 2)
        {
            Console.WriteLine("No available connections were found...");
            return;
        }

        foreach (PersonConnection currentConnection in personConnections)
        {
            List<PersonConnection> nonBlockedConnections = personConnections.Where(
                connection => !connection.BlockedUsers.Contains(currentConnection.Person.Username)
                && connection.ConnectionId != currentConnection.ConnectionId
            ).ToList();

            if (nonBlockedConnections.Count == 0) { continue; }

            PersonConnection? bestConnection = FindTheBestMatch(currentConnection, nonBlockedConnections);
            if (bestConnection == null)
            {
                Console.WriteLine($"Couldn't find the best match for connection {currentConnection.ConnectionId}");
                return;
            }


            int randomMessageIndex = RandomNumberGenerator.GetInt32(0, MESSAGES.Length);
            string randomMessage = MESSAGES[randomMessageIndex];

            currentConnection.IsReadingMail = true; 

            await _hubContext.Clients.Client(currentConnection.ConnectionId)
            .SendAsync("RecieveLoveLetter", new 
            {
                Username = bestConnection.Person.Username,
                City = bestConnection.Person.City,
                Age = bestConnection.Person.Age,
                PhoneNumber = randomMessageIndex == NOT_INTERESTED_MESSAGE_INDEX ? "" : bestConnection.Person.PhoneNumber
            }, randomMessage);

        }
    }

    private PersonConnection? FindTheBestMatch(PersonConnection currentConnection, List<PersonConnection> nonBlockedConnections)
    {
        int highestScore = 0;
        PersonConnection? bestMatchigConnection = null;

        foreach (PersonConnection connection in nonBlockedConnections)
        {
            int score = 0;
            if (connection.Person.City == currentConnection.Person.City)
            {
                score += 30;
            }

            if (currentConnection.Person.Age >= connection.Person.Age-2 && currentConnection.Person.Age <= connection.Person.Age+2)
            {
                score += 20;
            }

            score += RandomNumberGenerator.GetInt32(0, 101);

            if (highestScore < score)
            {
                bestMatchigConnection = connection;
                highestScore = score;
            }
        }

        return bestMatchigConnection;
    }
}