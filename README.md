Volunteer Coordinator System
📌 Project Overview

The Volunteer Coordinator System is a desktop-based application developed using C# (.NET Framework 4.8) and Microsoft SQL Server.
The system is designed to efficiently manage volunteers, events, help seekers, and donations on a single platform, mainly for social welfare and relief activities.

This project focuses on proper database design, object-oriented programming, and real-world workflow automation.

🎯 Objectives

Organize volunteer-based social and relief events

Maintain structured records of volunteers and users

Track donations and help seekers for each event

Improve coordination and transparency in volunteer management

🛠️ Technology Stack

Programming Language: C#

Framework: .NET Framework 4.8

Application Type: Desktop Application (Windows Forms)

Database: Microsoft SQL Server

IDE: Visual Studio

Version Control: Git & GitHub

👥 User Roles

Admin

Manage users and system configuration

Event Manager

Create and manage events

Assign volunteers

Monitor donations and help seekers

Volunteer

Participate in assigned events

⚙️ Core Features

User registration and role-based access

Event creation and management

Volunteer assignment to events

Donation tracking per event

Help seeker management

Relational database with proper integrity constraints

🗄️ Database Design

Database Name: VolunteerCoordinatorDB

Tables:

UserType

User

Event

EventVolunteer

EventDonation

EventHelpSeeker

The database is designed using relational normalization, with primary keys and foreign keys to ensure data consistency.

📁 The complete SQL schema is provided in:

Database/VolunteerCoordinatorDB.sql

How to create the database:

Open SQL Server Management Studio (SSMS)

Open the file VolunteerCoordinatorDB.sql

Execute the script (Press F5)

📂 Project Structure
Volunteer-Coordinator-System/
│
├── Volunteer_Coordinator_System/   # C# Source Code
├── Database/
│   └── VolunteerCoordinatorDB.sql  # Database Script
├── Screenshots/                    # UI & DB Screenshots
└── README.md

▶️ How to Run the Project

Clone or download the repository

Open the project in Visual Studio

Restore database using the provided SQL script

Update the connection string if required

Build and run the project

🔐 Security & Best Practices

Role-based access control

Structured database relationships

Separation of application logic and database schema

Reusable and maintainable code structure

🚀 Future Improvements

Web-based version using ASP.NET

REST API integration

Authentication encryption

Reporting and analytics dashboard

Cloud database deployment

👨‍💻 Author

Obayed Sarker
Student, Computer Science & Engineering
American International University – Bangladesh (AIUB)

📄 License

This project is developed for academic and learning purposes.