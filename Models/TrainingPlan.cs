using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models;

public partial class TrainingPlan
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? TrainingPlanName { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public int? PlannedDurationDays { get; set; }

    public string? FeaturedImage { get; set; }

    public virtual ICollection<DesignationTrainingPlan> DesignationTrainingPlans { get; set; } = new List<DesignationTrainingPlan>();

    public virtual ICollection<TrainingPlanCourse> TrainingPlanCourses { get; set; } = new List<TrainingPlanCourse>();

    public virtual ICollection<UserAssignCourse> UserAssignCourses { get; set; } = new List<UserAssignCourse>();
}
