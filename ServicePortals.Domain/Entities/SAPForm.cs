using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ServicePortals.Domain.Entities
{
    [Table("sap_forms")]
    public class SAPForm
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? Data { get; set; } = "{}";
        public int? SAPTypeId { get; set; }
        public SAPType? SAPType { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }

        [NotMapped]
        public SAPFormTest? SAPFormTest
        {
            get => JsonSerializer.Deserialize<SAPFormTest>(Data ?? "");
            set => Data = value is { } ? JsonSerializer.Serialize(value) : "{}";
        }
    }

    public class SAPFormTest
    {
        public string? Name { get; set; }
    }
}
