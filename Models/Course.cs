using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models;

public partial class Course
{

    // public int ID { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }


    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? VideoLink { get; set; }

    public string? CourseDuration { get; set; }

    public string? Keywords { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? PdfUpload { get; set; }

    public string? FeaturedImage { get; set; }

    

    public virtual ICollection<CourseCategoryMapping> CourseCategoryMappings { get; set; } = new List<CourseCategoryMapping>();

    public virtual ICollection<TrainingPlanCourse> TrainingPlanCourses { get; set; } = new List<TrainingPlanCourse>();

    public virtual ICollection<UserAssignCourse> UserAssignCourses { get; set; } = new List<UserAssignCourse>();

    public virtual ICollection<UserCourseProgress> UserCourseProgresses { get; set; } = new List<UserCourseProgress>();
}
