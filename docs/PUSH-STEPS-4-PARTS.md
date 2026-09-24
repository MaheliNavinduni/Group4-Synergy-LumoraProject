# Pushing this sprint's work to GitHub, in 4 parts

Repository: `MaheliNavinduni/Group4-Synergy-LumoraProject`
All 4 parts go into the **Dev** branch, **in order: 1, then 2, then 3, then 4.**

The work is already committed locally as 27 commits. It has been split into 4
branches, one per part. Nothing has been pushed yet.

| Part | Branch | Commits | What it covers |
|---|---|---|---|
| 1 | `part1/classes-attendance-payments` | 8 | Classes and enrolments, attendance rebuilt, payments as a cash fee register |
| 2 | `part2/validation-and-forms` | 6 | Input validation, the new dropdown and date controls, both registration forms |
| 3 | `part3/filters-and-ux` | 5 | Search and filters that work, hover effects, real charts, page widths |
| 4 | `part4/branding-and-testcases` | 8 | Brown palette, institute logo, home page, login, the test case list |

Each branch was built and tested on its own before being handed over:

| Part | Build errors | Tests |
|---|---|---|
| 1 | 0 | 97 passed |
| 2 | 0 | 141 passed |
| 3 | 0 | 141 passed |
| 4 | 0 | 138 passed |

(Part 4 shows 138 rather than 143 because an unused validation rule and its
two tests were deleted in that part. That is expected.)

---

## Before you start

Open **Command Prompt**, then run these one line at a time.

Go to the project folder:

```
cd C:\Users\Maheli\Desktop\LumoraAcademy
```

Check you are in the right repository:

```
git remote -v
```

You should see `Group4-Synergy-LumoraProject`. If you do not, stop and check the folder.

Make sure your Dev is up to date with GitHub:

```
git checkout Dev
```

```
git pull origin Dev
```

---

## PART 1 — Classes, Attendance and Payments

Switch to the part 1 branch:

```
git checkout part1/classes-attendance-payments
```

Check what is in it:

```
git log --oneline Dev..HEAD
```

You should see 8 commits. Now push it:

```
git push -u origin part1/classes-attendance-payments
```

**Then on GitHub:**

1. Open the repository in your browser.
2. A yellow bar appears saying `part1/classes-attendance-payments had recent pushes`. Click **Compare & pull request**.
3. At the top, the left box must say **Dev**, not `main`. If it says `main`, click it and choose **Dev**.
4. Title: `Part 1 - Classes, attendance and payments`
5. Click **Create pull request**.
6. Click **Merge pull request**, then **Confirm merge**.

**Do not start part 2 until part 1 is merged.**

---

## PART 2 — Validation and the registration forms

Bring your Dev up to date with the part 1 merge:

```
git checkout Dev
```

```
git pull origin Dev
```

Switch to part 2 and push it:

```
git checkout part2/validation-and-forms
```

```
git log --oneline Dev..HEAD
```

You should see 6 commits.

```
git push -u origin part2/validation-and-forms
```

**On GitHub:** same steps as part 1.
Title: `Part 2 - Input validation and the registration forms`
Base must be **Dev**. Create, then merge.

---

## PART 3 — Search, filters and screen behaviour

```
git checkout Dev
```

```
git pull origin Dev
```

```
git checkout part3/filters-and-ux
```

```
git log --oneline Dev..HEAD
```

You should see 5 commits.

```
git push -u origin part3/filters-and-ux
```

**On GitHub:** same steps.
Title: `Part 3 - Working search, filters and screen behaviour`
Base must be **Dev**. Create, then merge.

---

## PART 4 — Branding and the test case list

```
git checkout Dev
```

```
git pull origin Dev
```

```
git checkout part4/branding-and-testcases
```

```
git log --oneline Dev..HEAD
```

You should see 8 commits.

```
git push -u origin part4/branding-and-testcases
```

**On GitHub:** same steps.
Title: `Part 4 - Colour palette, institute logo and the test case list`
Base must be **Dev**. Create, then merge.

---

## After all four are merged

Bring your own copy up to date:

```
git checkout Dev
```

```
git pull origin Dev
```

Check everything arrived (you should see all 27 commits):

```
git log --oneline -30
```

Build and test the merged result:

```
dotnet build LumoraAcademy/LumoraAcademy.csproj
```

```
dotnet test LumoraAcademy.Tests/LumoraAcademy.Tests.csproj
```

You should get **0 errors** and **138 tests passed**.

Then tell the other members to run these two lines so they get the new work:

```
git checkout Dev
```

```
git pull origin Dev
```

They should also delete their old database file once, so the new demo data is
created. Close the app first, then paste this into Command Prompt:

```
del "%LOCALAPPDATA%\Packages\com.lumora.academy_9zz4h110yvjzm\LocalState\lumora.db"
```

---

## If something goes wrong

**The yellow "Compare & pull request" bar did not appear.**
Click the **Pull requests** tab, then **New pull request**. Set base to `Dev`
and compare to your part branch.

**The pull request shows far more commits than expected.**
The previous part has not been merged yet. Merge the earlier part first.

**`git push` says "everything up-to-date" but GitHub shows nothing.**
You are on the wrong branch. Run `git branch` and check which line has the `*`.

**You want to undo a push before merging.**
Close the pull request on GitHub without merging, then tell me and I will help.
