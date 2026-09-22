namespace LumoraAcademy.Models;

// Simple data classes used by the pages.
// Later these will be filled from the database instead of SampleData.

public class Student
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Id { get; set; } = "";
    public string Grade { get; set; } = "";
    public string AssignedTeacher { get; set; } = "";
    public string TeacherInitials { get; set; } = "";
    public string Status { get; set; } = "";      // Active, Inactive, Pending
    public string Initials { get; set; } = "";
}

public class Teacher
{
    public string Name { get; set; } = "";
    public string Subject { get; set; } = "";
    public string EmployeeId { get; set; } = "";
    public string Department { get; set; } = "";
    public string Subjects { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Status { get; set; } = "";      // Active, On Leave
    public string Initials { get; set; } = "";
}

public class ScheduleItem
{
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public string Title { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Status { get; set; } = "";      // PRESENT, ONGOING, or empty
}

public class SubjectCard
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Days { get; set; } = "";
    public string Time { get; set; } = "";
    public string Students { get; set; } = "";
    public string IconGlyph { get; set; } = "";
}

public class AttendanceRecord
{
    public string Date { get; set; } = "";
    public string Class { get; set; } = "";
    public string Subject { get; set; } = "";
    public int Total { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public string Status { get; set; } = "";      // Excellent, Poor, Perfect, Good
}

public class AtRiskStudent
{
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string Initials { get; set; } = "";
    public string Cohort { get; set; } = "";
    public string Subject1 { get; set; } = "";
    public string Subject2 { get; set; } = "";
    public string Average { get; set; } = "";
    public bool IsFalling { get; set; }
}

public class PaymentAccount
{
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string MonthlyFee { get; set; } = "";
    public string Outstanding { get; set; } = "";
    public string LastPaid { get; set; } = "";
    public string Status { get; set; } = "";      // Paid, Pending, Extension Granted, Dropout
    public string Action { get; set; } = "";      // View, Record, Review
}

public class Subject
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Curriculum { get; set; } = "";
    public string IconGlyph { get; set; } = "";
}

public class SchoolEvent
{
    public string Name { get; set; } = "";
    public string Date { get; set; } = "";
    public string Time { get; set; } = "";
    public string Location { get; set; } = "";
    public string Category { get; set; } = "";    // Academic, Admin, Holiday, Sports
}

public class UpcomingItem
{
    public string When { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string IconGlyph { get; set; } = "";
}

public class ActivityItem
{
    public string Message { get; set; } = "";
    public string TimeAgo { get; set; } = "";
    public string IconGlyph { get; set; } = "";
}

public class MarkAttendanceRow
{
    public int StudentDbId { get; set; }             // Students.Id in the database
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string Initials { get; set; } = "";
    public string Status { get; set; } = "";      // Present, Absent, Late
    public string Remarks { get; set; } = "";
}

public class ReportSubjectRow
{
    public string Subject { get; set; } = "";
    public int MonthlyTest { get; set; }
    public int TermExam { get; set; }
    public int FinalScore { get; set; }
    public string Grade { get; set; } = "";
    public string Remarks { get; set; } = "";
}

public class AssignedClass
{
    public string Grade { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Students { get; set; } = "";
}
