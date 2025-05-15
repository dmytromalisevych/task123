
-- Видалення існуючих таблиць якщо вони є
DROP TABLE IF EXISTS Appointments;
DROP TABLE IF EXISTS Doctors;
DROP TABLE IF EXISTS Patients;
DROP TABLE IF EXISTS AspNetRoleClaims;
DROP TABLE IF EXISTS AspNetUserClaims;
DROP TABLE IF EXISTS AspNetUserLogins;
DROP TABLE IF EXISTS AspNetUserRoles;
DROP TABLE IF EXISTS AspNetRoles;
DROP TABLE IF EXISTS AspNetUserTokens;
DROP TABLE IF EXISTS AspNetUsers;

-- Створення таблиць для Identity
CREATE TABLE AspNetRoles (
Id TEXT PRIMARY KEY,
Name TEXT,
NormalizedName TEXT,
ConcurrencyStamp TEXT
);

CREATE TABLE AspNetUsers (
Id TEXT PRIMARY KEY,
UserName TEXT,
NormalizedUserName TEXT,
Email TEXT,
NormalizedEmail TEXT,
EmailConfirmed INTEGER NOT NULL,
PasswordHash TEXT,
SecurityStamp TEXT,
ConcurrencyStamp TEXT,
PhoneNumber TEXT,
PhoneNumberConfirmed INTEGER NOT NULL,
TwoFactorEnabled INTEGER NOT NULL,
LockoutEnd TEXT,
LockoutEnabled INTEGER NOT NULL,
AccessFailedCount INTEGER NOT NULL
);

CREATE TABLE AspNetRoleClaims (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
RoleId TEXT NOT NULL,
ClaimType TEXT,
ClaimValue TEXT,
FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserClaims (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
UserId TEXT NOT NULL,
ClaimType TEXT,
ClaimValue TEXT,
FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserLogins (
LoginProvider TEXT NOT NULL,
ProviderKey TEXT NOT NULL,
ProviderDisplayName TEXT,
UserId TEXT NOT NULL,
PRIMARY KEY (LoginProvider, ProviderKey),
FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserRoles (
UserId TEXT NOT NULL,
 RoleId TEXT NOT NULL,
PRIMARY KEY (UserId, RoleId),
FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserTokens (
UserId TEXT NOT NULL,
LoginProvider TEXT NOT NULL,
Name TEXT NOT NULL,
Value TEXT,
PRIMARY KEY (UserId, LoginProvider, Name),
 FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

-- Створення таблиці лікарів з полями автентифікації
CREATE TABLE Doctors (
DoctorId INTEGER PRIMARY KEY AUTOINCREMENT,
FirstName TEXT NOT NULL,
LastName TEXT NOT NULL,
Specialization TEXT NOT NULL,
Email TEXT NOT NULL UNIQUE,
PasswordHash TEXT NOT NULL
);

-- Створення таблиці пацієнтів з полями автентифікації
CREATE TABLE Patients (
PatientId INTEGER PRIMARY KEY AUTOINCREMENT,
FirstName TEXT NOT NULL,
LastName TEXT NOT NULL,
DateOfBirth DATE NOT NULL,
Email TEXT NOT NULL UNIQUE,
PasswordHash TEXT NOT NULL
);

-- Створення таблиці призначень
CREATE TABLE Appointments (
AppointmentId INTEGER PRIMARY KEY AUTOINCREMENT,
DoctorId INTEGER NOT NULL,
PatientId INTEGER NOT NULL,
AppointmentDateTime DATETIME NOT NULL,
Status TEXT NOT NULL,
Notes TEXT,
FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
FOREIGN KEY (PatientId) REFERENCES Patients(PatientId)
);

-- Додавання базових ролей
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES
('1', 'Doctor', 'DOCTOR', '1'),
('2', 'Patient', 'PATIENT', '2');

-- Додавання тестових даних для лікарів (з хешованими паролями)
INSERT INTO Doctors (FirstName, LastName, Specialization, Email, PasswordHash) VALUES
('Марія', 'Коваленко', 'Терапевт', 'maria@hospital.com', 'AQAAAAIAAYagAAAAELYmkPILbCrZYWnxrEPXxFcQg4GFoAr3KNZ8wHgrI3US7zEp0KfG/yQlF/yGJkB4mA=='),
('Олександр', 'Петренко', 'Кардіолог', 'alex@hospital.com', 'AQAAAAIAAYagAAAAELYmkPILbCrZYWnxrEPXxFcQg4GFoAr3KNZ8wHgrI3US7zEp0KfG/yQlF/yGJkB4mA=='),
('Ірина', 'Мельник', 'Невролог', 'iryna@hospital.com', 'AQAAAAIAAYagAAAAELYmkPILbCrZYWnxrEPXxFcQg4GFoAr3KNZ8wHgrI3US7zEp0KfG/yQlF/yGJkB4mA==');

-- Додавання тестових даних для пацієнтів (з хешованими паролями)
INSERT INTO Patients (FirstName, LastName, DateOfBirth, Email, PasswordHash) VALUES
('Тетяна', 'Кириленко', '1990-05-15', 'tetiana@gmail.com', 'AQAAAAIAAYagAAAAELYmkPILbCrZYWnxrEPXxFcQg4GFoAr3KNZ8wHgrI3US7zEp0KfG/yQlF/yGJkB4mA=='),
('Наталія', 'Бондаренко', '1985-08-20', 'natalia@gmail.com', 'AQAAAAIAAYagAAAAELYmkPILbCrZYWnxrEPXxFcQg4GFoAr3KNZ8wHgrI3US7zEp0KfG/yQlF/yGJkB4mA=='),
('Максим', 'Романенко', '1995-03-10', 'maxim@gmail.com', 'AQAAAAIAAYagAAAAELYmkPILbCrZYWnxrEPXxFcQg4GFoAr3KNZ8wHgrI3US7zEp0KfG/yQlF/yGJkB4mA==');

-- Додавання тестових даних для призначень
INSERT INTO Appointments (DoctorId, PatientId, AppointmentDateTime, Status, Notes) VALUES
(1, 1, '2025-05-09 10:30:00', 'Scheduled', 'Первинний огляд'),
(2, 2, '2025-05-09 13:30:00', 'Scheduled', 'Планова консультація'),
(2, 3, '2025-05-09 15:30:00', 'Scheduled', 'Повторний прийом');
