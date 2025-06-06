using System;
using System.Collections.Generic;

namespace SchoolMedicalWpf.Dal.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public DateTime SendDate { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public string? Content { get; set; }

    public bool IsConfirmed { get; set; }

    public bool IsRead { get; set; }

    public Guid SourceId { get; set; }

    public string? Title { get; set; }

    public int Type { get; set; }

    public Guid? UserId { get; set; }

    public Guid? SenderId { get; set; }

    public virtual User? User { get; set; }
}
