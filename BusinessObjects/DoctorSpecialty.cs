using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class DoctorSpecialty
{
    public int SpecialtyId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ExamMethod> ExamMethods { get; set; } = new List<ExamMethod>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
