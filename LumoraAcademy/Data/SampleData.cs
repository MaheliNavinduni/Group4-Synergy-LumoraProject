using LumoraAcademy.Models;

namespace LumoraAcademy.Data;

// Sample data so the screens look like the design.
// TODO: replace with real data from the database once the backend is ready.
public static class SampleData
{
    // Icon glyphs from the "Segoe MDL2 Assets" font that ships with Windows.
    public static class Icons
    {
        public const string Dashboard = "\uE80F";
        public const string Subjects = "\uE8F1";
        public const string Attendance = "\uE9D5";
        public const string Students = "\uE716";
        public const string Teachers = "\uE77B";
        public const string Payments = "\uE8C7";
        public const string Academics = "\uE82D";
        public const string Events = "\uE787";
        public const string Calendar = "\uE787";
        public const string Clock = "\uE823";
        public const string Person = "\uE77B";
        public const string People = "\uE716";
        public const string Mail = "\uE715";
        public const string Phone = "\uE717";
        public const string Location = "\uE81D";
        public const string Search = "\uE721";
        public const string Filter = "\uE71C";
        public const string Download = "\uE896";
        public const string Print = "\uE749";
        public const string Edit = "\uE70F";
        public const string Delete = "\uE74D";
        public const string Plus = "\uE710";
        public const string ArrowLeft = "\uE72B";
        public const string ArrowRight = "\uE72A";
        public const string ChevronDown = "\uE70D";
        public const string ChevronLeft = "\uE76B";
        public const string ChevronRight = "\uE76C";
        public const string Chart = "\uE9D2";
        public const string TrendUp = "\uE8CB";
        public const string Warning = "\uE7BA";
        public const string Check = "\uE73E";
        public const string Document = "\uE8A5";
        public const string Camera = "\uE722";
        public const string Upload = "\uE898";
        public const string Note = "\uE70B";
        public const string Star = "\uE734";
        public const string Building = "\uE80F";
        public const string Room = "\uE8AB";
        public const string Science = "\uE95E";
        public const string Math = "\uE8EF";
        public const string Language = "\uE8C1";
        public const string Money = "\uE825";
        public const string Home = "\uE80F";
        public const string List = "\uE8FD";
        public const string Grid = "\uE80A";
        public const string Refresh = "\uE72C";
        public const string Close = "\uE711";
        public const string Info = "\uE946";
        public const string Education = "\uE7BE";
    }

    public static List<ScheduleItem> TeacherTodaySchedule = new()
    {
        new ScheduleItem { StartTime = "08:00 AM", EndTime = "09:30 AM", Title = "European History AP", Detail = "Grade 11 • Room 204" },
        new ScheduleItem { StartTime = "09:45 AM", EndTime = "11:15 AM", Title = "World Geography", Detail = "Grade 9 • Room 206" },
        new ScheduleItem { StartTime = "11:30 AM", EndTime = "12:30 PM", Title = "Lunch / Prep Period", Detail = "" },
    };

    public static List<SubjectCard> TeacherSubjects = new()
    {
        new SubjectCard { Name = "Science", Code = "MAT-401 • Room 302", Days = "Mon, Wed, Fri", Time = "08:00 AM - 09:30 AM", Students = "24 Students", IconGlyph = Icons.Science },
        new SubjectCard { Name = "ICT", Code = "MAT-450 • Room 305", Days = "Tue, Thu", Time = "10:00 AM - 11:30 AM", Students = "18 Students", IconGlyph = Icons.Chart },
        new SubjectCard { Name = "Sinhala", Code = "MAT-201 • Room 210", Days = "Mon, Wed, Fri", Time = "01:00 PM - 02:00 PM", Students = "30 Students", IconGlyph = Icons.Language },
    };

