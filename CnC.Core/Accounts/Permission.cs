using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Core.Accounts
{
    public class Permission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string PermissionName { get; set; }
        public bool IsAllowed { get; set; }

    }
}
