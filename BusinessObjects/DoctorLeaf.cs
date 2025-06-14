using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class DoctorLeaf
{
    public int LeaveId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly LeaveDate { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Doctor { get; set; } = null!;
}
