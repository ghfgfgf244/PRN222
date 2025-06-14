using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public bool Gender { get; set; }

    public DateTime Birthday { get; set; }

    public string Role { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual DoctorDetail? DoctorDetail { get; set; }

    public virtual ICollection<DoctorLeaf> DoctorLeaves { get; set; } = new List<DoctorLeaf>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
