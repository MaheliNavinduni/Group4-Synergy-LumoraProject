# Test Cases — Lumora Educational Institute Student Management System

ICT 2243 Software Engineering — Group 4 (Synergy)


**Total test cases:** 148  
**Covered by automated tests:** 125  
**Checked by hand on the running system:** 73


The automated tests are xUnit tests in `LumoraAcademy.Tests`. Run them with:

```
dotnet test LumoraAcademy.Tests/LumoraAcademy.Tests.csproj
```

The **Automated Test** column gives the name of the test method that proves that case, so a marker can find it in the code.

A test run reports more results than there are method names, because a `[Theory]` test runs once for each set of example data. The current run reports **138 passing**.

---

## Contents

1. [Login and Accounts](#1-login-and-accounts) — 11 cases
2. [Student Management](#2-student-management) — 30 cases
3. [Teacher Management](#3-teacher-management) — 13 cases
4. [Subjects and Classes](#4-subjects-and-classes) — 16 cases
5. [Attendance](#5-attendance) — 16 cases
6. [Payments and Fees](#6-payments-and-fees) — 21 cases
7. [Academic Performance](#7-academic-performance) — 11 cases
8. [Events](#8-events) — 5 cases
9. [Reports](#9-reports) — 8 cases
10. [User Interface](#10-user-interface) — 10 cases
11. [Non-functional](#11-non-functional) — 7 cases


## 1. Login and Accounts

### TC-AUTH-01 — Admin logs in with correct details

| Field | Detail |
|---|---|
| **Preconditions** | The system is installed and open on the Login page. |
| **Test steps** | 1. Enter the admin username.<br>2. Enter the admin password.<br>3. Click Login. |
| **Test data** | admin / admin123 |
| **Expected result** | The Admin Dashboard opens and the side menu shows Admin. |
| **Type** | Automated + Manual |
| **Automated test** | `Login_WithCorrectAdminCredentials_ReturnsAdminUser` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-02 — Teacher logs in with correct details

| Field | Detail |
|---|---|
| **Preconditions** | A teacher account exists. |
| **Test steps** | 1. Enter the teacher username.<br>2. Enter the password.<br>3. Click Login. |
| **Test data** | teacher / teacher123 |
| **Expected result** | The Teacher Dashboard opens and the login is linked to that teacher's record. |
| **Type** | Automated + Manual |
| **Automated test** | `Login_WithCorrectTeacherCredentials_ReturnsTeacherLinkedToTeacherRecord` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-03 — Wrong password is refused

| Field | Detail |
|---|---|
| **Preconditions** | The admin account exists. |
| **Test steps** | 1. Enter a valid username.<br>2. Enter the wrong password.<br>3. Click Login. |
| **Test data** | admin / wrongpass |
| **Expected result** | Login fails. A red message reads 'Incorrect username or password.' The password box is cleared. |
| **Type** | Automated + Manual |
| **Automated test** | `Login_WithWrongPassword_ReturnsNull` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-04 — Unknown username is refused

| Field | Detail |
|---|---|
| **Preconditions** | The system is on the Login page. |
| **Test steps** | 1. Enter a username that does not exist.<br>2. Enter any password.<br>3. Click Login. |
| **Test data** | nosuchuser / abc123 |
| **Expected result** | Login fails with the same message. No hint is given about which field was wrong. |
| **Type** | Automated |
| **Automated test** | `Login_WithUnknownUser_ReturnsNull` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-05 — Empty fields are refused

| Field | Detail |
|---|---|
| **Preconditions** | The system is on the Login page. |
| **Test steps** | 1. Leave the username empty.<br>2. Click Login.<br>3. Enter a username, leave the password empty.<br>4. Click Login. |
| **Test data** | (blank) |
| **Expected result** | Step 2 shows 'Please enter your username.' Step 4 shows 'Please enter your password.' The cursor moves to the empty box. |
| **Type** | Automated + Manual |
| **Automated test** | `Login_WithEmptyFields_ReturnsNull` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-06 — Username is not case sensitive

| Field | Detail |
|---|---|
| **Preconditions** | The admin account exists. |
| **Test steps** | 1. Enter the username in capitals.<br>2. Enter the correct password.<br>3. Click Login. |
| **Test data** | ADMIN / admin123 |
| **Expected result** | Login succeeds. |
| **Type** | Automated |
| **Automated test** | `Login_IsCaseInsensitiveForUsername` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-07 — A resigned teacher can no longer log in

| Field | Detail |
|---|---|
| **Preconditions** | A teacher has been put through Teacher Resignation. |
| **Test steps** | 1. Process the teacher's resignation.<br>2. Try to log in as that teacher. |
| **Test data** | The resigned teacher's username and password |
| **Expected result** | Login is refused. The teacher record is kept and shows Resigned. |
| **Type** | Automated |
| **Automated test** | `ProcessResignation_MarksResignedAndBlocksLogin` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-08 — A deactivated account cannot log in

| Field | Detail |
|---|---|
| **Preconditions** | A user account has been deactivated. |
| **Test steps** | 1. Deactivate the account.<br>2. Try to log in with it. |
| **Test data** | Any deactivated account |
| **Expected result** | Login is refused. |
| **Type** | Automated |
| **Automated test** | `Login_DeactivatedAccount_ReturnsNull` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-09 — Passwords are not stored as plain text

| Field | Detail |
|---|---|
| **Preconditions** | At least one account exists. |
| **Test steps** | 1. Open the Users table in the database file.<br>2. Look at the stored password. |
| **Test data** | admin123 |
| **Expected result** | The stored value is a PBKDF2 hash with its own salt. The typed password does not appear anywhere. |
| **Type** | Automated |
| **Automated test** | `PasswordIsStoredAsHash_NotPlainText` |
| **Actual result** | |
| **Status** | |

### TC-AUTH-10 — Enter key submits the login form

| Field | Detail |
|---|---|
| **Preconditions** | The system is on the Login page. |
| **Test steps** | 1. Type the username and press Enter.<br>2. Type the password and press Enter. |
| **Test data** | admin / admin123 |
| **Expected result** | Enter in the username box moves to the password box. Enter in the password box logs in without using the mouse. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-AUTH-11 — Log out returns to the Home page

| Field | Detail |
|---|---|
| **Preconditions** | A user is logged in. |
| **Test steps** | 1. Click Log out in the side menu. |
| **Test data** | - |
| **Expected result** | The Home page opens. Going back does not return to a signed-in page. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |


## 2. Student Management

### TC-STU-01 — Register a student with valid details

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as Admin, on Students > Register New Student. |
| **Test steps** | 1. Fill in every required field.<br>2. Click Save Registration. |
| **Test data** | Name: Nimali Perera, DOB 25/09/2010, Guardian: Sunil Perera, Phone 0771234567, Address: 45 Galle Road Colombo 03, Grade 10 |
| **Expected result** | The student is saved and a unique ID such as STU-2026-0041 is created and shown. |
| **Type** | Automated + Manual |
| **Automated test** | `Register_SavesStudentAndGeneratesUniqueId` |
| **Actual result** | |
| **Status** | |

### TC-STU-02 — Two students get different IDs

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as Admin. |
| **Test steps** | 1. Register one student.<br>2. Register a second student. |
| **Test data** | Two different names |
| **Expected result** | The two students have different generated IDs. |
| **Type** | Automated |
| **Automated test** | `Register_TwoStudents_GetDifferentGeneratedIds` |
| **Actual result** | |
| **Status** | |

### TC-STU-03 — A duplicate student ID is refused

| Field | Detail |
|---|---|
| **Preconditions** | A student with a given ID already exists. |
| **Test steps** | 1. Try to save another student with the same ID. |
| **Test data** | An ID already in use |
| **Expected result** | The save is refused with a clear message. Nothing is written to the database. |
| **Type** | Automated |
| **Automated test** | `Register_DuplicateStudentId_Throws` |
| **Actual result** | |
| **Status** | |

### TC-STU-04 — Empty required fields are refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Leave the form empty.<br>2. Click Save Registration. |
| **Test data** | (blank) |
| **Expected result** | A red bar appears at the top of the form naming the first missing field. Nothing is saved. |
| **Type** | Automated + Manual |
| **Automated test** | `Register_MissingMandatoryFields_Throws` |
| **Actual result** | |
| **Status** | |

### TC-STU-05 — A name containing digits is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Type a name with numbers in it.<br>2. Click Save Registration. |
| **Test data** | Student 123 |
| **Expected result** | The form is refused: names may only contain letters, spaces, dots and apostrophes. |
| **Type** | Automated |
| **Automated test** | `Name_RejectsBadNames` |
| **Actual result** | |
| **Status** | |

### TC-STU-06 — An invalid email is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Fill the form correctly but type a bad email.<br>2. Click Save Registration. |
| **Test data** | nimali@example |
| **Expected result** | The form is refused with 'Enter a valid email address, for example name@example.com.' |
| **Type** | Automated |
| **Automated test** | `Email_RejectsInvalidAddresses` |
| **Actual result** | |
| **Status** | |

### TC-STU-07 — A phone number that is not 10 digits is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Type a short phone number.<br>2. Click Save Registration. |
| **Test data** | 12345 |
| **Expected result** | The form is refused: the number must be 10 digits starting with 0. |
| **Type** | Automated |
| **Automated test** | `Phone_RejectsBadNumbers` |
| **Actual result** | |
| **Status** | |

### TC-STU-08 — A phone number typed with spaces or dashes is accepted

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Type the number with spaces.<br>2. Save.<br>3. Open the saved record. |
| **Test data** | 077 123 4567 |
| **Expected result** | The student is saved and the number is stored tidily as 0771234567. |
| **Type** | Automated |
| **Automated test** | `CleanPhone_StoresOneTidyForm` |
| **Actual result** | |
| **Status** | |

### TC-STU-09 — A future date of birth is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Set the date of birth to tomorrow.<br>2. Save. |
| **Test data** | Tomorrow's date |
| **Expected result** | The form is refused: 'Date of birth cannot be in the future.' |
| **Type** | Automated |
| **Automated test** | `DateOfBirth_RejectsFutureDates` |
| **Actual result** | |
| **Status** | |

### TC-STU-10 — An unlikely age is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Enter a date of birth giving an age outside 3 to 25.<br>2. Save. |
| **Test data** | 03/05/1950 |
| **Expected result** | The form is refused and asks the user to check the date. |
| **Type** | Automated |
| **Automated test** | `DateOfBirth_RejectsAnUnlikelyAge` |
| **Actual result** | |
| **Status** | |

### TC-STU-11 — An address without enough detail is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Type a very short address.<br>2. Save. |
| **Test data** | Colombo |
| **Expected result** | The form is refused and gives an example of a full address. |
| **Type** | Automated |
| **Automated test** | `Address_NeedsEnoughDetail` |
| **Actual result** | |
| **Status** | |

### TC-STU-12 — Search by student ID

| Field | Detail |
|---|---|
| **Preconditions** | Students exist. On the Student Directory. |
| **Test steps** | 1. Type a full student ID into Search Student. |
| **Test data** | STU-2026-0041 |
| **Expected result** | Only that student is listed. The list updates while typing, with no button press. |
| **Type** | Automated + Manual |
| **Automated test** | `Search_ByStudentId_FindsStudent` |
| **Actual result** | |
| **Status** | |

### TC-STU-13 — Search by part of a name, ignoring capitals

| Field | Detail |
|---|---|
| **Preconditions** | Students exist. |
| **Test steps** | 1. Type part of a name in lower case. |
| **Test data** | per |
| **Expected result** | Every student whose name contains those letters is listed. |
| **Type** | Automated |
| **Automated test** | `Search_ByPartialName_IsCaseInsensitive` |
| **Actual result** | |
| **Status** | |

### TC-STU-14 — Search by the parent's contact number

| Field | Detail |
|---|---|
| **Preconditions** | A student has a guardian phone number recorded. |
| **Test steps** | 1. Type the phone number into the search box. |
| **Test data** | 0771234567 |
| **Expected result** | The matching student is listed. |
| **Type** | Automated |
| **Automated test** | `Search_ByParentContactNumber_FindsStudent` |
| **Actual result** | |
| **Status** | |

### TC-STU-15 — A search with no matches shows an empty state

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Directory. |
| **Test steps** | 1. Type something that matches nobody. |
| **Test data** | zzzzz |
| **Expected result** | No rows are shown and the panel reads 'No students match these filters.' |
| **Type** | Automated + Manual |
| **Automated test** | `Search_NoMatch_ReturnsEmptyList` |
| **Actual result** | |
| **Status** | |

### TC-STU-16 — Filter by grade and by status

| Field | Detail |
|---|---|
| **Preconditions** | Students exist in several grades and statuses. |
| **Test steps** | 1. Pick a grade from the dropdown.<br>2. Pick one enrollment status. |
| **Test data** | 10th Grade, Active |
| **Expected result** | Only students in that grade with that status are listed. Only one status can be chosen at a time. |
| **Type** | Automated + Manual |
| **Automated test** | `Search_FiltersByGradeAndStatus` |
| **Actual result** | |
| **Status** | |

### TC-STU-17 — Edit a student and save

| Field | Detail |
|---|---|
| **Preconditions** | A student exists. |
| **Test steps** | 1. Open the student.<br>2. Click Edit.<br>3. Change a field.<br>4. Save. |
| **Test data** | New grade or phone number |
| **Expected result** | The change is saved and shown on the directory when you return. |
| **Type** | Automated + Manual |
| **Automated test** | `Update_ChangesAreSaved` |
| **Actual result** | |
| **Status** | |

### TC-STU-18 — Process a student departure

| Field | Detail |
|---|---|
| **Preconditions** | A student exists and is Active. |
| **Test steps** | 1. Open Student Departure.<br>2. Search and select the student.<br>3. Choose a reason and confirm. |
| **Test data** | Reason: Relocation |
| **Expected result** | The student's status becomes Inactive (or Dropout) and the record is kept, not deleted. |
| **Type** | Automated + Manual |
| **Automated test** | `ProcessDeparture_MarksInactiveAndKeepsRecord` |
| **Actual result** | |
| **Status** | |

### TC-STU-19 — Export the filtered student list

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Directory with filters applied. |
| **Test steps** | 1. Apply a filter.<br>2. Click Export CSV. |
| **Test data** | Grade 10 only |
| **Expected result** | A CSV file is saved and opened, containing every student the filter matches, not only the current page. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-STU-20 — Normal name formats are accepted

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Enter a name with initials or an apostrophe.<br>2. Save. |
| **Test data** | A.B. Fernando ; O'Brien |
| **Expected result** | Both are accepted. Initials, dots and apostrophes are allowed in a name. |
| **Type** | Automated |
| **Automated test** | `Name_AcceptsRealNames` |
| **Actual result** | |
| **Status** | |

### TC-STU-21 — An email is optional but must be valid if given

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Leave the student email blank and save.<br>2. Register another student with a valid email. |
| **Test data** | (blank) ; first.last+tag@mail.co.uk |
| **Expected result** | The blank email is accepted because it is optional. The valid email is also accepted. |
| **Type** | Automated |
| **Automated test** | `Email_CanBeOptional` |
| **Actual result** | |
| **Status** | |

### TC-STU-22 — A valid email is accepted

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Enter a correct email address.<br>2. Save. |
| **Test data** | nimali@example.com |
| **Expected result** | The student is saved. |
| **Type** | Automated |
| **Automated test** | `Email_AcceptsValidAddresses` |
| **Actual result** | |
| **Status** | |

### TC-STU-23 — An international phone format is accepted

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Enter the number in +94 form.<br>2. Save. |
| **Test data** | +94771234567 |
| **Expected result** | The number is accepted and stored as 0771234567. |
| **Type** | Automated |
| **Automated test** | `Phone_AcceptsSriLankanNumbers` |
| **Actual result** | |
| **Status** | |

### TC-STU-24 — A school age date of birth is accepted

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Enter a date of birth giving an age between 3 and 25.<br>2. Save. |
| **Test data** | 03/05/2010 |
| **Expected result** | The student is saved without complaint. |
| **Type** | Automated |
| **Automated test** | `DateOfBirth_AcceptsASchoolAgeStudent` |
| **Actual result** | |
| **Status** | |

### TC-STU-25 — Age is worked out correctly around the birthday

| Field | Detail |
|---|---|
| **Preconditions** | A student's date of birth is recorded. |
| **Test steps** | 1. Check the age on the day of the birthday.<br>2. Check it the day before. |
| **Test data** | Born 24/09/2010, today 24/09/2026 |
| **Expected result** | The age is 16 on the birthday and 15 the day before. |
| **Type** | Automated |
| **Automated test** | `AgeOn_CountsTheBirthdayCorrectly` |
| **Actual result** | |
| **Status** | |

### TC-STU-26 — Required fields are enforced consistently

| Field | Detail |
|---|---|
| **Preconditions** | Any form. |
| **Test steps** | 1. Leave a required box blank.<br>2. Then fill it in. |
| **Test data** | (blank) then Nimali |
| **Expected result** | The blank value is refused and the filled value is accepted, using the same rule everywhere. |
| **Type** | Automated |
| **Automated test** | `Required_RejectsEmptyValues, Required_AcceptsAValue` |
| **Actual result** | |
| **Status** | |

### TC-STU-27 — Only one problem is reported at a time

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Enter several wrong values at once.<br>2. Save. |
| **Test data** | Bad email and bad phone together |
| **Expected result** | One clear message is shown, so the user is not faced with a wall of errors. |
| **Type** | Automated |
| **Automated test** | `FirstProblem_ReportsOneMessageAtATime` |
| **Actual result** | |
| **Status** | |

### TC-STU-28 — A correctly filled form reports no problem

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration page. |
| **Test steps** | 1. Fill every field correctly.<br>2. Save. |
| **Test data** | Name, email and phone all valid |
| **Expected result** | No message is shown and the record is saved. |
| **Type** | Automated |
| **Automated test** | `FirstProblem_IsEmptyWhenEverythingIsValid` |
| **Actual result** | |
| **Status** | |

### TC-STU-29 — Changing a student ID to one already used is refused

| Field | Detail |
|---|---|
| **Preconditions** | Two students exist. |
| **Test steps** | 1. Edit one student.<br>2. Change their ID to the other student's ID.<br>3. Save. |
| **Test data** | An ID belonging to another student |
| **Expected result** | The save is refused. IDs stay unique. |
| **Type** | Automated |
| **Automated test** | `Update_ToAnotherStudentsId_Throws` |
| **Actual result** | |
| **Status** | |

### TC-STU-30 — The directory shows each student's assigned teacher

| Field | Detail |
|---|---|
| **Preconditions** | Students have an assigned teacher. |
| **Test steps** | 1. Open the Student Directory. |
| **Test data** | - |
| **Expected result** | The Assigned Teacher column shows the teacher's name and initials, not just an ID. |
| **Type** | Automated + Manual |
| **Automated test** | `GetAll_FillsAssignedTeacherName` |
| **Actual result** | |
| **Status** | |


## 3. Teacher Management

### TC-TCH-01 — Register a teacher and create their login

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as Admin. |
| **Test steps** | 1. Fill in the Teacher Registration form.<br>2. Click Register Teacher.<br>3. Log out and log in as the new teacher. |
| **Test data** | Name: K. Fernando, Email k.fernando@lumora.lk, Phone 0712345678, Username k.fernando, Password lumora1 |
| **Expected result** | A teacher ID such as TCH-2051 is created, and the teacher can sign in straight away. |
| **Type** | Automated + Manual |
| **Automated test** | `Register_CreatesTeacherAndLoginAccount` |
| **Actual result** | |
| **Status** | |

### TC-TCH-02 — A username that is already taken is refused

| Field | Detail |
|---|---|
| **Preconditions** | A teacher already uses that username. |
| **Test steps** | 1. Register another teacher with the same username. |
| **Test data** | An existing username |
| **Expected result** | The registration is refused and no half-made teacher record is left behind. |
| **Type** | Automated |
| **Automated test** | `Register_DuplicateUsername_ThrowsAndDoesNotSaveTeacher` |
| **Actual result** | |
| **Status** | |

### TC-TCH-03 — A teacher without an email is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Teacher Registration page. |
| **Test steps** | 1. Leave the email empty.<br>2. Save. |
| **Test data** | (blank email) |
| **Expected result** | The form is refused: 'Email address is required.' |
| **Type** | Automated |
| **Automated test** | `Register_MissingEmail_Throws` |
| **Actual result** | |
| **Status** | |

### TC-TCH-04 — A username that is too short is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Teacher Registration page. |
| **Test steps** | 1. Type a three letter username.<br>2. Save. |
| **Test data** | abc |
| **Expected result** | The form is refused: the username must be 4 to 20 characters. |
| **Type** | Automated |
| **Automated test** | `Username_NeedsFourToTwentyPlainCharacters` |
| **Actual result** | |
| **Status** | |

### TC-TCH-05 — A password without a number is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Teacher Registration page. |
| **Test steps** | 1. Type a password of letters only.<br>2. Save. |
| **Test data** | lumora |
| **Expected result** | The form is refused: the password needs at least one letter and one number. |
| **Type** | Automated |
| **Automated test** | `Password_NeedsALetterAndANumber` |
| **Actual result** | |
| **Status** | |

### TC-TCH-06 — A password shorter than the minimum is refused

| Field | Detail |
|---|---|
| **Preconditions** | Creating any account. |
| **Test steps** | 1. Enter a very short password. |
| **Test data** | ab1 |
| **Expected result** | The account is not created. |
| **Type** | Automated |
| **Automated test** | `CreateUser_ShortPassword_Throws` |
| **Actual result** | |
| **Status** | |

### TC-TCH-07 — Years of experience outside the allowed range is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Teacher Registration page. |
| **Test steps** | 1. Type an impossible number of years.<br>2. Save. |
| **Test data** | 99 |
| **Expected result** | The form is refused: experience must be between 0 and 60. |
| **Type** | Automated |
| **Automated test** | `WholeNumber_ChecksTheRange` |
| **Actual result** | |
| **Status** | |

### TC-TCH-08 — A joining date in the future is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Teacher Registration page. |
| **Test steps** | 1. Set the joining date to next month.<br>2. Save. |
| **Test data** | Next month |
| **Expected result** | The form is refused: the date of joining cannot be in the future. |
| **Type** | Automated |
| **Automated test** | `NotInFuture_And_NotInPast` |
| **Actual result** | |
| **Status** | |

### TC-TCH-09 — Search teachers by name, ID, department or subject

| Field | Detail |
|---|---|
| **Preconditions** | Teachers exist with departments and subjects recorded. |
| **Test steps** | 1. Type a subject name into the search box on the Teachers page. |
| **Test data** | science |
| **Expected result** | Teachers who teach that subject are listed. The list updates while typing. |
| **Type** | Automated + Manual |
| **Automated test** | `Search_FindsByNameOrId` |
| **Actual result** | |
| **Status** | |

### TC-TCH-10 — Editing a teacher also updates their login name

| Field | Detail |
|---|---|
| **Preconditions** | A teacher with a login exists. |
| **Test steps** | 1. Edit the teacher's full name.<br>2. Save.<br>3. Log in as that teacher. |
| **Test data** | A new full name |
| **Expected result** | The new name is shown in the side menu after they sign in. |
| **Type** | Automated |
| **Automated test** | `Update_SavesChangesAndSyncsLoginDisplayName` |
| **Actual result** | |
| **Status** | |

### TC-TCH-11 — Resignation requires the handover checklist

| Field | Detail |
|---|---|
| **Preconditions** | A teacher is selected on the Teacher Resignation page. |
| **Test steps** | 1. Choose a reason but leave a checklist item unticked.<br>2. Click Process. |
| **Test data** | Reason: Relocation, one item unticked |
| **Expected result** | The system refuses and asks for every handover item to be completed first. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-TCH-12 — Teacher counts and department percentages are correct

| Field | Detail |
|---|---|
| **Preconditions** | Several teachers exist across departments. |
| **Test steps** | 1. Open the Teachers page.<br>2. Compare the cards with the records. |
| **Test data** | - |
| **Expected result** | Total Teachers, On Leave and the two largest departments match the data. |
| **Type** | Automated |
| **Automated test** | `Counts_And_DepartmentPercentages_AreComputed` |
| **Actual result** | |
| **Status** | |

### TC-TCH-13 — Two accounts cannot share a username

| Field | Detail |
|---|---|
| **Preconditions** | An account already uses that username. |
| **Test steps** | 1. Try to create a second account with the same username. |
| **Test data** | An existing username |
| **Expected result** | The account is not created. |
| **Type** | Automated |
| **Automated test** | `CreateUser_DuplicateUsername_Throws` |
| **Actual result** | |
| **Status** | |


## 4. Subjects and Classes

### TC-SUB-01 — Add a subject

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as Admin, on Academics > Add New Subject. |
| **Test steps** | 1. Enter a name, a code and a curriculum.<br>2. Click Save Subject. |
| **Test data** | Combined Maths / MAT-101 / National Curriculum |
| **Expected result** | The subject is saved and appears in the list beside the form. |
| **Type** | Automated + Manual |
| **Automated test** | `AddSubject_SavesAndRejectsDuplicateCode` |
| **Actual result** | |
| **Status** | |

### TC-SUB-02 — A duplicate subject code is refused

| Field | Detail |
|---|---|
| **Preconditions** | A subject with that code already exists. |
| **Test steps** | 1. Add another subject using the same code. |
| **Test data** | MAT-101 |
| **Expected result** | The save is refused with a clear message. |
| **Type** | Automated |
| **Automated test** | `AddSubject_SavesAndRejectsDuplicateCode` |
| **Actual result** | |
| **Status** | |

### TC-SUB-03 — A subject with no name or code is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Add New Subject page. |
| **Test steps** | 1. Leave the fields empty.<br>2. Save. |
| **Test data** | (blank) |
| **Expected result** | The save is refused and names the missing field. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-CLS-01 — Create a class

| Field | Detail |
|---|---|
| **Preconditions** | At least one subject and one teacher exist. |
| **Test steps** | 1. Open Academics > Manage Classes.<br>2. Choose subject, grade and teacher.<br>3. Enter a room and a monthly fee.<br>4. Click Add Class. |
| **Test data** | Science / 10th Grade / Ms. Aries / Room 3 / 2500 |
| **Expected result** | The class is created and listed with its fee and student count. |
| **Type** | Automated + Manual |
| **Automated test** | `SeededClasses_HaveSubjectTeacherFeeAndStudentCount` |
| **Actual result** | |
| **Status** | |

### TC-CLS-02 — A class fee that is not a number is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Classes page. |
| **Test steps** | 1. Type letters into the monthly fee box.<br>2. Click Add Class. |
| **Test data** | free |
| **Expected result** | The class is not created: the fee must be a number. |
| **Type** | Automated |
| **Automated test** | `Money_ChecksTheAmount` |
| **Actual result** | |
| **Status** | |

### TC-CLS-03 — The same class cannot be created twice

| Field | Detail |
|---|---|
| **Preconditions** | A class already exists for that subject, grade and teacher. |
| **Test steps** | 1. Try to create the identical class again. |
| **Test data** | Same subject, grade and teacher |
| **Expected result** | The system refuses to create a duplicate class. |
| **Type** | Automated |
| **Automated test** | `Create_DuplicateClassForSameTeacherSubjectGrade_Throws` |
| **Actual result** | |
| **Status** | |

### TC-CLS-04 — Add a weekly time slot to a class

| Field | Detail |
|---|---|
| **Preconditions** | A class exists and is open for managing. |
| **Test steps** | 1. Choose a day, a start time and an end time.<br>2. Click Add. |
| **Test data** | Monday 16:00 to 18:00 |
| **Expected result** | The session is listed and the class now appears on that weekday in Mark Attendance. |
| **Type** | Automated + Manual |
| **Automated test** | `GetSessionsForDay_ReturnsTodaysClassesEarliestFirst` |
| **Actual result** | |
| **Status** | |

### TC-CLS-05 — A finish time before the start time is refused

| Field | Detail |
|---|---|
| **Preconditions** | Adding a weekly time slot. |
| **Test steps** | 1. Set the end time earlier than the start time.<br>2. Click Add. |
| **Test data** | Start 18:00, End 16:00 |
| **Expected result** | The session is refused with a clear message. |
| **Type** | Automated + Manual |
| **Automated test** | `AddSession_EndBeforeStart_Throws` |
| **Actual result** | |
| **Status** | |

### TC-CLS-06 — One teacher cannot be in two classes at once

| Field | Detail |
|---|---|
| **Preconditions** | The teacher already has a class at that time. |
| **Test steps** | 1. Add an overlapping session for the same teacher. |
| **Test data** | Monday 17:00 to 19:00 for a teacher already busy 16:00 to 18:00 |
| **Expected result** | The session is refused because the teacher is already teaching then. |
| **Type** | Automated |
| **Automated test** | `AddSession_ClashingWithSameTeacher_Throws` |
| **Actual result** | |
| **Status** | |

### TC-CLS-07 — Enrol a student in a class

| Field | Detail |
|---|---|
| **Preconditions** | A class and a student exist. |
| **Test steps** | 1. Open the class.<br>2. Pick the student.<br>3. Click Enrol. |
| **Test data** | Any student not yet in the class |
| **Expected result** | The student is added and the class student count goes up by one. |
| **Type** | Automated + Manual |
| **Automated test** | `Enroll_AddsStudentAndRaisesTheCount` |
| **Actual result** | |
| **Status** | |

### TC-CLS-08 — The same student cannot be enrolled twice

| Field | Detail |
|---|---|
| **Preconditions** | The student is already in the class. |
| **Test steps** | 1. Try to enrol them again. |
| **Test data** | A student already enrolled |
| **Expected result** | The system refuses. |
| **Type** | Automated |
| **Automated test** | `Enroll_SameStudentTwice_Throws` |
| **Actual result** | |
| **Status** | |

### TC-CLS-09 — Removing a student keeps the history

| Field | Detail |
|---|---|
| **Preconditions** | A student is enrolled in a class. |
| **Test steps** | 1. Remove the student from the class.<br>2. Check the attendance and fee history. |
| **Test data** | Any enrolled student |
| **Expected result** | The student no longer appears in the class, but their earlier attendance and payments are still there. |
| **Type** | Automated |
| **Automated test** | `Unenroll_RemovesFromClassButKeepsHistory` |
| **Actual result** | |
| **Status** | |

### TC-CLS-10 — Closing a class keeps its records

| Field | Detail |
|---|---|
| **Preconditions** | A class with students exists. |
| **Test steps** | 1. Click Close Class and confirm. |
| **Test data** | Any class |
| **Expected result** | The class is closed and its students are taken out, but the attendance and payment history is kept. |
| **Type** | Automated + Manual |
| **Automated test** | `Deactivate_ClosesTheClassAndItsEnrolments` |
| **Actual result** | |
| **Status** | |

### TC-CLS-11 — A class lists only its own students

| Field | Detail |
|---|---|
| **Preconditions** | Two classes with different students exist. |
| **Test steps** | 1. Open one class. |
| **Test data** | - |
| **Expected result** | Only the students enrolled in that class are listed, not every student in the grade. |
| **Type** | Automated + Manual |
| **Automated test** | `GetStudents_ReturnsOnlyTheStudentsEnrolledInThatClass` |
| **Actual result** | |
| **Status** | |

### TC-CLS-12 — A student's record lists every class they attend

| Field | Detail |
|---|---|
| **Preconditions** | A student attends more than one class. |
| **Test steps** | 1. Open the student's record. |
| **Test data** | - |
| **Expected result** | All of their classes are listed. |
| **Type** | Automated + Manual |
| **Automated test** | `GetClassesForStudent_ListsEveryClassTheyAttend` |
| **Actual result** | |
| **Status** | |

### TC-CLS-13 — A student who returns is put back in the same record

| Field | Detail |
|---|---|
| **Preconditions** | A student left a class and is enrolled again. |
| **Test steps** | 1. Remove the student from a class.<br>2. Enrol them again. |
| **Test data** | - |
| **Expected result** | The original enrolment record is reused, so the class does not build up duplicate rows. |
| **Type** | Automated |
| **Automated test** | `Enroll_AfterLeaving_ReusesTheSameRow` |
| **Actual result** | |
| **Status** | |


## 5. Attendance

### TC-ATT-01 — Today's classes are listed

| Field | Detail |
|---|---|
| **Preconditions** | Classes with weekly time slots exist. Logged in as Admin. |
| **Test steps** | 1. Click Attendance in the side menu. |
| **Test data** | Today's date |
| **Expected result** | Mark Attendance opens showing each class for today with its time, class name, teacher, room, student count and whether it has been marked. |
| **Type** | Automated + Manual |
| **Automated test** | `GetSessionsForDay_ReturnsTodaysClassesEarliestFirst` |
| **Actual result** | |
| **Status** | |

### TC-ATT-02 — Nothing is marked by default

| Field | Detail |
|---|---|
| **Preconditions** | A class is scheduled today and has not been marked. |
| **Test steps** | 1. Click the class to open the student sheet. |
| **Test data** | Any class |
| **Expected result** | Every student appears with no status chosen. No student defaults to Present. |
| **Type** | Automated + Manual |
| **Automated test** | `GetSheet_StartsWithEveryStudentUnmarked` |
| **Actual result** | |
| **Status** | |

### TC-ATT-03 — Mark a student present, absent or late

| Field | Detail |
|---|---|
| **Preconditions** | The student sheet is open. |
| **Test steps** | 1. Click Present on one row.<br>2. Click Absent on another.<br>3. Click Late on a third. |
| **Test data** | Three different students |
| **Expected result** | Each button turns its own colour and the counts at the top update immediately. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-ATT-04 — Clear a mark

| Field | Detail |
|---|---|
| **Preconditions** | A student has been marked. |
| **Test steps** | 1. Click Clear on that row. |
| **Test data** | Any marked student |
| **Expected result** | The row goes back to unmarked and the Not Marked count goes up. |
| **Type** | Automated |
| **Automated test** | `SaveSheet_ClearingAStatusRemovesTheEarlierMark` |
| **Actual result** | |
| **Status** | |

### TC-ATT-05 — Mark all present

| Field | Detail |
|---|---|
| **Preconditions** | The student sheet is open. |
| **Test steps** | 1. Click Mark all present. |
| **Test data** | - |
| **Expected result** | Every row is set to Present and the Present count equals the number of students. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-ATT-06 — Mark the rest absent

| Field | Detail |
|---|---|
| **Preconditions** | Some students are marked present, others are blank. |
| **Test steps** | 1. Click Mark rest absent. |
| **Test data** | - |
| **Expected result** | Only the blank rows become Absent. Rows already marked are not changed. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-ATT-07 — Blank rows are not saved as absent

| Field | Detail |
|---|---|
| **Preconditions** | Some students are left unmarked. |
| **Test steps** | 1. Mark only a few students.<br>2. Click Save Attendance.<br>3. Open the history. |
| **Test data** | 2 of 5 students marked |
| **Expected result** | Only the marked students are stored. The unmarked ones are counted as Not Recorded, which is not the same as Absent. |
| **Type** | Automated + Manual |
| **Automated test** | `SaveSheet_DoesNotSaveRowsLeftBlank` |
| **Actual result** | |
| **Status** | |

### TC-ATT-08 — Saving reports how many were marked

| Field | Detail |
|---|---|
| **Preconditions** | The student sheet has some marks. |
| **Test steps** | 1. Click Save Attendance. |
| **Test data** | - |
| **Expected result** | A message says how many of the class were marked, and the class now shows Marked in the list. |
| **Type** | Automated + Manual |
| **Automated test** | `IsMarked_TellsWhetherTheClassWasDoneThatDay` |
| **Actual result** | |
| **Status** | |

### TC-ATT-09 — Reopening a marked class shows the earlier marks

| Field | Detail |
|---|---|
| **Preconditions** | A class was marked and saved today. |
| **Test steps** | 1. Open the same class again. |
| **Test data** | - |
| **Expected result** | The marks that were saved are shown, not a blank sheet. |
| **Type** | Automated |
| **Automated test** | `GetSheet_ShowsWhatWasSavedEarlier` |
| **Actual result** | |
| **Status** | |

### TC-ATT-10 — Marking the same day twice does not duplicate rows

| Field | Detail |
|---|---|
| **Preconditions** | A class was marked earlier today. |
| **Test steps** | 1. Change a mark and save again.<br>2. Check the history. |
| **Test data** | - |
| **Expected result** | The earlier mark is updated. There is one row per student per day, not two. |
| **Type** | Automated |
| **Automated test** | `SaveSheet_MarkingTwiceUpdatesInsteadOfDuplicating` |
| **Actual result** | |
| **Status** | |

### TC-ATT-11 — An invalid status is rejected

| Field | Detail |
|---|---|
| **Preconditions** | Calling the attendance service directly. |
| **Test steps** | 1. Try to save a status that is not Present, Absent or Late. |
| **Test data** | Holiday |
| **Expected result** | The system refuses the value. |
| **Type** | Automated |
| **Automated test** | `Mark_InvalidStatus_Throws` |
| **Actual result** | |
| **Status** | |

### TC-ATT-12 — The history summary counts correctly

| Field | Detail |
|---|---|
| **Preconditions** | Attendance has been marked for a class. |
| **Test steps** | 1. Open Attendance > View History. |
| **Test data** | - |
| **Expected result** | Each row shows Enrolled, Present, Absent, Late and Not recorded, and the numbers add up. |
| **Type** | Automated + Manual |
| **Automated test** | `Summary_CountsMarkedStudentsAndShowsHowManyAreNotRecorded` |
| **Actual result** | |
| **Status** | |

### TC-ATT-13 — Perfect status when everyone marked is present

| Field | Detail |
|---|---|
| **Preconditions** | Every marked student in a class was present. |
| **Test steps** | 1. Look at that day's row in the history. |
| **Test data** | - |
| **Expected result** | The status badge reads Perfect. |
| **Type** | Automated |
| **Automated test** | `Summary_Status_IsPerfectWhenEveryMarkedStudentIsPresent` |
| **Actual result** | |
| **Status** | |

### TC-ATT-14 — History can be filtered by class and by month

| Field | Detail |
|---|---|
| **Preconditions** | Attendance exists for more than one class and month. |
| **Test steps** | 1. Choose a class from the dropdown.<br>2. Choose a month.<br>3. Click Clear. |
| **Test data** | Science - G10, September 2026 |
| **Expected result** | Only matching rows are shown. Clear puts every row back. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-ATT-15 — A late student counts towards the attendance rate

| Field | Detail |
|---|---|
| **Preconditions** | A class has present, late and absent students. |
| **Test steps** | 1. Check the attendance rate. |
| **Test data** | - |
| **Expected result** | Late is counted as attending, not as absent. |
| **Type** | Automated |
| **Automated test** | `AttendanceRate_CountsLateAsPresent` |
| **Actual result** | |
| **Status** | |

### TC-ATT-16 — Overall attendance statistics are produced

| Field | Detail |
|---|---|
| **Preconditions** | Attendance has been marked over several days. |
| **Test steps** | 1. Open the attendance summary cards. |
| **Test data** | - |
| **Expected result** | The average rate, the number of days logged and the perfect days are shown and match the records. |
| **Type** | Automated + Manual |
| **Automated test** | `GetStats_ReturnsSeededNumbers` |
| **Actual result** | |
| **Status** | |


## 6. Payments and Fees

### TC-PAY-01 — The fee register lists one row per enrolled student

| Field | Detail |
|---|---|
| **Preconditions** | A class has enrolled students and a monthly fee. |
| **Test steps** | 1. Open Payments.<br>2. Choose a month.<br>3. Click the class. |
| **Test data** | Science - G10, this month |
| **Expected result** | Every enrolled student has a row, with the amount due set to the class fee. |
| **Type** | Automated + Manual |
| **Automated test** | `GetRegister_HasOneRowPerEnrolledStudentWithTheClassFee` |
| **Actual result** | |
| **Status** | |

### TC-PAY-02 — Register rows are created once only

| Field | Detail |
|---|---|
| **Preconditions** | The register for a class and month has already been opened. |
| **Test steps** | 1. Open the same class and month again. |
| **Test data** | - |
| **Expected result** | The same rows are shown. No duplicates are created. |
| **Type** | Automated |
| **Automated test** | `GetRegister_CreatesMissingRowsOnceOnly` |
| **Actual result** | |
| **Status** | |

### TC-PAY-03 — Mark a student as paid

| Field | Detail |
|---|---|
| **Preconditions** | A student has an unpaid row. |
| **Test steps** | 1. Click Mark Paid on that row. |
| **Test data** | - |
| **Expected result** | The full fee is recorded, today's date is set, a receipt number starting RCP- is created and the status becomes Paid. |
| **Type** | Automated + Manual |
| **Automated test** | `MarkPaid_SetsTheFullFeeTodayAndGivesAReceiptNumber` |
| **Actual result** | |
| **Status** | |

### TC-PAY-04 — Undo a payment marked by mistake

| Field | Detail |
|---|---|
| **Preconditions** | A student is marked Paid. |
| **Test steps** | 1. Click Undo on that row. |
| **Test data** | - |
| **Expected result** | The row goes back to unpaid and the collected total drops accordingly. |
| **Type** | Automated + Manual |
| **Automated test** | `MarkUnpaid_UndoesIt` |
| **Actual result** | |
| **Status** | |

### TC-PAY-05 — Record a part payment

| Field | Detail |
|---|---|
| **Preconditions** | A student has an unpaid row. |
| **Test steps** | 1. Click the ... button.<br>2. Enter an amount smaller than the fee.<br>3. Save. |
| **Test data** | Fee 2500, paid 1000 |
| **Expected result** | The status becomes Partial and the balance still owed is shown. |
| **Type** | Automated + Manual |
| **Automated test** | `RecordPayment_PartAmountLeavesABalance` |
| **Actual result** | |
| **Status** | |

### TC-PAY-06 — A payment needs an amount and a date

| Field | Detail |
|---|---|
| **Preconditions** | Recording a payment. |
| **Test steps** | 1. Leave the amount or date empty. |
| **Test data** | (blank) |
| **Expected result** | The payment is refused. |
| **Type** | Automated |
| **Automated test** | `RecordPayment_AmountAndDateAreMandatory` |
| **Actual result** | |
| **Status** | |

### TC-PAY-07 — Status is Pending before the due date

| Field | Detail |
|---|---|
| **Preconditions** | An unpaid fee whose due date has not passed. |
| **Test steps** | 1. Look at the status. |
| **Test data** | Due on the 10th, today is earlier |
| **Expected result** | The status reads Pending. |
| **Type** | Automated |
| **Automated test** | `Status_Pending_BeforeTheDueDate` |
| **Actual result** | |
| **Status** | |

### TC-PAY-08 — Status is Overdue after the due date

| Field | Detail |
|---|---|
| **Preconditions** | An unpaid fee whose due date has passed. |
| **Test steps** | 1. Look at the status. |
| **Test data** | Due on the 10th, today is later |
| **Expected result** | The status reads Overdue. |
| **Type** | Automated |
| **Automated test** | `Status_Overdue_AfterTheDueDate` |
| **Actual result** | |
| **Status** | |

### TC-PAY-09 — The due date is the 10th of the month

| Field | Detail |
|---|---|
| **Preconditions** | Any month's register. |
| **Test steps** | 1. Check the due date used for the status. |
| **Test data** | 2026-09 |
| **Expected result** | The due date is 10 September 2026. |
| **Type** | Automated |
| **Automated test** | `DueDateFor_IsTheTenthOfTheMonth` |
| **Actual result** | |
| **Status** | |

### TC-PAY-10 — Grant an extension of one to three months

| Field | Detail |
|---|---|
| **Preconditions** | A student cannot pay on time. |
| **Test steps** | 1. Click the ... button.<br>2. Grant an extension of 2 months. |
| **Test data** | 2 months |
| **Expected result** | The deadline moves and the status reads Extension Granted. |
| **Type** | Automated + Manual |
| **Automated test** | `GrantExtension_MovesTheDeadlineBy1To3Months` |
| **Actual result** | |
| **Status** | |

### TC-PAY-11 — An extension longer than three months is refused

| Field | Detail |
|---|---|
| **Preconditions** | Granting an extension. |
| **Test steps** | 1. Try to grant four months. |
| **Test data** | 4 months |
| **Expected result** | The extension is refused. |
| **Type** | Automated |
| **Automated test** | `GrantExtension_MovesTheDeadlineBy1To3Months` |
| **Actual result** | |
| **Status** | |

### TC-PAY-12 — Outstanding fees add up across every class

| Field | Detail |
|---|---|
| **Preconditions** | A student attends more than one class and owes on both. |
| **Test steps** | 1. Open the student's record. |
| **Test data** | - |
| **Expected result** | The amount outstanding is the total across all their classes. |
| **Type** | Automated |
| **Automated test** | `OutstandingForStudent_AddsUpEveryClassTheyTake` |
| **Actual result** | |
| **Status** | |

### TC-PAY-13 — The month totals are correct

| Field | Detail |
|---|---|
| **Preconditions** | A month's register has paid and unpaid students. |
| **Test steps** | 1. Look at the four cards at the top of Payments. |
| **Test data** | - |
| **Expected result** | Total Expected, Collected, Outstanding and Overdue match the register underneath. |
| **Type** | Automated + Manual |
| **Automated test** | `Summary_AddsUpThisMonthsRegister` |
| **Actual result** | |
| **Status** | |

### TC-PAY-14 — The outstanding list shows only students who still owe

| Field | Detail |
|---|---|
| **Preconditions** | Some students have paid in full. |
| **Test steps** | 1. Open the outstanding list. |
| **Test data** | - |
| **Expected result** | Students who have paid in full are not listed. |
| **Type** | Automated |
| **Automated test** | `GetOutstanding_ListsOnlyStudentsWhoStillOweMoney` |
| **Actual result** | |
| **Status** | |

### TC-PAY-15 — Only the relevant button is shown on each row

| Field | Detail |
|---|---|
| **Preconditions** | A register with paid and unpaid students. |
| **Test steps** | 1. Compare a paid row with an unpaid row. |
| **Test data** | - |
| **Expected result** | Unpaid rows show Mark Paid. Paid rows show Undo. Both never appear together. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-PAY-16 — Recent payments appear on the dashboard

| Field | Detail |
|---|---|
| **Preconditions** | A payment has just been taken. |
| **Test steps** | 1. Mark a student paid.<br>2. Open the Admin Dashboard. |
| **Test data** | - |
| **Expected result** | The payment is listed under Recent Activity, newest first. |
| **Type** | Automated + Manual |
| **Automated test** | `GetRecentlyPaid_ReturnsOnlyPaidRowsNewestFirst` |
| **Actual result** | |
| **Status** | |

### TC-PAY-17 — Status is Paid once the full fee is in

| Field | Detail |
|---|---|
| **Preconditions** | A student has paid the whole fee. |
| **Test steps** | 1. Look at the status. |
| **Test data** | Fee 2500, paid 2500 |
| **Expected result** | The status reads Paid and nothing is outstanding. |
| **Type** | Automated |
| **Automated test** | `Status_Paid_WhenTheFullFeeIsIn` |
| **Actual result** | |
| **Status** | |

### TC-PAY-18 — Status is Partial when only some money is in

| Field | Detail |
|---|---|
| **Preconditions** | A student has paid part of the fee. |
| **Test steps** | 1. Look at the status. |
| **Test data** | Fee 2500, paid 1000 |
| **Expected result** | The status reads Partial. |
| **Type** | Automated |
| **Automated test** | `Status_Partial_WhenSomeMoneyIsIn` |
| **Actual result** | |
| **Status** | |

### TC-PAY-19 — Status stays Extension Granted while the extension lasts

| Field | Detail |
|---|---|
| **Preconditions** | A student has been given an extension. |
| **Test steps** | 1. Look at the status before the new deadline. |
| **Test data** | Extension of 2 months |
| **Expected result** | The status reads Extension Granted rather than Overdue. |
| **Type** | Automated |
| **Automated test** | `Status_ExtensionGranted_WhileTheExtensionLasts` |
| **Actual result** | |
| **Status** | |

### TC-PAY-20 — The register shows both paid and unpaid students

| Field | Detail |
|---|---|
| **Preconditions** | A class has a mix of paid and unpaid students. |
| **Test steps** | 1. Open the register for that class. |
| **Test data** | - |
| **Expected result** | Paid students show Paid and the others show an unpaid status, in the same list. |
| **Type** | Automated + Manual |
| **Automated test** | `GetRegister_ShowsTheSeededPaidAndUnpaidStudents` |
| **Actual result** | |
| **Status** | |

### TC-PAY-21 — The recent payments list is limited in length

| Field | Detail |
|---|---|
| **Preconditions** | Many payments have been taken. |
| **Test steps** | 1. Ask for the two most recent payments. |
| **Test data** | count = 2 |
| **Expected result** | No more than two are returned, so the dashboard stays short. |
| **Type** | Automated |
| **Automated test** | `GetRecentlyPaid_RespectsTheCount` |
| **Actual result** | |
| **Status** | |


## 7. Academic Performance

### TC-ACA-01 — Record a monthly, term or year end mark

| Field | Detail |
|---|---|
| **Preconditions** | A student and a subject exist. |
| **Test steps** | 1. Open the student.<br>2. Click Enter Marks.<br>3. Choose subject and exam type, enter a mark.<br>4. Save. |
| **Test data** | Science, Term, 78 |
| **Expected result** | The mark is saved with the matching letter grade and shown in the student's record. |
| **Type** | Automated + Manual |
| **Automated test** | `RecordMark_MonthlyTermAndYearEnd_AreSavedWithGrades` |
| **Actual result** | |
| **Status** | |

### TC-ACA-02 — A mark above 100 or below 0 is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Enter Marks page. |
| **Test steps** | 1. Type 101 as the mark.<br>2. Save. |
| **Test data** | 101 |
| **Expected result** | The mark is refused: it must be between 0 and 100. |
| **Type** | Automated + Manual |
| **Automated test** | `Marks_AreOutOfOneHundred` |
| **Actual result** | |
| **Status** | |

### TC-ACA-03 — An invalid exam type is refused

| Field | Detail |
|---|---|
| **Preconditions** | Recording a mark. |
| **Test steps** | 1. Use an exam type the system does not know. |
| **Test data** | Pop quiz |
| **Expected result** | The mark is refused. |
| **Type** | Automated |
| **Automated test** | `RecordMark_OutOfRangeOrWrongType_Throws` |
| **Actual result** | |
| **Status** | |

### TC-ACA-04 — Record a Cambridge result

| Field | Detail |
|---|---|
| **Preconditions** | A student and a subject exist. |
| **Test steps** | 1. Enter a Cambridge exam name and grade.<br>2. Save. |
| **Test data** | IGCSE Maths, A |
| **Expected result** | The result is saved and listed in the student's record. |
| **Type** | Automated + Manual |
| **Automated test** | `RecordCambridgeResult_SavesGradeAndAppearsInStudentRecord` |
| **Actual result** | |
| **Status** | |

### TC-ACA-05 — Cambridge results do not change the internal average

| Field | Detail |
|---|---|
| **Preconditions** | A student has both internal marks and a Cambridge result. |
| **Test steps** | 1. Compare the average before and after adding the Cambridge result. |
| **Test data** | - |
| **Expected result** | The internal average is unchanged. |
| **Type** | Automated |
| **Automated test** | `CambridgeResults_DoNotAffectInternalAverage` |
| **Actual result** | |
| **Status** | |

### TC-ACA-06 — Marks map to the right letter grade

| Field | Detail |
|---|---|
| **Preconditions** | Marks have been recorded. |
| **Test steps** | 1. Check the grade shown beside several marks. |
| **Test data** | 92, 78, 55, 30 |
| **Expected result** | Each mark shows the correct letter grade. |
| **Type** | Automated |
| **Automated test** | `GradeFor_MapsMarksToLetters` |
| **Actual result** | |
| **Status** | |

### TC-ACA-07 — At-risk students below 60 percent are found

| Field | Detail |
|---|---|
| **Preconditions** | Students with low marks exist. |
| **Test steps** | 1. Open At-Risk Students.<br>2. Leave the threshold at Below 60%. |
| **Test data** | - |
| **Expected result** | Only students averaging under 60 percent are listed. |
| **Type** | Automated + Manual |
| **Automated test** | `GetAtRiskStudents_FindsStudentsBelow60Percent` |
| **Actual result** | |
| **Status** | |

### TC-ACA-08 — A higher threshold includes more students

| Field | Detail |
|---|---|
| **Preconditions** | On the At-Risk Students page. |
| **Test steps** | 1. Change the threshold to Below 80%. |
| **Test data** | Below 80% |
| **Expected result** | More students are listed than at Below 60%. |
| **Type** | Automated + Manual |
| **Automated test** | `GetAtRiskStudents_HigherThreshold_IncludesMoreStudents` |
| **Actual result** | |
| **Status** | |

### TC-ACA-09 — At-risk filtering by subject

| Field | Detail |
|---|---|
| **Preconditions** | At-risk students exist in several subjects. |
| **Test steps** | 1. Choose one subject from the dropdown. |
| **Test data** | Science |
| **Expected result** | Only students weak in that subject are listed, and the list updates as soon as the subject is chosen. |
| **Type** | Automated + Manual |
| **Automated test** | `GetAtRiskStudents_FilterBySubject` |
| **Actual result** | |
| **Status** | |

### TC-ACA-10 — A progress note is saved against the student

| Field | Detail |
|---|---|
| **Preconditions** | A student is open. |
| **Test steps** | 1. Write a progress note recommending extra classes.<br>2. Save. |
| **Test data** | Any note text |
| **Expected result** | The note is stored and shown on the student's record. |
| **Type** | Automated |
| **Automated test** | `ProgressNote_WithExtraClassRecommendation_IsSaved` |
| **Actual result** | |
| **Status** | |

### TC-ACA-11 — At-risk students show their weak subjects and their year group

| Field | Detail |
|---|---|
| **Preconditions** | At-risk students exist. |
| **Test steps** | 1. Open At-Risk Students. |
| **Test data** | - |
| **Expected result** | Each row names the subjects the student is weak in and the grade they are in. |
| **Type** | Automated + Manual |
| **Automated test** | `GetAtRiskStudents_ListsWeakSubjectsAndCohort` |
| **Actual result** | |
| **Status** | |


## 8. Events

### TC-EVT-01 — Create an event

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as Admin, on Upcoming Events. |
| **Test steps** | 1. Enter a title, category, date and location.<br>2. Click Create. |
| **Test data** | Parents' Meeting, Academic, next Friday, Hall |
| **Expected result** | The event is saved and listed. |
| **Type** | Automated + Manual |
| **Automated test** | `Create_Update_Delete_Event` |
| **Actual result** | |
| **Status** | |

### TC-EVT-02 — An event without a title is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Upcoming Events page. |
| **Test steps** | 1. Leave the title empty.<br>2. Click Create. |
| **Test data** | (blank) |
| **Expected result** | The event is refused: 'Event title is required.' |
| **Type** | Automated + Manual |
| **Automated test** | `Create_WithoutTitle_Throws` |
| **Actual result** | |
| **Status** | |

### TC-EVT-03 — An event dated in the past is refused

| Field | Detail |
|---|---|
| **Preconditions** | On the Upcoming Events page. |
| **Test steps** | 1. Set the date to yesterday.<br>2. Click Create. |
| **Test data** | Yesterday |
| **Expected result** | The event is refused: the event date cannot be in the past. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-EVT-04 — Edit and delete an event

| Field | Detail |
|---|---|
| **Preconditions** | An event exists. |
| **Test steps** | 1. Click the pencil, change a field, save.<br>2. Click the bin and confirm. |
| **Test data** | - |
| **Expected result** | The change is shown in the list, and the event disappears after confirming the delete. |
| **Type** | Automated + Manual |
| **Automated test** | `Create_Update_Delete_Event` |
| **Actual result** | |
| **Status** | |

### TC-EVT-05 — Only future events are shown as upcoming

| Field | Detail |
|---|---|
| **Preconditions** | Past and future events exist. |
| **Test steps** | 1. Open the upcoming list. |
| **Test data** | - |
| **Expected result** | Only future events are listed, soonest first. |
| **Type** | Automated |
| **Automated test** | `GetUpcoming_ReturnsOnlyFutureEventsSoonestFirst` |
| **Actual result** | |
| **Status** | |


## 9. Reports

### TC-RPT-01 — Open a student progress report

| Field | Detail |
|---|---|
| **Preconditions** | A student with marks and attendance exists. |
| **Test steps** | 1. Open the student.<br>2. Click Print Report. |
| **Test data** | - |
| **Expected result** | The report shows the student's details, one row per subject, the total, the GPA and the attendance. |
| **Type** | Automated + Manual |
| **Automated test** | `StudentReport_MatchesTheDesignedReportCard` |
| **Actual result** | |
| **Status** | |

### TC-RPT-02 — A report for a student with no marks is empty, not an error

| Field | Detail |
|---|---|
| **Preconditions** | A newly registered student with no marks. |
| **Test steps** | 1. Open their report. |
| **Test data** | - |
| **Expected result** | The report opens with empty rows. The system does not crash. |
| **Type** | Automated |
| **Automated test** | `StudentReport_ForStudentWithNoMarks_IsEmptyNotError` |
| **Actual result** | |
| **Status** | |

### TC-RPT-03 — Generate the report as a PDF

| Field | Detail |
|---|---|
| **Preconditions** | A student report is open. |
| **Test steps** | 1. Click Print. |
| **Test data** | - |
| **Expected result** | A PDF file is created and opened in the default viewer, ready to print. |
| **Type** | Automated + Manual |
| **Automated test** | `Save_WritesAValidPdfFile` |
| **Actual result** | |
| **Status** | |

### TC-RPT-04 — A PDF can be made for a student with no marks

| Field | Detail |
|---|---|
| **Preconditions** | A student with no marks. |
| **Test steps** | 1. Generate their PDF. |
| **Test data** | - |
| **Expected result** | A valid PDF is produced without error. |
| **Type** | Automated |
| **Automated test** | `ToBytes_WorksForStudentWithNoMarks` |
| **Actual result** | |
| **Status** | |

### TC-RPT-05 — The payment report can be filtered by status

| Field | Detail |
|---|---|
| **Preconditions** | Payments exist in several statuses. |
| **Test steps** | 1. Build the payment report.<br>2. Filter by Overdue. |
| **Test data** | Overdue |
| **Expected result** | Only overdue rows are listed. |
| **Type** | Automated |
| **Automated test** | `PaymentReport_ListsEveryStudentWithFees_AndCanFilterByStatus` |
| **Actual result** | |
| **Status** | |

### TC-RPT-06 — The academic report can be filtered by grade

| Field | Detail |
|---|---|
| **Preconditions** | Students exist in several grades. |
| **Test steps** | 1. Build the academic report for one grade. |
| **Test data** | 10th Grade |
| **Expected result** | Only students in that grade are listed. |
| **Type** | Automated |
| **Automated test** | `AcademicReport_CanFilterByGrade` |
| **Actual result** | |
| **Status** | |

### TC-RPT-07 — Subject averages match the marks entered

| Field | Detail |
|---|---|
| **Preconditions** | Marks exist for several subjects. |
| **Test steps** | 1. Compare the dashboard bar chart with the recorded marks. |
| **Test data** | - |
| **Expected result** | There is one bar per subject that has marks, and each height matches the average. |
| **Type** | Automated + Manual |
| **Automated test** | `AverageBySubject_HasOneEntryPerSubjectWithMarks` |
| **Actual result** | |
| **Status** | |

### TC-RPT-08 — The GPA is worked out correctly

| Field | Detail |
|---|---|
| **Preconditions** | A student has marks. |
| **Test steps** | 1. Check the GPA on their report. |
| **Test data** | 92, 78, 55 |
| **Expected result** | The GPA matches the four point scale. |
| **Type** | Automated |
| **Automated test** | `GpaFor_MapsPercentToFourPointScale` |
| **Actual result** | |
| **Status** | |


## 10. User Interface

### TC-UI-01 — Every dropdown can be recognised

| Field | Detail |
|---|---|
| **Preconditions** | Any page with a dropdown. |
| **Test steps** | 1. Look at a dropdown.<br>2. Hover over it. |
| **Test data** | - |
| **Expected result** | An arrow is shown on the right. On hover the border and arrow change to the brand colour. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-02 — A date can be typed as well as picked

| Field | Detail |
|---|---|
| **Preconditions** | On any form with a date, for example Date of Birth. |
| **Test steps** | 1. Type the date straight into the box.<br>2. Then click the calendar icon. |
| **Test data** | 25/09/2010 |
| **Expected result** | The typed date is accepted, and the calendar also works. An impossible date turns the text red. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-03 — Typed text stays readable

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Registration form. |
| **Test steps** | 1. Type into every field. |
| **Test data** | Any text |
| **Expected result** | What is typed is fully visible and is not cut off by the box. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-04 — Buttons react to the mouse

| Field | Detail |
|---|---|
| **Preconditions** | Any page with buttons. |
| **Test steps** | 1. Hover over a button.<br>2. Press and hold it. |
| **Test data** | - |
| **Expected result** | The colour changes on hover and again while pressed. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-05 — Table rows show they can be clicked

| Field | Detail |
|---|---|
| **Preconditions** | On the Student Directory. |
| **Test steps** | 1. Move the mouse down the rows. |
| **Test data** | - |
| **Expected result** | The row under the mouse is highlighted. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-06 — The side menu shows where you are

| Field | Detail |
|---|---|
| **Preconditions** | Logged in. |
| **Test steps** | 1. Open each menu item in turn. |
| **Test data** | - |
| **Expected result** | The current page is a cream pill with a gold bar. Hovering another item lights it up. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-07 — Attendance opens the marking screen directly

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as Admin. |
| **Test steps** | 1. Click Attendance in the side menu. |
| **Test data** | - |
| **Expected result** | Mark Attendance opens. View History is one click away at the top right. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-08 — Long lists are split into pages

| Field | Detail |
|---|---|
| **Preconditions** | More students exist than fit on one page. |
| **Test steps** | 1. Look at the bottom of the Student Directory.<br>2. Click page 2. |
| **Test data** | - |
| **Expected result** | The page numbers work and the count line matches what is shown. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-09 — A photo can be uploaded and removed

| Field | Detail |
|---|---|
| **Preconditions** | On the Student or Teacher Registration form. |
| **Test steps** | 1. Click the photo box and choose a JPG.<br>2. Click Remove photo. |
| **Test data** | A JPG under 2 MB |
| **Expected result** | The photo is shown, and removing it puts the upload box back. A file over 2 MB is refused. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-UI-10 — The system uses one colour scheme throughout

| Field | Detail |
|---|---|
| **Preconditions** | Any page. |
| **Test steps** | 1. Move through every screen. |
| **Test data** | - |
| **Expected result** | The same brown palette, borders and card style are used everywhere, with the institute logo on the side menu, Home, Login and the report. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |


## 11. Non-functional

### TC-NFR-01 — Data survives closing the app

| Field | Detail |
|---|---|
| **Preconditions** | Records have been added. |
| **Test steps** | 1. Add a student.<br>2. Close the app completely.<br>3. Open it again. |
| **Test data** | - |
| **Expected result** | The student is still there. |
| **Type** | Automated + Manual |
| **Automated test** | `DataSurvivesReopen` |
| **Actual result** | |
| **Status** | |

### TC-NFR-02 — The sample data is only created once

| Field | Detail |
|---|---|
| **Preconditions** | A database file already exists. |
| **Test steps** | 1. Close and reopen the app twice. |
| **Test data** | - |
| **Expected result** | The demo data is not added again and no duplicates appear. |
| **Type** | Automated |
| **Automated test** | `FreshDatabase_IsSeededOnce_AndNotAgainOnReopen` |
| **Actual result** | |
| **Status** | |

### TC-NFR-03 — The system works with no internet

| Field | Detail |
|---|---|
| **Preconditions** | The computer is offline. |
| **Test steps** | 1. Turn off the network.<br>2. Use the system normally. |
| **Test data** | - |
| **Expected result** | Everything works, because the data is held in a local SQLite file on the office computer. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-NFR-04 — The database can be backed up

| Field | Detail |
|---|---|
| **Preconditions** | The system has data. |
| **Test steps** | 1. Run the backup. |
| **Test data** | - |
| **Expected result** | A copy of the database file is created. |
| **Type** | Automated |
| **Automated test** | `Backup_CreatesACopyOfTheDatabaseFile` |
| **Actual result** | |
| **Status** | |

### TC-NFR-05 — A teacher cannot reach admin-only screens

| Field | Detail |
|---|---|
| **Preconditions** | Logged in as a teacher. |
| **Test steps** | 1. Look at the side menu. |
| **Test data** | - |
| **Expected result** | Payments, Teachers and Academics are not offered to a teacher. |
| **Type** | Manual |
| **Automated test** | — |
| **Actual result** | |
| **Status** | |

### TC-NFR-06 — Changing a password stops the old one working

| Field | Detail |
|---|---|
| **Preconditions** | An account exists. |
| **Test steps** | 1. Change the password.<br>2. Try the old password. |
| **Test data** | - |
| **Expected result** | The old password is refused and the new one works. |
| **Type** | Automated |
| **Automated test** | `ChangePassword_OldPasswordStopsWorking` |
| **Actual result** | |
| **Status** | |

### TC-NFR-07 — All data can be cleared

| Field | Detail |
|---|---|
| **Preconditions** | The system has data. |
| **Test steps** | 1. Run the clear-all operation. |
| **Test data** | - |
| **Expected result** | Every table is emptied, which is used when handing a clean copy to another member. |
| **Type** | Automated |
| **Automated test** | `ClearAll_EmptiesEveryTable` |
| **Actual result** | |
| **Status** | |
