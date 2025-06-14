using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class DoctorSpecialty
{
    public int SpecialtyId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DoctorDetail> DoctorDetails { get; set; } = new List<DoctorDetail>();
}
