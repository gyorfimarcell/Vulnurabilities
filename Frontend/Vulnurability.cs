using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UglyToad.PdfPig.Fonts.Encodings;

namespace Frontend
{
    public class Vulnurability
    {
        private static HttpClient _httpClient = new() {
            BaseAddress = new Uri("http://localhost:8000")
        };

        public string Name { get; set; }
        public string Synopsis { get; set; }
        public string Description { get; set; }
        public string SeeAlso { get; set; }
        public string Solution { get; set; }
        public string RiskFactor { get; set; }
        public string BaseScore { get; set; }
        public string TemporalScore { get; set; }
        public HostInfo HostInfo { get; set; }

        public string Label { get; set; }

        public async Task GetLabel(HostInfo host)
        {
            HostInfo = host;

            StringContent json = new(
                JsonSerializer.Serialize(this),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync("", json);
            response.EnsureSuccessStatusCode();

            Label = await response.Content.ReadAsStringAsync();
        }
    }
}
