using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
