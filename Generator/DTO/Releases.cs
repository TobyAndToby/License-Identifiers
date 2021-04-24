using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Generator.DTO
{
    public class Releases
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }
    }
}
