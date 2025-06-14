using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class DoctorDetail
{
    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public virtual User Doctor { get; set; } = null!;

    public virtual DoctorSpecialty Specialty { get; set; } = null!;
}