    public static List<AttendanceRecord> AttendanceRecords = new()
    {
        new AttendanceRecord { Date = "Nov 30, 2026", Class = "Grade 10 - A", Subject = "Mathematics", Total = 32, Present = 30, Absent = 1, Late = 1, Status = "Excellent" },
        new AttendanceRecord { Date = "Nov 30, 2026", Class = "Grade 11 - B", Subject = "Physics", Total = 28, Present = 22, Absent = 4, Late = 2, Status = "Poor" },
        new AttendanceRecord { Date = "Nov 29, 2026", Class = "Grade 10 - A", Subject = "Literature", Total = 32, Present = 32, Absent = 0, Late = 0, Status = "Perfect" },
        new AttendanceRecord { Date = "Nov 29, 2026", Class = "Grade 9 - C", Subject = "History", Total = 30, Present = 27, Absent = 2, Late = 1, Status = "Good" },
    };

    public static List<Student> Students = new()
    {
        new Student { Name = "Alvarez, Marcus", Email = "marcus.a@student.edu", Id = "#STU-8492", Grade = "11th Grade", AssignedTeacher = "Dr. Richards", TeacherInitials = "DR", Status = "Active", Initials = "MA" },
        new Student { Name = "Chen, Emily", Email = "emily.c@student.edu", Id = "#STU-3321", Grade = "9th Grade", AssignedTeacher = "Ms. Smith", TeacherInitials = "MS", Status = "Active", Initials = "EC" },
        new Student { Name = "Johnson, Tyrell", Email = "tyrell.j@student.edu", Id = "#STU-1198", Grade = "12th Grade", AssignedTeacher = "Mr. Davis", TeacherInitials = "JD", Status = "Inactive", Initials = "TJ" },
        new Student { Name = "Patel, Aarav", Email = "aarav.p@student.edu", Id = "#STU-9942", Grade = "10th Grade", AssignedTeacher = "Ms. Smith", TeacherInitials = "MS", Status = "Pending", Initials = "AP" },
    };

    public static List<Teacher> Teachers = new()
    {
        new Teacher { Name = "Robert Chen", Subject = "Science", EmployeeId = "TCH-2048", Department = "STEM", Subjects = "AP Physics, Chem", Email = "r.chen@eduadmin.com", Phone = "+1 (555) 123-4567", Status = "Active", Initials = "RC" },
        new Teacher { Name = "Sarah Jenkins", Subject = "English Lit", EmployeeId = "TCH-1932", Department = "Humanities", Subjects = "Literature, Creative Writing", Email = "s.jenkins@eduadmin.com", Phone = "+1 (555) 234-5678", Status = "On Leave", Initials = "SJ" },
        new Teacher { Name = "Marcus Johnson", Subject = "Phys Ed", EmployeeId = "TCH-2104", Department = "Athletics", Subjects = "Health, Basketball", Email = "m.johnson@eduadmin.com", Phone = "+1 (555) 345-6789", Status = "Active", Initials = "MJ" },
        new Teacher { Name = "Elena Lopez", Subject = "Mathematics", EmployeeId = "TCH-1085", Department = "STEM", Subjects = "Calculus, Algebra", Email = "e.lopez@eduadmin.com", Phone = "+1 (555) 456-7890", Status = "Active", Initials = "EL" },
    };

    public static List<AtRiskStudent> AtRiskStudents = new()
    {
        new AtRiskStudent { Name = "Emma Stone", Id = "ID: 2024-0891", Initials = "ES", Cohort = "Grade 10 - Alpha", Subject1 = "Mathematics", Subject2 = "Physics", Average = "54%", IsFalling = true },
        new AtRiskStudent { Name = "James Chen", Id = "ID: 2024-0442", Initials = "JC", Cohort = "Grade 11 - Beta", Subject1 = "Literature", Subject2 = "", Average = "62%", IsFalling = false },
        new AtRiskStudent { Name = "Mia Patel", Id = "ID: 2024-1102", Initials = "MP", Cohort = "Grade 10 - Alpha", Subject1 = "Chemistry", Subject2 = "Biology", Average = "48%", IsFalling = true },
    };

