using System;
using System.Collections.Generic;

namespace LMS.Models;

public partial class UserCourseProgress
{
    public int Id { get; set; }

    public int? FkUserId { get; set; }

    public int? FkCourseId { get; set; }

    public bool? Status { get; set; }

    public DateTime? LastAccessed { get; set; }

    public virtual Course? FkCourse { get; set; }
}
