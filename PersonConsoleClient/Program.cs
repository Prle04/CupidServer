using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

Console.WriteLine("=== Chaotic Cupid Console Client ===");

string serverUrl = "https://localhost:7110";

var connection = new HubConnectionBuilder()
    .WithUrl($"{serverUrl}/cupidHub")
    .WithAutomaticReconnect()
    .Build();

connection.On<LoveLetterDto>("ReceiveLetter", letter =>
{
    Console.WriteLine();
    Console.WriteLine("=== LOVE LETTER RECEIVED ===");
    Console.WriteLine($"From: {letter.SenderUsername}");
    Console.WriteLine($"City: {letter.SenderCity}");
    Console.WriteLine($"Age: {letter.SenderAge}");
    
    //Nece se ispisati nista ako je poruka da nije zainteresovana
    if (!string.IsNullOrWhiteSpace(letter.SenderPhoneNumber))
        Console.WriteLine($"Phone: {letter.SenderPhoneNumber}");

    Console.WriteLine($"Message: {letter.Message}");
    Console.WriteLine("============================");
    Console.WriteLine("Type /confirm to receive new letters.");
});

await connection.StartAsync();

Console.WriteLine("Connected to Cupid Hub.");
Console.WriteLine($"ConnectionId: {connection.ConnectionId}");

string username = ReadRequiredText("Enter username: ");
string city = ReadRequiredText("Enter city: ");
int age = ReadPositiveInt("Enter age: ");
string phoneNumber = ReadRequiredText("Enter phone number: ");

using HttpClient httpClient = new HttpClient();

var registerRequest = new
{
    Username = username,
    City = city,
    Age = age,
    PhoneNumber = phoneNumber,
    ConnectionId = connection.ConnectionId
};

var response = await httpClient.PostAsJsonAsync(
    $"{serverUrl}/api/person/init",
    registerRequest
);

string responseText = await response.Content.ReadAsStringAsync();

if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Registration failed: {responseText}");
    Console.WriteLine("Press ENTER to exit.");
    Console.ReadLine();
    return;
}

Console.WriteLine(responseText);
Console.WriteLine("You are now registered.");
Console.WriteLine();
Console.WriteLine("Available commands:");
Console.WriteLine("/confirm - confirm received letter");
Console.WriteLine("/block username - block a user");
Console.WriteLine("/exit - close client");
Console.WriteLine();
Console.WriteLine("Waiting for love letters...");

while (true)
{
    string? command = Console.ReadLine();

    if (command == null)
        continue;

    if (command.Equals("/exit", StringComparison.OrdinalIgnoreCase))
        break;

    if (command.Equals("/confirm", StringComparison.OrdinalIgnoreCase))
    {
        var confirmRequest = new
        {
            Username = username
        };

        var confirmResponse = await httpClient.PostAsJsonAsync(
            $"{serverUrl}/api/person/confirm",
            confirmRequest
        );

        string confirmText = await confirmResponse.Content.ReadAsStringAsync();

        if (confirmResponse.IsSuccessStatusCode)
            Console.WriteLine(confirmText);
        else
            Console.WriteLine($"Confirm failed: {confirmText}");

        continue;
    }

    if (command.StartsWith("/block ", StringComparison.OrdinalIgnoreCase))
    {
        string blockedUsername = command.Substring("/block ".Length).Trim();

        if (string.IsNullOrWhiteSpace(blockedUsername))
        {
            Console.WriteLine("Usage: /block username");
            continue;
        }

        var blockRequest = new
        {
            Username = username,
            BlockedUsername = blockedUsername
        };

        var blockResponse = await httpClient.PostAsJsonAsync(
            $"{serverUrl}/api/person/block",
            blockRequest
        );

        string blockText = await blockResponse.Content.ReadAsStringAsync();

        if (blockResponse.IsSuccessStatusCode)
            Console.WriteLine(blockText);
        else
            Console.WriteLine($"Block failed: {blockText}");

        continue;
    }
    Console.WriteLine("Unknown command. Available commands: /confirm, /block username, /exit");
}

static string ReadRequiredText(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
            return input.Trim();

        Console.WriteLine("Value cannot be empty.");
    }
}

static int ReadPositiveInt(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int number))
        {
            Console.WriteLine("Please enter a valid number.");
            continue;
        }

        if (number <= 0)
        {
            Console.WriteLine("Number must be positive.");
            continue;
        }

        return number;
    }
}

public class LoveLetterDto
{
    public string SenderUsername { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public int SenderAge { get; set; }
    public string SenderPhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}