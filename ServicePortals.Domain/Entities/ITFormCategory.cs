using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("it_form_categories")]
    public class ITFormCategory
    {
        public Guid? ITFormId { get; set; }
        public int? ITCategoryId { get; set; }
    }
}
