📘 Daily Routine Tracker (WPF + SQL Server)
A desktop productivity app built with WPF (.NET) and SQL Server, designed to help users:

Track daily tasks (✔ Done / ❌ Not done)

Evaluate themselves across focus categories (score 1–10)

Monitor progress using weekly/monthly charts and reports

Save, load, and update entries with a clean, persistent UI

✨ Features
✅ Task & Evaluation Grid – Auto-loaded from your database

✅ Log Today – Insert/update daily status and scores

✅ Summary Reports – Weekly/monthly score breakdowns

✅ Chart Dashboard – Bar chart with color-coded scores

✅ Auto-restore Today’s Log – Continues where you left off

✅ Dropdown Score Selection – Quick input per category

🛠 Tech Stack
Area	Stack
UI	WPF (.NET 6 or 7)
DB	SQL Server (local)
ORM	Dapper
Charts	LiveCharts
Architecture	MVVM

📦 Local Setup
🧩 Prerequisites
Windows 10/11

SQL Server (local or Express)

.NET Desktop Runtime (v6 or v7)

⚙️ Configure Your Connection String
Edit DbService.cs:
private readonly string _connectionString = 
    "Server=localhost;Database=MyDailyRoutine;User Id=sa;Password=yourpassword;TrustServerCertificate=True;";


    🛠 Database Schema
Run the following SQL script to create tables:

<details> <summary>Click to expand schema</summary>

-- TaskDefinition
CREATE TABLE TaskDefinition (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    TaskName VARCHAR(255),
    TimeSlot VARCHAR(50)
);

-- EvaluationDefinition
CREATE TABLE EvaluationDefinition (
    EvaluationId INT PRIMARY KEY IDENTITY(1,1),
    Question VARCHAR(255),
    Category VARCHAR(50)
);

-- DailyLog
CREATE TABLE DailyLog (
    LogId INT PRIMARY KEY IDENTITY(1,1),
    LogDate DATE UNIQUE,
    TaskScore INT,
    EvaluationAvg FLOAT,
    CombinedScore FLOAT
);

-- TaskLog
CREATE TABLE TaskLog (
    TaskLogId INT PRIMARY KEY IDENTITY(1,1),
    LogId INT FOREIGN KEY REFERENCES DailyLog(LogId),
    TaskId INT FOREIGN KEY REFERENCES TaskDefinition(TaskId),
    IsCompleted BIT
);

-- EvaluationLog
CREATE TABLE EvaluationLog (
    EvalLogId INT PRIMARY KEY IDENTITY(1,1),
    LogId INT FOREIGN KEY REFERENCES DailyLog(LogId),
    EvaluationId INT FOREIGN KEY REFERENCES EvaluationDefinition(EvaluationId),
    Score INT
);


🚀 Publish Locally
In Visual Studio: Build → Publish → Folder

Run the .exe inside the publish/ folder

🙌 Contributing
This is a personal utility app — feel free to fork, enhance, or turn into a more advanced self-tracking platform.