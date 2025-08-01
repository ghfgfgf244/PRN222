using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class Patient
{
    public int PatientId { get; set; }

    public int? RegisteredBy { get; set; }

    public string FullName { get; set; } = null!;

    [Range(1, 150, ErrorMessage = "Tuổi không được bé hơn 1")]
    public int Age { get; set; }

    public bool Gender { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User? RegisteredByNavigation { get; set; }
}
