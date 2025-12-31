/* =========================================
   Database: VolunteerCoordinatorDB
   DBMS: Microsoft SQL Server
   ========================================= */

CREATE DATABASE VolunteerCoordinatorDB;
GO

USE VolunteerCoordinatorDB;
GO

/* =========================================
   Table: UserType
   ========================================= */
CREATE TABLE UserType (
    UserTypeID INT IDENTITY(1,1) PRIMARY KEY,
    UserTypeName NVARCHAR(50) NOT NULL
);

INSERT INTO UserType (UserTypeName)
VALUES ('Admin'), ('Event Manager'), ('Volunteer');

/* =========================================
   Table: [User]
   ========================================= */
CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    Gender NVARCHAR(10),
    Location NVARCHAR(200),
    BloodGroup NVARCHAR(10),
    UserTypeID INT NOT NULL,
    FOREIGN KEY (UserTypeID) REFERENCES UserType(UserTypeID)
);

/* =========================================
   Table: Event
   ========================================= */
CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(150) NOT NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(150),
    Description NVARCHAR(MAX),
    AssignEvent INT,
    FOREIGN KEY (AssignEvent) REFERENCES [User](UserID)
);

/* =========================================
   Table: EventDonation
   ========================================= */
CREATE TABLE EventDonation (
    DonationID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    UserID INT NOT NULL,
    DonationType NVARCHAR(50),
    Amount DECIMAL(10,2) NOT NULL,
    DonationDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (EventID) REFERENCES Event(EventID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

/* =========================================
   Table: EventHelpSeeker
   ========================================= */
CREATE TABLE EventHelpSeeker (
    HelpID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    UserID INT NOT NULL,
    HelpType NVARCHAR(100),
    HelpLocation NVARCHAR(150),
    Phone NVARCHAR(15),
    Status NVARCHAR(50),
    FOREIGN KEY (EventID) REFERENCES Event(EventID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

/* =========================================
   Table: EventVolunteer
   ========================================= */
CREATE TABLE EventVolunteer (
    EventVolunteerID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    UserID INT NOT NULL,
    AssignedRole NVARCHAR(100),
    Status NVARCHAR(50),
    FOREIGN KEY (EventID) REFERENCES Event(EventID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

/* =========================================
   Sample Data
   ========================================= */

INSERT INTO [User]
(Name, Email, Password, Phone, Gender, Location, BloodGroup, UserTypeID)
VALUES
('Admin User', 'admin@vcs.com', 'admin123', '01700000000', 'Male', 'Dhaka', 'O+', 1),
('Event Manager', 'manager@vcs.com', 'manager123', '01800000000', 'Male', 'Sylhet', 'A+', 2),
('Volunteer One', 'volunteer@vcs.com', 'vol123', '01900000000', 'Female', 'Chattogram', 'B+', 3);

INSERT INTO Event
(EventName, EventDate, Location, Description, AssignEvent)
VALUES
('Flood Relief Program', '2025-01-15', 'Sylhet',
 'Relief distribution for flood affected people', 2);

INSERT INTO EventVolunteer
(EventID, UserID, AssignedRole, Status)
VALUES
(1, 3, 'Field Volunteer', 'Active');

INSERT INTO EventHelpSeeker
(EventID, UserID, HelpType, HelpLocation, Phone, Status)
VALUES
(1, 3, 'Food & Shelter', 'Sylhet City', '01900000000', 'Pending');

INSERT INTO EventDonation
(EventID, UserID, DonationType, Amount)
VALUES
(1, 1, 'Cash', 5000.00);
