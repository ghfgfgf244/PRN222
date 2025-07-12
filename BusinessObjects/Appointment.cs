using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public int MethodId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public int SlotId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Doctor { get; set; } = null!;

    public virtual ExamMethod Method { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual TimeSlot Slot { get; set; } = null!;

    public virtual DoctorSpecialty Specialty { get; set; } = null!;
}
