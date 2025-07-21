using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class ExamMethod
{
    public int MethodId { get; set; }

    public string Name { get; set; } = null!;

    public int? SpecialtyId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual DoctorSpecialty? Specialty { get; set; }
}
