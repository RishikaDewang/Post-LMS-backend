using System;
using System.Collections.Generic;

namespace LMS.Models;

public partial class DesignationTrainingPlan
{
    public int Id { get; set; }

    public int? FkDesignationId { get; set; }

    public int? FkTrainingplanId { get; set; }

    public virtual Designation? FkDesignation { get; set; }

    public virtual TrainingPlan? FkTrainingplan { get; set; }
}
