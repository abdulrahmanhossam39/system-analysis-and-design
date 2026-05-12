namespace SchoolBusApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = "";
        public string NameEn { get; set; } = "";
        public string Phone { get; set; } = "";
        public string ParentName { get; set; } = "";
        public string ParentPhone { get; set; } = "";
        public string Address { get; set; } = "";
        public string SchoolName { get; set; } = "";
        public string Grade { get; set; } = "";
        public int? RouteId { get; set; }
        public string RouteName { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Today;
        public override string ToString() => $"{NameAr} - {NameEn}";
    }

    public class Driver
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = "";
        public string NameEn { get; set; } = "";
        public string Phone { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public override string ToString() => NameAr;
    }

    public class Bus
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = "";
        public string Model { get; set; } = "";
        public int Capacity { get; set; }
        public int? DriverId { get; set; }
        public string DriverName { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public override string ToString() => PlateNumber;
    }

    public class Route
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = "";
        public string NameEn { get; set; } = "";
        public int? BusId { get; set; }
        public string BusPlate { get; set; } = "";
        public string DepartureTime { get; set; } = "";
        public string ReturnTime { get; set; } = "";
        public string Notes { get; set; } = "";
        public override string ToString() => NameAr;
    }

    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public DateTime AttendanceDate { get; set; } = DateTime.Today;
        public string MorningStatus { get; set; } = "Present";
        public string EveningStatus { get; set; } = "Present";
        public string Notes { get; set; } = "";
    }

    public class Payment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Today;
        public string Month { get; set; } = "";
        public int Year { get; set; } = DateTime.Today.Year;
        public string Status { get; set; } = "Paid";
        public string Notes { get; set; } = "";
    }
}
