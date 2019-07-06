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
                _httpClient = new HttpClient{ Timeout = new TimeSpan(0, 0, 5) };

                var tasks = new List<Task>();
                
                tasks.Add(DoIt());
                tasks.Add(DoIt());
                tasks.Add(DoIt());
                tasks.Add(DoIt());
                tasks.Add(DoIt());
                tasks.Add(DoIt());

                await Task.WhenAll(tasks);

                await Task.Delay(1000);
            }
        }

        static async Task DoIt()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://40.113.199.125/api/api/values");
                
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"{DateTime.Now} - {content}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }
    }
}
