using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Họ tên không được bỏ trống")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
    [RegularExpression(@"^(0|\+84)(3[2-9]|5[2689]|7[06-9]|8[1-9]|9[0-9])[0-9]{7}$",
        ErrorMessage = "Số điện thoại không hợp lệ (theo định dạng Việt Nam)")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Email không được bỏ trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public bool Gender { get; set; }

    public DateTime Birthday { get; set; }

    public string Role { get; set; } = null!;

    public string? Avatar { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DoctorLeaf> DoctorLeaves { get; set; } = new List<DoctorLeaf>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<DoctorSpecialty> Specialties { get; set; } = new List<DoctorSpecialty>();
}
