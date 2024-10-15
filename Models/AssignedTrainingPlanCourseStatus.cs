using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public partial class AssignedTrainingPlanCourseStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string? status_name { get; set; }

        // Add the foreign key property
        public int? assigncoursestatus_id { get; set; }

        public virtual ICollection<UserAssignCourse> UserAssignCourses { get; set; } = new List<UserAssignCourse>();
    }
}



