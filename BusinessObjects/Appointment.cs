using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public string? ExamMethod { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public string TimeSlot { get; set; } = null!;

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual DoctorSpecialty Specialty { get; set; } = null!;
}
