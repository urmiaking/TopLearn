using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TopLearn.DataLayer.Entities.Permissions
{
    public class Permission
    {
        public int Id { get; set; }

        [Display(Name = "عنوان دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual List<Permission> Permissions { get; set; }

        public virtual List<RolePermission> RolePermissions { get; set; }
    }
}
