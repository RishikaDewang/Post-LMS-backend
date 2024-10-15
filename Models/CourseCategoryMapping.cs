using System;
using System.Collections.Generic;

namespace LMS.Models;

public partial class CourseCategoryMapping
{
    public int Id { get; set; }

    public int? FkCourseId { get; set; }

    public int? FkCategoryId { get; set; }

    public virtual Category? FkCategory { get; set; }

    public virtual Course? FkCourse { get; set; }
}
