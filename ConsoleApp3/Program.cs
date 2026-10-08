using System.Text;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace ConsoleApp3;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var client = new AmazonDynamoDBClient(RegionEndpoint.EUNorth1);

            var table = Table.LoadTable(client, "Users");
            var scanFilter = new ScanFilter();
            var search = table.Scan(scanFilter);

            using var writer = new StreamWriter("students.txt", false, Encoding.UTF8);

            await writer.WriteLineAsync("ID | Name | Email | Details");
            await writer.WriteLineAsync("----------------------------");

            var count = 0;

            do
            {
                var documentSet = await search.GetNextSetAsync();
                foreach (var doc in documentSet)
                {
                    var userId = doc.Contains("UserId") ? doc["UserId"].AsString() : "N/A";
                    var name = doc.Contains("Name") ? doc["Name"].AsString() : "N/A";
                    var email = doc.Contains("Email") ? doc["Email"].AsString() : "N/A";
                    var role = doc.Contains("Addirional") ? doc["Addirional"].AsString() : "N/A"; // typo in nosql

                    var line = $"{userId} | {name} | {email} | {role}";
                    await writer.WriteLineAsync(line);

                    count++;
                }
            } while (!search.IsDone);

            Console.WriteLine(Path.GetFullPath("students.txt"));
    }
}