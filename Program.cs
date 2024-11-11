using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Api;
using Api.Models;
using RestSharp;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Введите криптовалюту: ");
        string coin = Console.ReadLine();
        Console.Write("Введите период времени (например, 1d, 1y): ");
        string time = Console.ReadLine();
        Console.Write("Введите базовую валюту (например, USD): ");
        string currency = Console.ReadLine();

        var options = new RestClientOptions($"https://api.metatracker.pro/api/currency/{coin}/chart?base={currency}&period={time}");
        var client = new RestClient(options);
        var request = new RestRequest();
        request.AddHeader("accept", "application/json");

        try
        {
            var response = await client.GetAsync(request);
            if (response.IsSuccessful)
            {
                var jsonResponse = response.Content;
                Console.WriteLine("Raw JSON response: ");
                Console.WriteLine(jsonResponse); // Выводим сырые данные ответа

                try
                {
                    var currencyRecords = JsonSerializer.Deserialize<List<DataRecord>>(jsonResponse);
                    if (currencyRecords != null)
                    {
                        // Сохраняем данные в базу данных
                        using (var dbContext = new AppDbContext())
                        {
                            foreach (var record in currencyRecords)
                            {
                                record.Coin = coin;
                                record.Currency = currency;
                                record.Time = time;
                                record.Date = DateTime.UtcNow; // Сохраняем время запроса

                                dbContext.DataRecords.Add(record);
                            }
                            await dbContext.SaveChangesAsync();
                        }

                        Console.WriteLine("Данные успешно сохранены в базу данных.");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при разборе данных.");
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Ошибка при десериализации JSON: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }

        Console.ReadLine();
    }
}
