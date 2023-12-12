using CnC.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Core.Affiliates
{
    public class Address : CnCObject
    {
        [Required, MinLength(3), MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MinLength(3), MaxLength(50)]
        public string LastName { get; set; }
        [Required, MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, MaxLength(50)]
        public string Company { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        [MinLength(10), MaxLength(500)]

        [DisplayName("Address 1")]
        public string Address1 { get; set; }
        [MinLength(10), MaxLength(500)]

        [DisplayName("Address 2")]
        public string Address2 { get; set; }
        [MaxLength(10)]
        public string ZipPostalCode { get; set; }

        [Required, MinLength(11), MaxLength(20)]
        public string PhoneNumber { get; set; }
        [MaxLength(20)]
        public string FaxNumber { get; set; }

        [NotMapped]
        public Country Country { get; set; }
        [NotMapped]
        public City City { get; set; }
    }
}
