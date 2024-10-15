using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models;

public partial class User
{

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public int? FkRoleId { get; set; }

    public int? FkDesignationId { get; set; }

    public virtual Designation? FkDesignation { get; set; }

    public virtual Role? FkRole { get; set; }

    public string? Profile { get; set; }

    [NotMapped] // This property is not stored in the database
    public string? RoleName { get; set; }

    [NotMapped] // This property is not stored in the database
    public string? DesignationName { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public virtual ICollection<UserAssignCourse> UserAssignCourses { get; set; } = new List<UserAssignCourse>();

}

