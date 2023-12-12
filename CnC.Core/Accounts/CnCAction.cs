using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Accounts
{
    public class CnCAction : CnCObject
    {
        /// <summary>
        /// User Friendly Name to Show in Menu or Tab
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Display name is required")]
        [DisplayName("Display Name")]
        [MaxLength(50)]
        public string DisplayName { get; set; }
        /// <summary>
        /// Controller Action Name
        /// </summary>
        [Index("ActionRoute", 1, IsUnique = true)]
        [DisplayName("Action Name")]
        [MaxLength(50)]
        public string ActionName { get; set; }
        /// <summary>
        /// Controller Name
        /// </summary>
        [MaxLength(50)]
        [DisplayName("Controller Name")]
        [Index("ActionRoute", 2, IsUnique = true)]
        public string ControllerName { get; set; }

        [DisplayName("Parent Action Id")]
        public int? ParentActionId { get; set; }
        /// <summary>
        /// Set true to show in the menu
        /// </summary>
        [Required]
        [DefaultValue(false)]
        [DisplayName("Show in Menu")]
        public bool ShowInMenu { get; set; }
        /// <summary>
        /// Set true to show in the tabs. It will override ShowInMenu
        /// </summary>
        [Required]
        [DefaultValue(false)]
        [DisplayName("Show in Tab")]
        public bool ShowInTab { get; set; }
        /// <summary>
        /// Whether Log in DB or not?
        /// </summary>
        [Required]
        [DefaultValue(true)]
        [DisplayName("Log")]
        public bool Log { get; set; }

        #region Custom Propertie

        [NotMapped]
        public bool IsAllowed { get; set; }

        [NotMapped]
        public bool IsDefault { get; set; }

        [NotMapped]
        public List<CnCAction> SubActions { get; set; }

        [NotMapped]
        [DisplayName("Parent Action")]
        public string ParentActionName { get; set; }

        [NotMapped]
        public CnCAction DefaultAction { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayName("Created Date From")]
        public DateTime? CreatedDateFrom { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayName("Created Date To")]
        public DateTime? CreatedDateTo { get; set; }
        #endregion
    }
}
