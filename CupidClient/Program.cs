using Microsoft.AspNetCore.SignalR.Client;

string username = "";
while (string.IsNullOrWhiteSpace(username))
{
    Console.Write("Enter your Username: ");
    username = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(username)) Console.WriteLine("Error, Username can't be empty!");
}

int age = -1;
while (age <= 0)
{
    Console.Write("Enter your Age: ");
    if (!int.TryParse(Console.ReadLine(), out age) || age < 18 )
    {
        Console.WriteLine("Error! You have to be at least 18 years old!");
        age = -1;
    }
}

string city = "";
while (string.IsNullOrWhiteSpace(city))
{
    Console.Write("Enter your City: ");
    city = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(city)) Console.WriteLine("Error, City can't be empty!");
}

string phoneNumber = "";
while (string.IsNullOrWhiteSpace(phoneNumber))
{
    Console.Write("Enter your Phone Number: ");
    phoneNumber = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(phoneNumber)) Console.WriteLine("Error, Phone Number can't be empty!");
}


var connection = new HubConnectionBuilder().WithUrl("http://localhost:5205/cupidHub").Build();

bool hasNewLetter = false;
connection.On<LoveLetter>("RecieveLoveLetter", (loveLetter) => 
{
    hasNewLetter = true;
    
    Console.WriteLine();
    Console.WriteLine("You have received a new love letter!");
    Console.WriteLine($"From: {loveLetter.Username} ({loveLetter.City}, {loveLetter.Age} years old)");
    Console.WriteLine($"Message: \"{loveLetter.Message}\"");

    Console.WriteLine($"Phone: {(loveLetter.PhoneNumber == "" ? "(hidden)" : loveLetter.PhoneNumber)}");

    Console.WriteLine("---------------------------------");
    Console.WriteLine("Press ENTER to confirm receipt or type /block username:");
});
await connection.StartAsync();

await connection.InvokeAsync("InitSinglePerson", new { Username = username, City = city, Age = age, PhoneNumber = phoneNumber });


while (true)
{
    string input = Console.ReadLine()?.Trim();

    if (hasNewLetter)
    {
        if (input.StartsWith("/block"))
        {
            string[] partsOfInput = input.Split(' ');
            if (partsOfInput.Length > 1)
            {
                string userToBlock = partsOfInput[1];
                await connection.InvokeAsync("BlockUser", userToBlock);
                Console.WriteLine($"You have blocked user: {userToBlock}");
            }
        }
        else
        {
            await connection.InvokeAsync("AcceptRequest");
            Console.WriteLine("You have successfully confirmed receipt of the letter.");
        }

        hasNewLetter = false;
    }
    else
    {
        if (input != null && input.StartsWith("/block"))
        {
            string[] partsOfInput = input.Split(' ');
            if (partsOfInput.Length > 1)
            {
                await connection.InvokeAsync("BlockUser", partsOfInput[1]);
                Console.WriteLine($"You have blocked user: {partsOfInput[1]}");
            }
        }
    }
}
