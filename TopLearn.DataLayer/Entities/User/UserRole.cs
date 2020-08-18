using System;
using System.Collections.Generic;
using System.Text;

namespace TopLearn.DataLayer.Entities.User
{
    public class UserRole
    {
        public UserRole()
        { }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        #region Relations

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }

        #endregion
    }
}
