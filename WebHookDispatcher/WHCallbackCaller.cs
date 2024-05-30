using System.Net.Http.Json;
using System.Text.Json;
using Commons.Model.Order;

namespace WebHookDispatcher
{
    public class WHCallbackCaller
    {
        private readonly HttpClient HttpClient = new();

        public async Task InsertWebHookAsync(string url, OrderHookEvent orderHookEvent)
        {
            await HttpClient.PostAsJsonAsync(url, JsonSerializer.Serialize(orderHookEvent));
        }
    }
}
