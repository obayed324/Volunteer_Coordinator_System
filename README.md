# 🧩 Volunteer Coordinator System

## 📌 Project Overview

The **Volunteer Coordinator System** is a desktop-based application developed using **C# (.NET Framework 4.8)** and **Microsoft SQL Server**.  
The system is designed to efficiently manage **volunteers, events, help seekers, and donations** on a single platform, primarily for **social welfare and relief activities**.

This project emphasizes:
- Proper relational database design  
- Object-Oriented Programming (OOP) principles  
- Real-world workflow automation for volunteer management  

---

## 🎯 Objectives

- Organize volunteer-based social and relief events  
- Maintain structured records of volunteers and system users  
- Track donations and help seekers for each event  
- Improve coordination, transparency, and accountability  

---

## 🛠️ Technology Stack

| Category | Technology |
|--------|------------|
| Programming Language | C# |
| Framework | .NET Framework 4.8 |
| Application Type | Desktop Application (Windows Forms) |
| Database | Microsoft SQL Server |
| IDE | Visual Studio |
| Version Control | Git & GitHub |

---

## 👥 User Roles

### 🔹 Admin
- Manage users and system configuration  

### 🔹 Event Manager
- Create and manage events  
- Assign volunteers  
- Monitor donations and help seekers  

### 🔹 Volunteer
- Participate in assigned events  

---

## ⚙️ Core Features

- User registration and **role-based access control**
- Event creation and management
- Volunteer assignment to events
- Donation tracking per event
- Help seeker management
- Relational database with integrity constraints

## 🗄️ Database Design

**Database Name:** `VolunteerCoordinatorDB`

### Tables
- `UserType`
- `User`
- `Event`
- `EventVolunteer`
- `EventDonation`
- `EventHelpSeeker`

The database follows **relational normalization** principles and uses **primary keys and foreign keys** to ensure data consistency and integrity.

## 📁 Database Schema

📂 The complete SQL schema is available at:

Database/VolunteerCoordinatorDB.sql

### 🧪 How to Create the Database

1. Open **SQL Server Management Studio (SSMS)**
2. Open `VolunteerCoordinatorDB.sql`
3. Execute the script (Press **F5**)

---

## 📂 Project Structure

Volunteer-Coordinator-System/
│
├── Volunteer_Coordinator_System/          # C# WinForms Application
│   ├── Properties/                        # Project properties
│   ├── References/                        # .NET references
│   ├── Resources/                         # App resources
│   │
│   ├── AdminView.cs                       # Admin dashboard
│   ├── DonationForm.cs                   # Donation management
│   ├── DonorView.cs                      # Donor interface
│   ├── Event.cs                           # Event model
│   ├── EventManager.cs                   # Event manager logic
│   ├── EventManageByAdmin.cs              # Admin event control
│   ├── GeneralUser.cs                    # General user model
│   ├── HelpSeeking.cs                    # Help seeker handling
│   ├── LoginForm.cs                      # User login form
│   ├── MainForm.cs                       # Main application UI
│   ├── MyActivityForm.cs                 # User activity tracking
│   ├── SignUp.cs                         # User registration
│   ├── User.cs                           # User model
│   ├── VolunteerView.cs                  # Volunteer dashboard
│   ├── VolunteerAndHelpSeekerManageByAdmin.cs
│   │                                      # Admin control panel
│   ├── DbHelper.cs                       # Database connection & queries
│   ├── App.config                        # Connection string & config
│   └── Program.cs                        # Application entry point
│
├── Database/                              # Database scripts
│   └── VolunteerCoordinatorDB.sql        # SQL Server schema
│
├── Screenshots/                           # UI & DB screenshots
│
├── README.md                              # Project documentation
└── .gitignore                             # Git ignore rules





## ▶️ How to Run the Project

1. Clone or download the repository  
2. Open the project in **Visual Studio**  
3. Restore the database using the provided SQL script  
4. Update the **connection string** if required  
5. Build and run the project  

---

## 🔐 Security & Best Practices

- Role-based access control
- Structured database relationships
- Separation of application logic and database schema
- Reusable and maintainable code structure

---

## 🚀 Future Improvements

- Web-based version using **ASP.NET**
- REST API integration
- Encrypted authentication
- Reporting and analytics dashboard
- Cloud database deployment

---

## 👨‍💻 Author

**Obayed Sarker**  
Student, Computer Science & Engineering  
American International University – Bangladesh (AIUB)

- 🌐 Portfolio: https://obayed-eportfolio.netlify.app/
- 💻 GitHub: https://github.com/obayed324

---

## 📄 License

This project is developed for **academic and learning purposes**.
