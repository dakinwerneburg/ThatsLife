using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    /// <summary>
    /// Creates one instance of HttpClient and all connections
    /// will utilize this instance.  Class is created to possible expand
    /// on distributing API information from this application.
    /// </summary>
    public static class ApiHub
    {
        public static HttpClient client { get; set; }

        public static void InitializeClient()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public static async Task<string> GetJson(string url)
        {
            string json = "";
            using (HttpResponseMessage response = await ApiHub.client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
            return json;
        }
    }
}





