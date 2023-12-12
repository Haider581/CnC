using System.ComponentModel.DataAnnotations;
namespace CnC.Core.Localized
{
    public class LocalizedStringResource
    {
        [Key]
        public int ResourceId { get; set; }

        [MaxLength(150)]
        public string ResourceName { get; set; }

        [MaxLength(200)]
        public string ResourceValue { get; set; }

        [Required]
        public int LanguageId { get; set; }
    }
}
