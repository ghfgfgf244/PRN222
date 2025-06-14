using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Patient
{
    public int PatientId { get; set; }

    public int RegisteredBy { get; set; }

    public string FullName { get; set; } = null!;

    public int Age { get; set; }

    public string? Gender { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User RegisteredByNavigation { get; set; } = null!;
}
