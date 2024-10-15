using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models;

public partial class UserAssignCourse
{
    // public int ID { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? FkUserId { get; set; }

    public int? FkCourseId { get; set; }

    public DateTime? DateAssigned { get; set; }

    public DateTime? DateCompleted { get; set; }

    public int? FkTrainingplanId { get; set; }

    public bool? Status { get; set; }

    public virtual Course? FkCourse { get; set; }

    public virtual User? FkUser { get; set; }

    public virtual TrainingPlan? FkTrainingPlan { get; set; }

    // Add the foreign key property
    public int? assigncoursestatus_id { get; set; }

    public int? assigncoursepriority_id { get; set; }

    public virtual AssignedTrainingPlanCoursePriority? FKAssignedTrainingPlanCoursePriority{ get; set; }


    public virtual AssignedTrainingPlanCourseStatus? FKAssignedTrainingPlanCourseStatus { get; set; }
}
