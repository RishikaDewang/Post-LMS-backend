using System;
using System.Collections.Generic;

namespace LMS.Models;

public partial class UserActivityLogsAuditrail
{
    public int Id { get; set; }

    public int? FkUserId { get; set; }

    public string? ActivityType { get; set; }

    public byte[]? Timestamp { get; set; }

    public string? Detail { get; set; }
}
