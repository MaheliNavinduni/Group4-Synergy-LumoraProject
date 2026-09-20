using LumoraAcademy.Core.Entities;
using LumoraAcademy.Core.Security;

namespace LumoraAcademy.Core.Database;

// Fills an empty database with demo rows so the screens have something to show.
// Runs once, the first time the app starts. Safe to delete once real data exists.
public static class DemoDataSeeder
{
    public static void SeedIfEmpty(AppDatabase db)
    {
        if (!db.IsEmpty()) return;
        Seed(db);
    }

    public static void Seed(AppDatabase db)
    {
        var c = db.Connection;

        // ---------- Teachers ----------
        var aries = new Teacher { TeacherId = "TCH-1001", FullName = "Miss. Aries", Designation = "Senior Mathematics Faculty", Department = "STEM", Subjects = "Science, ICT, Sinhala", Email = "Aries@eduadmin.com", Phone = "+1 (555) 019-1001", JoiningDate = new DateTime(2018, 8, 15), YearsOfExperience = 8, Status = "Active", DateOfBirth = new DateTime(1990, 3, 2), Gender = "Female", BloodGroup = "A+" };
        var chen = new Teacher { TeacherId = "TCH-2048", FullName = "Robert Chen", Designation = "Senior Mathematics", Department = "STEM", Subjects = "AP Physics, Chem", Email = "r.chen@eduadmin.com", Phone = "+1 (555) 123-4567", Address = "1429 Elm Street, Springfield, IL 62704", JoiningDate = new DateTime(2012, 8, 19), YearsOfExperience = 12, Status = "Active", DateOfBirth = new DateTime(1980, 5, 12), Gender = "Male", BloodGroup = "O+" };
        var jenkins = new Teacher { TeacherId = "TCH-1932", FullName = "Sarah Jenkins", Designation = "English Lit", Department = "Humanities", Subjects = "Literature, Creative Writing", Email = "s.jenkins@eduadmin.com", Phone = "+1 (555) 234-5678", JoiningDate = new DateTime(2015, 1, 10), YearsOfExperience = 9, Status = "On Leave", DateOfBirth = new DateTime(1985, 7, 21), Gender = "Female", BloodGroup = "B+" };
        var johnson = new Teacher { TeacherId = "TCH-2104", FullName = "Marcus Johnson", Designation = "Phys Ed", Department = "Athletics", Subjects = "Health, Basketball", Email = "m.johnson@eduadmin.com", Phone = "+1 (555) 345-6789", JoiningDate = new DateTime(2019, 2, 1), YearsOfExperience = 5, Status = "Active", DateOfBirth = new DateTime(1992, 11, 3), Gender = "Male", BloodGroup = "AB+" };
        var lopez = new Teacher { TeacherId = "TCH-1085", FullName = "Elena Lopez", Designation = "Mathematics", Department = "STEM", Subjects = "Calculus, Algebra", Email = "e.lopez@eduadmin.com", Phone = "+1 (555) 456-7890", JoiningDate = new DateTime(2010, 9, 1), YearsOfExperience = 14, Status = "Active", DateOfBirth = new DateTime(1982, 1, 30), Gender = "Female", BloodGroup = "O-" };
        var davis = new Teacher { TeacherId = "TCH-1550", FullName = "Mr. Davis", Designation = "Homeroom 11B", Department = "Humanities", Subjects = "History", Email = "davis@eduadmin.com", Phone = "+1 (555) 019-1550", JoiningDate = new DateTime(2016, 4, 4), YearsOfExperience = 7, Status = "Active", Gender = "Male" };
        var richards = new Teacher { TeacherId = "TCH-1200", FullName = "Dr. Richards", Designation = "Science Lead", Department = "STEM", Subjects = "Biology", Email = "richards@eduadmin.com", Phone = "+1 (555) 019-1200", JoiningDate = new DateTime(2011, 6, 1), YearsOfExperience = 13, Status = "Active", Gender = "Male" };
        var smith = new Teacher { TeacherId = "TCH-1300", FullName = "Ms. Smith", Designation = "English", Department = "Humanities", Subjects = "English", Email = "smith@eduadmin.com", Phone = "+1 (555) 019-1300", JoiningDate = new DateTime(2017, 3, 20), YearsOfExperience = 6, Status = "Active", Gender = "Female" };
        c.InsertAll(new[] { aries, chen, jenkins, johnson, lopez, davis, richards, smith });

        // ---------- Login accounts ----------
        var (adminHash, adminSalt) = PasswordHasher.Hash("admin123");
        var (teacherHash, teacherSalt) = PasswordHasher.Hash("teacher123");
        c.Insert(new User { Username = "admin", PasswordHash = adminHash, PasswordSalt = adminSalt, Role = "Admin", DisplayName = "Admin", IsActive = true });
        c.Insert(new User { Username = "teacher", PasswordHash = teacherHash, PasswordSalt = teacherSalt, Role = "Teacher", DisplayName = "Miss. Aries", IsActive = true, TeacherId = aries.Id });

        // ---------- Subjects ----------
        var maths = new Subject { Code = "M2103", Name = "Mathematics", Curriculum = "National Curriculum" };
        var science = new Subject { Code = "S1022", Name = "Science", Curriculum = "National Curriculum" };
        var scienceCam = new Subject { Code = "S2350", Name = "Science", Curriculum = "Cambridge Curriculum" };
        var mathsCam = new Subject { Code = "M1203", Name = "Mathematics", Curriculum = "Cambridge Curriculum" };
        var english = new Subject { Code = "E1289", Name = "Languages", Curriculum = "English" };
        var tamil = new Subject { Code = "T5632", Name = "Languages", Curriculum = "Second Language Tamil" };
        var mathsCam2 = new Subject { Code = "M4859", Name = "Mathematics", Curriculum = "Cambridge Curriculum" };
        var ict = new Subject { Code = "I3001", Name = "ICT", Curriculum = "National Curriculum" };
        var sinhala = new Subject { Code = "L2001", Name = "Sinhala", Curriculum = "National Curriculum" };
        c.InsertAll(new[] { maths, science, scienceCam, mathsCam, english, tamil, mathsCam2, ict, sinhala });

        // ---------- Students ----------
        var eleanor = new Student { StudentId = "STU-2023-8941", FullName = "Eleanor Vance", DateOfBirth = new DateTime(2006, 10, 12), Gender = "Female", BloodGroup = "A+", School = "District 4, Hillside High School", Grade = "11th Grade", Section = "B", Email = "eleanor.v@student.edu", GuardianName = "Arthur Vance (Father)", GuardianPhone = "+1 (555) 019-2834", GuardianEmail = "a.vance@email.com", JoiningDate = new DateTime(2021, 8, 15), AssignedTeacherId = davis.Id, Status = "Active" };
        var marcus = new Student { StudentId = "STU-8492", FullName = "Marcus Alvarez", DateOfBirth = new DateTime(2008, 2, 14), Gender = "Male", Grade = "11th Grade", Section = "A", Email = "marcus.a@student.edu", GuardianName = "Luis Alvarez", GuardianPhone = "+1 (555) 200-8492", JoiningDate = new DateTime(2022, 1, 10), AssignedTeacherId = richards.Id, Status = "Active" };
        var emily = new Student { StudentId = "STU-3321", FullName = "Emily Chen", DateOfBirth = new DateTime(2010, 6, 1), Gender = "Female", Grade = "9th Grade", Section = "A", Email = "emily.c@student.edu", GuardianName = "Wei Chen", GuardianPhone = "+1 (555) 200-3321", JoiningDate = new DateTime(2023, 9, 1), AssignedTeacherId = smith.Id, Status = "Active" };
        var tyrell = new Student { StudentId = "STU-1198", FullName = "Tyrell Johnson", DateOfBirth = new DateTime(2007, 3, 9), Gender = "Male", Grade = "12th Grade", Section = "C", Email = "tyrell.j@student.edu", GuardianName = "Dana Johnson", GuardianPhone = "+1 (555) 200-1198", JoiningDate = new DateTime(2020, 8, 20), AssignedTeacherId = davis.Id, Status = "Inactive" };
        var aarav = new Student { StudentId = "STU-9942", FullName = "Aarav Patel", DateOfBirth = new DateTime(2009, 12, 25), Gender = "Male", Grade = "10th Grade", Section = "A", Email = "aarav.p@student.edu", GuardianName = "Priya Patel", GuardianPhone = "+1 (555) 200-9942", JoiningDate = new DateTime(2024, 1, 8), AssignedTeacherId = smith.Id, Status = "Pending" };
        var emma = new Student { StudentId = "STU-2024-0891", FullName = "Emma Stone", DateOfBirth = new DateTime(2009, 4, 18), Gender = "Female", Grade = "10th Grade", Section = "Alpha", Email = "emma.s@student.edu", GuardianName = "Karen Stone", GuardianPhone = "+1 (555) 200-0891", JoiningDate = new DateTime(2024, 1, 15), AssignedTeacherId = aries.Id, Status = "Active" };
        var james = new Student { StudentId = "STU-2024-0442", FullName = "James Chen", DateOfBirth = new DateTime(2008, 9, 2), Gender = "Male", Grade = "11th Grade", Section = "Beta", Email = "james.c@student.edu", GuardianName = "Mei Chen", GuardianPhone = "+1 (555) 200-0442", JoiningDate = new DateTime(2024, 1, 15), AssignedTeacherId = aries.Id, Status = "Active" };
        var mia = new Student { StudentId = "STU-2024-1102", FullName = "Mia Patel", DateOfBirth = new DateTime(2009, 7, 30), Gender = "Female", Grade = "10th Grade", Section = "Alpha", Email = "mia.p@student.edu", GuardianName = "Raj Patel", GuardianPhone = "+1 (555) 200-1102", JoiningDate = new DateTime(2024, 1, 15), AssignedTeacherId = aries.Id, Status = "Active" };
        var alexander = new Student { StudentId = "SEA-24-0891", FullName = "Alexander J. Sterling", DateOfBirth = new DateTime(2009, 1, 20), Gender = "Male", Grade = "10th Grade", Section = "A", Email = "alex.s@student.edu", GuardianName = "Margaret Sterling", GuardianPhone = "+1 (555) 200-0777", JoiningDate = new DateTime(2023, 8, 1), AssignedTeacherId = aries.Id, Status = "Active" };
        var emmaT = new Student { StudentId = "STU-2023-9942", FullName = "Emma Thompson", DateOfBirth = new DateTime(2008, 5, 5), Gender = "Female", Grade = "11th Grade", Section = "A", Email = "emma.t@student.edu", GuardianName = "Paul Thompson", GuardianPhone = "+1 (555) 200-9943", JoiningDate = new DateTime(2023, 1, 9), AssignedTeacherId = lopez.Id, Status = "Active" };
        var liam = new Student { StudentId = "STU-2023-3881", FullName = "Liam Chen", DateOfBirth = new DateTime(2008, 8, 8), Gender = "Male", Grade = "11th Grade", Section = "A", Email = "liam.c@student.edu", GuardianName = "Anna Chen", GuardianPhone = "+1 (555) 200-3881", JoiningDate = new DateTime(2023, 1, 9), AssignedTeacherId = lopez.Id, Status = "Active" };
        var sophia = new Student { StudentId = "STU-2023-1145", FullName = "Sophia Martinez", DateOfBirth = new DateTime(2007, 11, 11), Gender = "Female", Grade = "12th Grade", Section = "B", Email = "sophia.m@student.edu", GuardianName = "Carlos Martinez", GuardianPhone = "+1 (555) 200-1145", JoiningDate = new DateTime(2022, 8, 22), AssignedTeacherId = chen.Id, Status = "Active" };
        var noah = new Student { StudentId = "STU-2023-7102", FullName = "Noah Jackson", DateOfBirth = new DateTime(2008, 2, 2), Gender = "Male", Grade = "11th Grade", Section = "C", Email = "noah.j@student.edu", GuardianName = "Tina Jackson", GuardianPhone = "+1 (555) 200-7102", JoiningDate = new DateTime(2022, 8, 22), AssignedTeacherId = chen.Id, Status = "Dropout", DepartureDate = new DateTime(2023, 6, 30), DepartureReason = "Relocated" };
        c.InsertAll(new[] { eleanor, marcus, emily, tyrell, aarav, emma, james, mia, alexander, emmaT, liam, sophia, noah });

        // ---------- Exam results ----------
        // Alexander's report card (Academic Progress Report page)
        AddResults(c, alexander.Id, maths.Id, aries.Id, 88, 92, "Excellent analytical skills. Consistent effort.");
        AddResults(c, alexander.Id, sinhala.Id, aries.Id, 95, 94, "Outstanding essay compositions. Active participant.");
        AddResults(c, alexander.Id, ict.Id, aries.Id, 82, 86, "Solid grasp of concepts, needs work on lab reports.");
        AddResults(c, alexander.Id, tamil.Id, aries.Id, 91, 89, "Strong critical thinking demonstrated in class debates.");
        AddResults(c, alexander.Id, science.Id, aries.Id, 96, 98, "Exceptional translation abilities. Top of the class.");
        // Cambridge (external) result
        c.Insert(new ExamResult { StudentId = alexander.Id, SubjectId = mathsCam.Id, ExamType = "Cambridge", ExamName = "IGCSE May 2026", Marks = 87, Grade = "A", RecordedOn = DateTime.Now, RecordedByTeacherId = aries.Id });

        // At-risk students (averages below 60%)
        AddResults(c, emma.Id, maths.Id, aries.Id, 52, 56, "Struggling with algebra.");
        AddResults(c, emma.Id, science.Id, aries.Id, 50, 58, "Needs support in physics topics.");
        AddResults(c, james.Id, english.Id, aries.Id, 60, 64, "Reading comprehension improving.");
        AddResults(c, mia.Id, science.Id, aries.Id, 45, 51, "Chemistry and biology both below target.");
        AddResults(c, mia.Id, maths.Id, aries.Id, 58, 62, "");
        // Eleanor (good student)
        AddResults(c, eleanor.Id, maths.Id, lopez.Id, 93, 95, "Excellent.");
        AddResults(c, eleanor.Id, science.Id, richards.Id, 90, 94, "Very strong.");

        // ---------- Progress notes ----------
        c.Insert(new ProgressNote { StudentId = emma.Id, TeacherId = aries.Id, Note = "Recommend extra Mathematics class on Saturdays.", RecommendExtraClass = true, CreatedOn = DateTime.Now.AddDays(-7) });

        // ---------- Payments ----------
        var thisMonth = DateTime.Now.ToString("yyyy-MM");
        c.Insert(new Payment { StudentId = emmaT.Id, Month = thisMonth, AmountDue = 450, AmountPaid = 450, DueDate = new DateTime(2023, 10, 15), PaymentDate = new DateTime(2023, 10, 12) });
        c.Insert(new Payment { StudentId = liam.Id, Month = thisMonth, AmountDue = 450, AmountPaid = 0, DueDate = DateTime.Today.AddDays(10) });
        c.Insert(new Payment { StudentId = sophia.Id, Month = thisMonth, AmountDue = 1650, AmountPaid = 0, DueDate = DateTime.Today.AddDays(-20), ExtensionUntil = DateTime.Today.AddMonths(1), Remarks = "Parent requested extension until next month due to bank delay. Approved by Admin." });
        c.Insert(new Payment { StudentId = noah.Id, Month = "2023-01", AmountDue = 1350, AmountPaid = 0, DueDate = new DateTime(2023, 1, 31), PaymentDate = new DateTime(2023, 1, 15) });
        c.Insert(new Payment { StudentId = eleanor.Id, Month = thisMonth, AmountDue = 450, AmountPaid = 450, DueDate = DateTime.Today.AddDays(5), PaymentDate = DateTime.Today.AddDays(-2) });

        // ---------- Attendance ----------
        var g10a = new[] { emma, mia, alexander, aarav };
        var g11b = new[] { eleanor, james, emmaT, liam };
        AddAttendanceDay(c, DateTime.Today.AddDays(-1), "Grade 10 - A", maths.Id, g10a, absent: 0, late: 1);
        AddAttendanceDay(c, DateTime.Today.AddDays(-1), "Grade 11 - B", science.Id, g11b, absent: 2, late: 1);
        AddAttendanceDay(c, DateTime.Today.AddDays(-2), "Grade 10 - A", english.Id, g10a, absent: 0, late: 0);
        AddAttendanceDay(c, DateTime.Today.AddDays(-2), "Grade 11 - B", maths.Id, g11b, absent: 1, late: 0);

        // ---------- Timetable ----------
        // Miss. Aries: Science with 10th Grade (Mon/Wed/Fri), ICT with 11th Grade (Tue/Thu), Sinhala with 10th Grade (Mon/Wed/Fri)
        foreach (int day in new[] { 1, 3, 5 })
        {
            c.Insert(new ClassSession { TeacherId = aries.Id, SubjectId = science.Id, ClassName = "10th Grade", DayOfWeek = day, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 30, 0), Room = "302" });
            c.Insert(new ClassSession { TeacherId = aries.Id, SubjectId = sinhala.Id, ClassName = "10th Grade", DayOfWeek = day, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(14, 0, 0), Room = "210" });
        }
        foreach (int day in new[] { 2, 4 })
        {
            c.Insert(new ClassSession { TeacherId = aries.Id, SubjectId = ict.Id, ClassName = "11th Grade", DayOfWeek = day, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 30, 0), Room = "305" });
        }
        // Robert Chen and Elena Lopez
        c.Insert(new ClassSession { TeacherId = chen.Id, SubjectId = mathsCam.Id, ClassName = "12th Grade", DayOfWeek = 1, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 30, 0), Room = "304" });
        c.Insert(new ClassSession { TeacherId = chen.Id, SubjectId = mathsCam.Id, ClassName = "11th Grade", DayOfWeek = 1, StartTime = new TimeSpan(9, 45, 0), EndTime = new TimeSpan(11, 15, 0), Room = "304" });
        c.Insert(new ClassSession { TeacherId = chen.Id, SubjectId = maths.Id, ClassName = "10th Grade", DayOfWeek = 1, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 30, 0), Room = "212" });
        c.Insert(new ClassSession { TeacherId = lopez.Id, SubjectId = maths.Id, ClassName = "11th Grade", DayOfWeek = 3, StartTime = new TimeSpan(9, 30, 0), EndTime = new TimeSpan(11, 0, 0), Room = "305" });

        // ---------- Events ----------
        c.InsertAll(new[]
        {
            new SchoolEvent { Title = "Spring Science Fair", Date = new DateTime(2024, 4, 15), Time = "09:00 AM - 03:00 PM", Location = "Main Gymnasium", Category = "Academic", Description = "Annual showcase of student science projects across all grade levels. Setup begins at 7 AM." },
            new SchoolEvent { Title = "Staff Development Day", Date = new DateTime(2024, 4, 22), Time = "08:00 AM - 04:00 PM", Location = "Virtual / Zoom", Category = "Admin", Description = "Training for all teaching staff." },
            new SchoolEvent { Title = "Memorial Day Holiday", Date = new DateTime(2024, 5, 27), Time = "All Day", Location = "Campus Closed", Category = "Holiday", Description = "" },
            new SchoolEvent { Title = "Varsity Basketball Finals", Date = new DateTime(2024, 6, 2), Time = "06:00 PM", Location = "City Arena", Category = "Sports", Description = "" },
            new SchoolEvent { Title = "Faculty Meeting", Date = DateTime.Today, Time = "03:30 PM", Location = "Main Auditorium", Category = "Admin", Description = "Monthly all-staff meeting in the main auditorium." },
            new SchoolEvent { Title = "Parent-Teacher Conferences", Date = DateTime.Today.AddDays(1), Time = "All Day", Location = "Classrooms", Category = "Academic", Description = "Scheduled blocks for 1-on-1 parent meetings." },
            new SchoolEvent { Title = "Grades Submission Deadline", Date = DateTime.Today.AddDays(2), Time = "05:00 PM", Location = "Online", Category = "Academic", Description = "All mid-term grades must be finalized in the system." },
        });
    }

    // Adds a monthly and a term result for one subject.
    private static void AddResults(SQLite.SQLiteConnection c, int studentId, int subjectId, int teacherId, double monthly, double term, string remarks)
    {
        c.Insert(new ExamResult { StudentId = studentId, SubjectId = subjectId, ExamType = "Monthly", ExamName = "August 2026", Marks = monthly, Grade = ExamResult.GradeFor(monthly), RecordedOn = DateTime.Now.AddDays(-30), RecordedByTeacherId = teacherId });
        c.Insert(new ExamResult { StudentId = studentId, SubjectId = subjectId, ExamType = "Term", ExamName = "Term 2 2026", Marks = term, Grade = ExamResult.GradeFor(term), Remarks = remarks, RecordedOn = DateTime.Now.AddDays(-5), RecordedByTeacherId = teacherId });
    }

    // Marks a whole class for one day: the first `absent` students absent, the next `late` late, the rest present.
    private static void AddAttendanceDay(SQLite.SQLiteConnection c, DateTime date, string className, int subjectId, Student[] students, int absent, int late)
    {
        for (int i = 0; i < students.Length; i++)
        {
            string status = i < absent ? "Absent" : i < absent + late ? "Late" : "Present";
            c.Insert(new AttendanceEntry { StudentId = students[i].Id, Date = date.Date, ClassName = className, SubjectId = subjectId, Status = status });
        }
    }
}
