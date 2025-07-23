
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/appHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => console.log("✅ SignalR connected"))
    .catch(err => console.error("❌ SignalR connection error: ", err));

/* --- USER EVENTS --- */
connection.on("UserCreated", function (user) {
    Swal.fire({
        icon: "info",
        title: "Người dùng mới",
        text: `Họ tên: ${user.fullName} (${user.email})`,
        timer: 3000,
        toast: true,
        position: 'top-end',
        showConfirmButton: false
    });

    // Reload nếu đang ở trang danh sách
    if (window.location.pathname.includes("/Users/Index")) {
        location.reload();
    }
});

// === Handler khi user được cập nhật ===
connection.on("UserUpdated", function (user) {
    Swal.fire({
        icon: "success",
        title: "Đã cập nhật người dùng",
        text: `Họ tên: ${user.fullName}`,
        timer: 3000,
        toast: true,
        position: 'top-end',
        showConfirmButton: false
    });

    if (window.location.pathname.includes("/Users/Index")) {
        location.reload();
    }
});

// === Handler khi user bị xóa ===
connection.on("UserDeleted", function (userId) {
    Swal.fire({
        icon: "warning",
        title: "Người dùng đã bị xoá",
        text: `ID: ${userId}`,
        timer: 3000,
        toast: true,
        position: 'top-end',
        showConfirmButton: false
    });

    if (window.location.pathname.includes("/Users/Index")) {
        location.reload();
    }
});

/* --- REGISTER EVENTS --- */
connection.on("UserRegistered", user => {
    console.log("📝 User Registered:", user);
    // TODO: Cập nhật danh sách chờ xác thực
    location.reload();
});
connection.on("EmailVerified", userId => {
    console.log("✅ Email Verified:", userId);
    // TODO: Cập nhật trạng thái user
    location.reload();
});

/* --- APPOINTMENT EVENTS --- */
connection.on("AppointmentCreated", appt => {
    console.log("📅 Appointment Created:", appt);
    location.reload();
});
connection.on("AppointmentUpdated", appt => {
    console.log("✏️ Appointment Updated:", appt);
    location.reload();
});

/* --- DOCTOR LEAF EVENTS --- */
connection.on("DoctorLeafCreated", leaf => {
    console.log("🌴 Doctor Leave Created:", leaf);
    location.reload();
});
connection.on("DoctorLeafDeleted", leafId => {
    console.log("❌ Doctor Leave Deleted:", leafId);
    location.reload();
});

/* --- PATIENT EVENTS --- */
connection.on("PatientCreate", patient => {
    console.log("🧑‍⚕️ Patient Created:", patient);
    location.reload();
});
connection.on("PatientUpdate", patient => {
    console.log("🔄 Patient Updated:", patient);
    location.reload();
});

/* --- DOCTOR SPECIALTY EVENTS --- */
connection.on("DoctorSpecialtyUpdated", data => {
    console.log("🩺 Doctor Specialty Updated:", data);
    location.reload();
});
