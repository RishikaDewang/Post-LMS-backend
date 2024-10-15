using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public partial class AssignedTrainingPlanCoursePriority
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string? priority_name { get; set; }

        // Add the foreign key property
        public int? assigncoursepriority_id { get; set; }

        public virtual ICollection<UserAssignCourse> UserAssignCourses { get; set; } = new List<UserAssignCourse>();
    }
}