    public static List<PaymentAccount> PaymentAccounts = new()
    {
        new PaymentAccount { Name = "Emma Thompson", Id = "STU-2023-9942", MonthlyFee = "$450.00", Outstanding = "$0.00", LastPaid = "Oct 12, 2023", Status = "Paid", Action = "View" },
        new PaymentAccount { Name = "Liam Chen", Id = "STU-2023-3881", MonthlyFee = "$450.00", Outstanding = "$450.00", LastPaid = "Sep 15, 2023", Status = "Pending", Action = "Record" },
        new PaymentAccount { Name = "Sophia Martinez", Id = "STU-2023-1145", MonthlyFee = "$550.00", Outstanding = "$1,650.00", LastPaid = "Aug 05, 2023", Status = "Extension Granted", Action = "Record" },
        new PaymentAccount { Name = "Noah Jackson", Id = "STU-2023-7102", MonthlyFee = "$450.00", Outstanding = "$1,350.00", LastPaid = "Jan 15, 2023", Status = "Dropout", Action = "Review" },
    };

    public static List<Subject> Subjects = new()
    {
        new Subject { Name = "Science - S1022", Code = "S1022", Curriculum = "National Curriculum", IconGlyph = Icons.Science },
        new Subject { Name = "Mathematics - M2103", Code = "M2103", Curriculum = "National Curriculum", IconGlyph = Icons.Math },
        new Subject { Name = "Science - S2350", Code = "S2350", Curriculum = "Cambridge Curriculum", IconGlyph = Icons.Science },
        new Subject { Name = "Mathematics - M1203", Code = "M1203", Curriculum = "Cambridge Curriculum", IconGlyph = Icons.Math },
        new Subject { Name = "Languages - E1289", Code = "E1289", Curriculum = "English", IconGlyph = Icons.Language },
        new Subject { Name = "Languages - T5632", Code = "T5632", Curriculum = "Second Language Tamil", IconGlyph = Icons.Language },
        new Subject { Name = "Mathematics - M4859", Code = "M4859", Curriculum = "Cambridge Curriculum", IconGlyph = Icons.Math },
    };

    public static List<SchoolEvent> Events = new()
    {
        new SchoolEvent { Name = "Spring Science Fair", Date = "Apr 15, 2024", Time = "09:00 AM - 03:00 PM", Location = "Main Gymnasium", Category = "Academic" },
        new SchoolEvent { Name = "Staff Development Day", Date = "Apr 22, 2024", Time = "08:00 AM - 04:00 PM", Location = "Virtual / Zoom", Category = "Admin" },
        new SchoolEvent { Name = "Memorial Day Holiday", Date = "May 27, 2024", Time = "All Day", Location = "Campus Closed", Category = "Holiday" },
        new SchoolEvent { Name = "Varsity Basketball Finals", Date = "Jun 02, 2024", Time = "06:00 PM", Location = "City Arena", Category = "Sports" },
    };

    public static List<UpcomingItem> UpcomingThisWeek = new()
    {
        new UpcomingItem { When = "Today, 3:30 PM", Title = "Faculty Meeting", Description = "Monthly all-staff meeting in the main auditorium.", IconGlyph = Icons.Clock },
        new UpcomingItem { When = "Thu, Oct 12", Title = "Parent-Teacher Conferences", Description = "Scheduled blocks for 1-on-1 parent meetings.", IconGlyph = Icons.Calendar },
        new UpcomingItem { When = "Fri, Oct 13", Title = "Grades Submission Deadline", Description = "All mid-term grades must be finalized in the system.", IconGlyph = Icons.Warning },
    };

    public static List<ActivityItem> RecentActivity = new()
    {
        new ActivityItem { Message = "Sarah Jenkins registered as a new student.", TimeAgo = "10 mins ago", IconGlyph = Icons.Person },
        new ActivityItem { Message = "Mr. Davis updated syllabus for Biology 101.", TimeAgo = "1 hour ago", IconGlyph = Icons.Document },
        new ActivityItem { Message = "Payment failed for invoice #4432.", TimeAgo = "4 hours ago", IconGlyph = Icons.Warning },
    };

