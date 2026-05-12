using Microsoft.Data.SqlClient;

namespace SchoolBusApp
{
    public static class Database
    {
        public static string ConnectionString { get; set; } =
            "Data Source=DESKTOP-7O2V9PM\\SQLEXPRESS;Initial Catalog=student_transport;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        public static void InitializeDatabase()
        {
            using var conn = GetConnection();
            conn.Open();

            var tables = new[]
            {
                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Students' AND xtype='U')
                  CREATE TABLE Students (
                      Id INT IDENTITY(1,1) PRIMARY KEY,
                      NameAr NVARCHAR(100) NOT NULL,
                      NameEn NVARCHAR(100),
                      Phone NVARCHAR(20),
                      ParentName NVARCHAR(100),
                      ParentPhone NVARCHAR(20),
                      Address NVARCHAR(200),
                      SchoolName NVARCHAR(100),
                      Grade NVARCHAR(50),
                      RouteId INT,
                      IsActive BIT DEFAULT 1,
                      CreatedDate DATE DEFAULT GETDATE()
                  )",

                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Drivers' AND xtype='U')
                  CREATE TABLE Drivers (
                      Id INT IDENTITY(1,1) PRIMARY KEY,
                      NameAr NVARCHAR(100) NOT NULL,
                      NameEn NVARCHAR(100),
                      Phone NVARCHAR(20),
                      LicenseNumber NVARCHAR(50),
                      IsActive BIT DEFAULT 1
                  )",

                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Buses' AND xtype='U')
                  CREATE TABLE Buses (
                      Id INT IDENTITY(1,1) PRIMARY KEY,
                      PlateNumber NVARCHAR(20) NOT NULL,
                      Model NVARCHAR(50),
                      Capacity INT DEFAULT 0,
                      DriverId INT,
                      IsActive BIT DEFAULT 1
                  )",

                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Routes' AND xtype='U')
                  CREATE TABLE Routes (
                      Id INT IDENTITY(1,1) PRIMARY KEY,
                      NameAr NVARCHAR(100) NOT NULL,
                      NameEn NVARCHAR(100),
                      BusId INT,
                      DepartureTime NVARCHAR(10),
                      ReturnTime NVARCHAR(10),
                      Notes NVARCHAR(200)
                  )",

                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Attendance' AND xtype='U')
                  CREATE TABLE Attendance (
                      Id INT IDENTITY(1,1) PRIMARY KEY,
                      StudentId INT NOT NULL,
                      AttendanceDate DATE NOT NULL,
                      MorningStatus NVARCHAR(10) DEFAULT 'Present',
                      EveningStatus NVARCHAR(10) DEFAULT 'Present',
                      Notes NVARCHAR(200)
                  )",

                @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Payments' AND xtype='U')
                  CREATE TABLE Payments (
                      Id INT IDENTITY(1,1) PRIMARY KEY,
                      StudentId INT NOT NULL,
                      Amount DECIMAL(10,2) NOT NULL,
                      PaymentDate DATE,
                      Month NVARCHAR(20),
                      Year INT,
                      Status NVARCHAR(20) DEFAULT 'Paid',
                      Notes NVARCHAR(200)
                  )"
            };

            foreach (var sql in tables)
            {
                using var cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
