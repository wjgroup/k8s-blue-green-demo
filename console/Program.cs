using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace console
{
    class Program
    {
        private static HttpClient _httpClient;

        static void Main(string[] args)
        {
            MainAsync(args).Wait();

            Console.WriteLine("Hello World!");
        }

        static async Task MainAsync(string[] args)
        {
            while(true)
            {
                _httpClient = new HttpClient{ Timeout = new TimeSpan(0, 0, 10) };

                var tasks = new List<Task>();
                
                tasks.Add(DoIt(args[0])); await Task.Delay(100);
                tasks.Add(DoIt(args[0])); await Task.Delay(100);
                tasks.Add(DoIt(args[0])); await Task.Delay(100);
                tasks.Add(DoIt(args[0])); await Task.Delay(100);
                tasks.Add(DoIt(args[0])); await Task.Delay(100);
                tasks.Add(DoIt(args[0])); await Task.Delay(100);

                //await Task.WhenAll(tasks);

                await Task.Delay(100);
            }
        }

        static async Task DoIt(string ip)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ip}/api/api/values");
                
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"{DateTime.Now} - {response.StatusCode} - {content}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }
    }
}