    public static List<ScheduleItem> StudentTodaySchedule = new()
    {
        new ScheduleItem { StartTime = "08:00", EndTime = "AM", Title = "AP Physics", Detail = "Room 402 • Dr. Aris", Status = "PRESENT" },
        new ScheduleItem { StartTime = "09:30", EndTime = "AM", Title = "World Literature", Detail = "Room 210 • Ms. Blake", Status = "PRESENT" },
        new ScheduleItem { StartTime = "11:00", EndTime = "AM", Title = "Calculus II", Detail = "Room 305 • Mr. Chen", Status = "ONGOING" },
    };

    public static List<ScheduleItem> TeacherDetailSchedule = new()
    {
        new ScheduleItem { StartTime = "08:00 AM - 09:30 AM", Title = "Advanced Calculus", Detail = "Grade 12 - A", Status = "Room 304" },
        new ScheduleItem { StartTime = "09:45 AM - 11:15 AM", Title = "AP Statistics", Detail = "Grade 11 - B", Status = "Room 304" },
        new ScheduleItem { StartTime = "11:30 AM - 12:30 PM", Title = "Planning Period", Detail = "—", Status = "Staff Room B" },
        new ScheduleItem { StartTime = "02:00 PM - 03:30 PM", Title = "Geometry", Detail = "Grade 10 - C", Status = "Room 212" },
    };

    public static List<AssignedClass> AssignedClasses = new()
    {
        new AssignedClass { Grade = "Grade 12 - A", Subject = "Advanced Calculus", Students = "32 Students" },
        new AssignedClass { Grade = "Grade 11 - B", Subject = "AP Statistics", Students = "28 Students" },
        new AssignedClass { Grade = "Grade 10 - C", Subject = "Geometry", Students = "35 Students" },
    };

    public static List<MarkAttendanceRow> MarkAttendanceRows = new()
    {
        new MarkAttendanceRow { Name = "Alice Anderson", Id = "STD-11B-001", Initials = "AA", Status = "Present", Remarks = "" },
        new MarkAttendanceRow { Name = "Brian Benson", Id = "STD-11B-002", Initials = "BB", Status = "Absent", Remarks = "Medical leave" },
        new MarkAttendanceRow { Name = "Chloe Carter", Id = "STD-11B-003", Initials = "CC", Status = "Late", Remarks = "Arrived at 9:15 AM" },
        new MarkAttendanceRow { Name = "David Davis", Id = "STD-11B-004", Initials = "DD", Status = "Present", Remarks = "" },
    };

    public static List<ReportSubjectRow> ReportRows = new()
    {
        new ReportSubjectRow { Subject = "Mathematics", MonthlyTest = 88, TermExam = 92, FinalScore = 90, Grade = "A-", Remarks = "Excellent analytical skills. Consistent effort." },
        new ReportSubjectRow { Subject = "Sinhala", MonthlyTest = 95, TermExam = 94, FinalScore = 95, Grade = "A", Remarks = "Outstanding essay compositions. Active participant." },
        new ReportSubjectRow { Subject = "ICT", MonthlyTest = 82, TermExam = 86, FinalScore = 84, Grade = "B", Remarks = "Solid grasp of concepts, needs work on lab reports." },
        new ReportSubjectRow { Subject = "Second language Tamil", MonthlyTest = 91, TermExam = 89, FinalScore = 90, Grade = "A-", Remarks = "Strong critical thinking demonstrated in class debates." },
        new ReportSubjectRow { Subject = "Science", MonthlyTest = 96, TermExam = 98, FinalScore = 97, Grade = "A+", Remarks = "Exceptional translation abilities. Top of the class." },
    };
}
