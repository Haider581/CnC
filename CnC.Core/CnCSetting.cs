using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core
{
    public class CnCSetting
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(1000)]
        public string Value { get; set; }
        [DataType(DataType.MultilineText)]
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
