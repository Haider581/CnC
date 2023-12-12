using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core
{
    public class CnCObject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        public CnCObject()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
