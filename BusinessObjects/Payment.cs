using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int AppointmentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime? PaidAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
