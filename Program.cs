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

        //var options = new RestClientOptions($"https://api.metatracker.pro/api/currency/{coin}/chart?base={currency}&period={time}");
        var options = new RestClientOptions($"https://api.metatracker.pro/api/currency/bitcoin/chart?base=USD&period=1d");
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
                    var currencyRecords = JsonSerializer.Deserialize<DataRes>(jsonResponse);
                    if (currencyRecords != null)
                    {
                        // Сохраняем данные в базу данных
                        using (var dbContext = new AppDbContext())
                        {
                            DataRecord lastIndex = await dbContext.GetLastRecordAsync();
                            Console.WriteLine(lastIndex.Id);
                            for (int i = 0; i < currencyRecords.data.Count; i++)
                            {
                                var record = currencyRecords.data[i]; // Текущая запись
                                int id = i + 1 + lastIndex.Id; // Индекс +1 (чтобы начать с 1, а не с 0)

                                DataRecord dataRecord = new DataRecord(id, coin, currency, time, (int)record.value, (int)record.time); // Преобразуем данные в нужные типы

                                dbContext.DataRecords.Add(dataRecord);
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
