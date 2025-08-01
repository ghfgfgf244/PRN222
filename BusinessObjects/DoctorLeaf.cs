using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class DoctorLeaf
{
    public int LeaveId { get; set; }

    public int? DoctorId { get; set; }

    public DateOnly LeaveDate { get; set; }

    [Required]
    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual User? Doctor { get; set; }
}
