using Microsoft.Data.SqlClient;
using SchoolBusApp.Models;

namespace SchoolBusApp
{
    public static class DataAccess
    {
        // ─── STUDENTS ───────────────────────────────────────────
        public static List<Student> GetStudents(string search = "")
        {
            var list = new List<Student>();
            using var conn = Database.GetConnection();
            conn.Open();
            var sql = @"SELECT s.*, r.NameAr AS RouteName FROM Students s
                        LEFT JOIN Routes r ON s.RouteId = r.Id
                        WHERE s.IsActive = 1
                        AND (s.NameAr LIKE @s OR s.NameEn LIKE @s OR s.ParentPhone LIKE @s OR s.SchoolName LIKE @s)
                        ORDER BY s.NameAr";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Student {
                    Id = (int)r["Id"], NameAr = r["NameAr"].ToString()!,
                    NameEn = r["NameEn"].ToString()!, Phone = r["Phone"].ToString()!,
                    ParentName = r["ParentName"].ToString()!, ParentPhone = r["ParentPhone"].ToString()!,
                    Address = r["Address"].ToString()!, SchoolName = r["SchoolName"].ToString()!,
                    Grade = r["Grade"].ToString()!, RouteId = r["RouteId"] as int?,
                    RouteName = r["RouteName"].ToString()!, IsActive = (bool)r["IsActive"]
                });
            return list;
        }

        public static void SaveStudent(Student s)
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = s.Id == 0
                ? "INSERT INTO Students(NameAr,NameEn,Phone,ParentName,ParentPhone,Address,SchoolName,Grade,RouteId) VALUES(@a,@e,@p,@pn,@pp,@ad,@sc,@gr,@ri)"
                : "UPDATE Students SET NameAr=@a,NameEn=@e,Phone=@p,ParentName=@pn,ParentPhone=@pp,Address=@ad,SchoolName=@sc,Grade=@gr,RouteId=@ri WHERE Id=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@a", s.NameAr);
            cmd.Parameters.AddWithValue("@e", s.NameEn);
            cmd.Parameters.AddWithValue("@p", s.Phone);
            cmd.Parameters.AddWithValue("@pn", s.ParentName);
            cmd.Parameters.AddWithValue("@pp", s.ParentPhone);
            cmd.Parameters.AddWithValue("@ad", s.Address);
            cmd.Parameters.AddWithValue("@sc", s.SchoolName);
            cmd.Parameters.AddWithValue("@gr", s.Grade);
            cmd.Parameters.AddWithValue("@ri", (object?)s.RouteId ?? DBNull.Value);
            if (s.Id != 0) cmd.Parameters.AddWithValue("@id", s.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteStudent(int id)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("UPDATE Students SET IsActive=0 WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ─── DRIVERS ────────────────────────────────────────────
        public static List<Driver> GetDrivers()
        {
            var list = new List<Driver>();
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Drivers WHERE IsActive=1 ORDER BY NameAr", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Driver { Id=(int)r["Id"], NameAr=r["NameAr"].ToString()!,
                    NameEn=r["NameEn"].ToString()!, Phone=r["Phone"].ToString()!,
                    LicenseNumber=r["LicenseNumber"].ToString()!, IsActive=(bool)r["IsActive"] });
            return list;
        }

        public static void SaveDriver(Driver d)
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = d.Id == 0
                ? "INSERT INTO Drivers(NameAr,NameEn,Phone,LicenseNumber) VALUES(@a,@e,@p,@l)"
                : "UPDATE Drivers SET NameAr=@a,NameEn=@e,Phone=@p,LicenseNumber=@l WHERE Id=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@a", d.NameAr);
            cmd.Parameters.AddWithValue("@e", d.NameEn);
            cmd.Parameters.AddWithValue("@p", d.Phone);
            cmd.Parameters.AddWithValue("@l", d.LicenseNumber);
            if (d.Id != 0) cmd.Parameters.AddWithValue("@id", d.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteDriver(int id)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("UPDATE Drivers SET IsActive=0 WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        // ─── BUSES ──────────────────────────────────────────────
        public static List<Bus> GetBuses()
        {
            var list = new List<Bus>();
            using var conn = Database.GetConnection(); conn.Open();
            var sql = @"SELECT b.*, d.NameAr AS DriverName FROM Buses b
                        LEFT JOIN Drivers d ON b.DriverId = d.Id WHERE b.IsActive=1";
            using var cmd = new SqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Bus { Id=(int)r["Id"], PlateNumber=r["PlateNumber"].ToString()!,
                    Model=r["Model"].ToString()!, Capacity=(int)r["Capacity"],
                    DriverId=r["DriverId"] as int?, DriverName=r["DriverName"].ToString()!, IsActive=(bool)r["IsActive"] });
            return list;
        }

        public static void SaveBus(Bus b)
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = b.Id == 0
                ? "INSERT INTO Buses(PlateNumber,Model,Capacity,DriverId) VALUES(@p,@m,@c,@d)"
                : "UPDATE Buses SET PlateNumber=@p,Model=@m,Capacity=@c,DriverId=@d WHERE Id=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", b.PlateNumber);
            cmd.Parameters.AddWithValue("@m", b.Model);
            cmd.Parameters.AddWithValue("@c", b.Capacity);
            cmd.Parameters.AddWithValue("@d", (object?)b.DriverId ?? DBNull.Value);
            if (b.Id != 0) cmd.Parameters.AddWithValue("@id", b.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteBus(int id)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("UPDATE Buses SET IsActive=0 WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        // ─── ROUTES ─────────────────────────────────────────────
        public static List<Route> GetRoutes()
        {
            var list = new List<Route>();
            using var conn = Database.GetConnection(); conn.Open();
            var sql = @"SELECT r.*, b.PlateNumber AS BusPlate FROM Routes r
                        LEFT JOIN Buses b ON r.BusId = b.Id ORDER BY r.NameAr";
            using var cmd = new SqlCommand(sql, conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new Route { Id=(int)rd["Id"], NameAr=rd["NameAr"].ToString()!,
                    NameEn=rd["NameEn"].ToString()!, BusId=rd["BusId"] as int?,
                    BusPlate=rd["BusPlate"].ToString()!,
                    DepartureTime=rd["DepartureTime"].ToString()!,
                    ReturnTime=rd["ReturnTime"].ToString()!,
                    Notes=rd["Notes"].ToString()! });
            return list;
        }

        public static void SaveRoute(Route ro)
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = ro.Id == 0
                ? "INSERT INTO Routes(NameAr,NameEn,BusId,DepartureTime,ReturnTime,Notes) VALUES(@a,@e,@b,@dt,@rt,@n)"
                : "UPDATE Routes SET NameAr=@a,NameEn=@e,BusId=@b,DepartureTime=@dt,ReturnTime=@rt,Notes=@n WHERE Id=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@a", ro.NameAr);
            cmd.Parameters.AddWithValue("@e", ro.NameEn);
            cmd.Parameters.AddWithValue("@b", (object?)ro.BusId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@dt", ro.DepartureTime == "" ? DBNull.Value : ro.DepartureTime);
            cmd.Parameters.AddWithValue("@rt", ro.ReturnTime == "" ? DBNull.Value : ro.ReturnTime);
            cmd.Parameters.AddWithValue("@n", ro.Notes);
            if (ro.Id != 0) cmd.Parameters.AddWithValue("@id", ro.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteRoute(int id)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Routes WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        // ─── ATTENDANCE ─────────────────────────────────────────
        public static List<AttendanceRecord> GetAttendance(DateTime date)
        {
            var list = new List<AttendanceRecord>();
            using var conn = Database.GetConnection(); conn.Open();
            var sql = @"SELECT a.*, s.NameAr AS StudentName FROM Attendance a
                        JOIN Students s ON a.StudentId = s.Id
                        WHERE a.AttendanceDate = @d ORDER BY s.NameAr";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@d", date.Date);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new AttendanceRecord { Id=(int)r["Id"], StudentId=(int)r["StudentId"],
                    StudentName=r["StudentName"].ToString()!,
                    AttendanceDate=Convert.ToDateTime(r["AttendanceDate"]),
                    MorningStatus=r["MorningStatus"].ToString()!,
                    EveningStatus=r["EveningStatus"].ToString()!,
                    Notes=r["Notes"].ToString()! });
            return list;
        }

        public static void GenerateDailyAttendance(DateTime date)
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = @"INSERT INTO Attendance(StudentId, AttendanceDate)
                        SELECT Id, @d FROM Students WHERE IsActive=1
                        AND Id NOT IN (SELECT StudentId FROM Attendance WHERE AttendanceDate=@d)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@d", date.Date);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateAttendance(int id, string morning, string evening, string notes)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Attendance SET MorningStatus=@m, EveningStatus=@e, Notes=@n WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@m", morning);
            cmd.Parameters.AddWithValue("@e", evening);
            cmd.Parameters.AddWithValue("@n", notes);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteAttendance(int id)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Attendance WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ─── PAYMENTS ───────────────────────────────────────────
        public static List<Payment> GetPayments(string search = "", int year = 0)
        {
            var list = new List<Payment>();
            using var conn = Database.GetConnection(); conn.Open();
            var sql = @"SELECT p.*, s.NameAr AS StudentName FROM Payments p
                        JOIN Students s ON p.StudentId = s.Id
                        WHERE (s.NameAr LIKE @s OR s.NameEn LIKE @s)
                        AND (@y = 0 OR p.Year = @y)
                        ORDER BY p.PaymentDate DESC";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            cmd.Parameters.AddWithValue("@y", year);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new Payment { Id=(int)r["Id"], StudentId=(int)r["StudentId"],
                    StudentName=r["StudentName"].ToString()!,
                    Amount=Convert.ToDecimal(r["Amount"]),
                    PaymentDate=Convert.ToDateTime(r["PaymentDate"]),
                    Month=r["Month"].ToString()!, Year=Convert.ToInt32(r["Year"]),
                    Status=r["Status"].ToString()!, Notes=r["Notes"].ToString()! });
            return list;
        }

        public static void SavePayment(Payment p)
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = p.Id == 0
                ? "INSERT INTO Payments(StudentId,Amount,PaymentDate,Month,Year,Status,Notes) VALUES(@si,@am,@pd,@mo,@yr,@st,@no)"
                : "UPDATE Payments SET StudentId=@si,Amount=@am,PaymentDate=@pd,Month=@mo,Year=@yr,Status=@st,Notes=@no WHERE Id=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@si", p.StudentId);
            cmd.Parameters.AddWithValue("@am", p.Amount);
            cmd.Parameters.AddWithValue("@pd", p.PaymentDate);
            cmd.Parameters.AddWithValue("@mo", p.Month);
            cmd.Parameters.AddWithValue("@yr", p.Year);
            cmd.Parameters.AddWithValue("@st", p.Status);
            cmd.Parameters.AddWithValue("@no", p.Notes);
            if (p.Id != 0) cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeletePayment(int id)
        {
            using var conn = Database.GetConnection(); conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Payments WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        // ─── DASHBOARD STATS ────────────────────────────────────
        public static (int students, int buses, int drivers, int routes, decimal todayPayments, int presentToday) GetDashboardStats()
        {
            using var conn = Database.GetConnection(); conn.Open();
            var sql = @"
                SELECT
                    (SELECT COUNT(*) FROM Students WHERE IsActive=1) AS Students,
                    (SELECT COUNT(*) FROM Buses WHERE IsActive=1) AS Buses,
                    (SELECT COUNT(*) FROM Drivers WHERE IsActive=1) AS Drivers,
                    (SELECT COUNT(*) FROM Routes) AS Routes,
                    (SELECT ISNULL(SUM(Amount),0) FROM Payments WHERE PaymentDate=CAST(GETDATE() AS DATE)) AS TodayPay,
                    (SELECT COUNT(*) FROM Attendance WHERE AttendanceDate=CAST(GETDATE() AS DATE) AND MorningStatus='Present') AS PresentToday";
            using var cmd = new SqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            r.Read();
            return ((int)r["Students"], (int)r["Buses"], (int)r["Drivers"],
                    (int)r["Routes"], Convert.ToDecimal(r["TodayPay"]), (int)r["PresentToday"]);
        }
    }
}
