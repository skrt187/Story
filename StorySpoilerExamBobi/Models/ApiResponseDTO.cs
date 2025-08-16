using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StorySpoilerExamBobi.Models
{
    class ApiResponseDTO
    {
        [JsonPropertyName("Msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("StoryId")]
        public string? storyId { get; set; }
    }
}
