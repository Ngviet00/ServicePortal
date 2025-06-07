using System.Text.Json.Serialization;

namespace ServicePortal.Modules.User.DTO
{

    public class OrgChartUserDto
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("level")]
        public string? Level { get; set; }

        [JsonPropertyName("level_parent")]
        public string? LevelParent { get; set; }
    }

    public class OrgChartChildNode
    {
        [JsonPropertyName("level")]
        public string Level { get; set; } = string.Empty;

        [JsonPropertyName("people")]
        public List<OrgChartUserDto> People { get; set; } = new();

        [JsonPropertyName("children")]
        public List<OrgChartChildNode> Children { get; set; } = new();
    }
}
