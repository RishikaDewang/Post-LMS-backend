using System;
using System.Collections.Generic;

namespace LMS.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CourseCategoryMapping> CourseCategoryMappings { get; set; } = new List<CourseCategoryMapping>();
}
