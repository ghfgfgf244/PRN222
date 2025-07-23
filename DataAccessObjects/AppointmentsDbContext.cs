using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObjects;

public partial class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext()
    {
    }

    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<DoctorLeaf> DoctorLeaves { get; set; }

    public virtual DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }

    public virtual DbSet<ExamMethod> ExamMethods { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
        return configuration["ConnectionStrings:MyStockDB"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2B850B13A");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.MethodId).HasColumnName("MethodID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.SlotId).HasColumnName("SlotID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Appointme__Docto__4E88ABD4");

            entity.HasOne(d => d.Method).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.MethodId)
                .HasConstraintName("FK__Appointme__Metho__5070F446");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Appointme__Patie__4D94879B");

            entity.HasOne(d => d.Slot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__Appointme__SlotI__5165187F");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Appointme__Speci__4F7CD00D");
        });

        modelBuilder.Entity<DoctorLeaf>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__DoctorLe__796DB979B5FC7E1C");

            entity.HasIndex(e => new { e.DoctorId, e.LeaveDate }, "UQ_Doctor_Leave").IsUnique();

            entity.Property(e => e.LeaveId).HasColumnName("LeaveID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Reason).HasMaxLength(255);

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorLeaves)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__DoctorLea__Docto__571DF1D5");
        });

        modelBuilder.Entity<DoctorSpecialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__DoctorSp__D768F64819A80FDE");

            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ExamMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__ExamMeth__FC681FB1E3713C73");

            entity.Property(e => e.MethodId).HasColumnName("MethodID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");

            entity.HasOne(d => d.Specialty).WithMany(p => p.ExamMethods)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__ExamMetho__Speci__48CFD27E");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346A05198D6");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.RegisteredByNavigation).WithMany(p => p.Patients)
                .HasForeignKey(d => d.RegisteredBy)
                .HasConstraintName("FK__Patients__Regist__440B1D61");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__TimeSlot__0A124A4F656859BB");

            entity.Property(e => e.SlotId).HasColumnName("SlotID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE78DE7BF");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Users__85FB4E38D2C959BF").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053476A00598").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Avatar).IsUnicode(false);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(20);

            entity.HasMany(d => d.Specialties).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "DoctorDetail",
                    r => r.HasOne<DoctorSpecialty>().WithMany()
                        .HasForeignKey("SpecialtyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DoctorDet__Speci__412EB0B6"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DoctorDet__UserI__403A8C7D"),
                    j =>
                    {
                        j.HasKey("UserId", "SpecialtyId").HasName("PK__DoctorDe__8AFE43C80AFCF063");
                        j.ToTable("DoctorDetails");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("SpecialtyId").HasColumnName("SpecialtyID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
