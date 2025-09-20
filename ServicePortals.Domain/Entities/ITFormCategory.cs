using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("it_form_categories")]
    [Index(nameof(ITFormId)), Index(nameof(ITCategoryId))]
    public class ITFormCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ITFormId { get; set; }
        public int ITCategoryId { get; set; }
        public ITForm? ITForm { get; set; }
        public ITCategory? ITCategory { get; set; }
    }
}
