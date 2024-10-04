using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frontend
{
    public class HostInfo
    {
        public string Dns { get; set; }
        public string Ip { get; set; }
        public string Mac { get; set; }
        public string Os { get; set; }

        public string ToJson() { return JsonSerializer.Serialize(this); }
    }
}
