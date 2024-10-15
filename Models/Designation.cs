using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models;

public partial class Designation
{

    // public int ID { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? DesignationName { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<DesignationTrainingPlan> DesignationTrainingPlans { get; set; } = new List<DesignationTrainingPlan>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
