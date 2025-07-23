CREATE DATABASE AppointmentsDB;
GO
USE AppointmentsDB;
GO

--User
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE,
    [Password] VARCHAR(255) NOT NULL,
    Gender BIT NOT NULL, -- 0 = N?, 1 = Nam
    Birthday DATETIME NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Patient', 'Doctor')),
    Avatar VARCHAR(MAX)NULL,

    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1 -- Cho phép khoá/m? tài kho?n
);
GO


--chuyên ngành
CREATE TABLE DoctorSpecialties (
    SpecialtyID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);
GO


--Bác s?
CREATE TABLE DoctorDetails (
    UserID INT,
    SpecialtyID INT,

    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (SpecialtyID) REFERENCES DoctorSpecialties(SpecialtyID),

    PRIMARY KEY (UserID, SpecialtyID));
GO


--B?nh nhân
CREATE TABLE Patients (
    PatientID INT PRIMARY KEY IDENTITY(1,1),
    RegisteredBy INT, -- ng??i ??ng ký (UserID)
    FullName NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Gender BIT NOT NULL,
    Note NVARCHAR(255),
    FOREIGN KEY (RegisteredBy) REFERENCES Users(UserID)
);
GO

CREATE TABLE TimeSlots (
    SlotID INT PRIMARY KEY IDENTITY(1,1),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL
);

CREATE TABLE ExamMethods (
    MethodID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    SpecialtyID INT,
    FOREIGN KEY (SpecialtyID) REFERENCES DoctorSpecialties(SpecialtyID)
);

--l?ch h?n khám
CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT,
    DoctorID INT,
    SpecialtyID INT,
    MethodID INT,
    AppointmentDate DATE NOT NULL,
    SlotID INT, -- ➤ Thay vì TimeSlot TIME

    Status NVARCHAR(20) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Users(UserID),
    FOREIGN KEY (SpecialtyID) REFERENCES DoctorSpecialties(SpecialtyID),
    FOREIGN KEY (MethodID) REFERENCES ExamMethods(MethodID),
    FOREIGN KEY (SlotID) REFERENCES TimeSlots(SlotID)
);

GO

--bác s? ngh? phép
CREATE TABLE DoctorLeaves (
    LeaveID INT PRIMARY KEY IDENTITY(1,1),
    DoctorID INT,
    LeaveDate DATE NOT NULL,
    Reason NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
	IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (DoctorID) REFERENCES Users(UserID),
    CONSTRAINT UQ_Doctor_Leave UNIQUE (DoctorID, LeaveDate) -- Không trùng ngày ngh?
);
GO

INSERT INTO TimeSlots (StartTime, EndTime)
VALUES
('08:00', '08:30'),
('08:30', '09:00'),
('09:00', '09:30'),
('09:30', '10:00'),
('10:00', '10:30'),
('10:30', '11:00'),
('11:00', '11:30'),
('11:30', '12:00'),
('13:00', '13:30'),
('13:30', '14:00'),
('14:00', '14:30'),
('14:30', '15:00'),
('15:00', '15:30'),
('15:30', '16:00'),
('16:00', '16:30'),
('16:30', '17:00');
GO

INSERT INTO DoctorSpecialties (Name) VALUES 
(N'Da liễu'),
(N'Tai mũi họng'),
(N'Nội tổng quát'),
(N'Xương khớp'),
(N'Tim mạch'),
(N'Nhi khoa'),
(N'Sản phụ khoa');
GO

INSERT INTO ExamMethods (Name, SpecialtyID) VALUES 
-- Da liễu (ID = 1)
(N'Khám da tổng quát', 1),
(N'Phân tích dị ứng da', 1),
(N'Khám mụn trứng cá', 1),
(N'Điều trị sẹo bằng laser', 1),
(N'Khám nấm da', 1),
(N'Phân tích da kỹ thuật số', 1),

-- Tai mũi họng (ID = 2)
(N'Nội soi tai mũi họng', 2),
(N'Test dị ứng tai mũi họng', 2),
(N'Chụp CT tai xương đá', 2),
(N'Đo thính lực', 2),

-- Nội tổng quát (ID = 3)
(N'Khám tổng quát', 3),
(N'Xét nghiệm máu cơ bản', 3),
(N'Xét nghiệm nước tiểu', 3),
(N'Đo huyết áp', 3),
(N'Khám gan mật', 3),
(N'Khám dạ dày', 3),

-- Xương khớp (ID = 4)
(N'Chụp X-quang xương', 4),
(N'Đo mật độ xương', 4),
(N'Khám thoái hóa khớp', 4),
(N'Chụp MRI cột sống', 4),
(N'Khám đau lưng – cột sống', 4),
(N'Điện cơ (EMG)', 4),

-- Tim mạch (ID = 5)
(N'Điện tâm đồ (ECG)', 5),
(N'Siêu âm tim', 5),
(N'Đo Holter ECG', 5),
(N'Test gắng sức', 5),
(N'Siêu âm Doppler tim', 5),
(N'Khám tăng huyết áp', 5),

-- Nhi khoa (ID = 6)
(N'Tư vấn phát triển trẻ em', 6),
(N'Khám nhi tổng quát', 6),
(N'Tiêm chủng định kỳ', 6),
(N'Tư vấn dinh dưỡng cho trẻ', 6),
(N'Đo chiều cao – cân nặng', 6),
(N'Khám sốt siêu vi', 6),

-- Sản phụ khoa (ID = 7)
(N'Khám phụ khoa', 7),
(N'Siêu âm thai', 7),
(N'Khám thai định kỳ', 7),
(N'Xét nghiệm PAP smear', 7),
(N'Nội soi cổ tử cung', 7),
(N'Khám vô sinh nữ', 7);
GO
