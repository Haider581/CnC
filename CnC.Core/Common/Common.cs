using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Common
{
    public class Country
    {
        public int Id { get; set; }
        /// <summary>
        /// Calling Code
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        public short CallingCode { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Short Name e.g. PK
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(3)]
        public string Code { get; set; }
        [Required]
        public int CurrencyId { get; set; }

        #region Custom Properties

        [NotMapped]
        public Currency Currency { get; set; }

        #endregion
    }

    public class City
    {
        public int Id { get; set; }
        /// <summary>
        /// Area Code
        /// </summary>
        [Required]
        public short AreaCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public int CountryId { get; set; }
    }

    public class Currency
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required, StringLength(3)]
        [Index(IsUnique = true)]
        public string Code { get; set; }
        [Required, DefaultValue(false)]
        public bool IsSystemCurrency { get; set; }
    }

    public class CurrencyRate : CnCObject
    {
        [Required]
        public int CurrencyId { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Rate { get; set; }

        #region Custom Properties

        [NotMapped]
        public Currency Currency { get; set; }

        #endregion
    }

    public class Language
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// By default orientation is left to right
        /// </summary>
        [Required, DefaultValue(false)]
        public bool IsRightToLeftOrientation { get; set; }
        [Required, DefaultValue(false)]
        public bool IsSystemLanguage { get; set; }
        [MaxLength(100)]
        public string ValidationFileName { get; set; }
    }
}
