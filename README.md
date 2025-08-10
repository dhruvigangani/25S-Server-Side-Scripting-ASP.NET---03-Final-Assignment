# ShiftSchedularApplication

A comprehensive employee shift scheduling application built with ASP.NET Core 8.0.

## Features

- **Shift Management**: Create, edit, and manage employee shifts
- **Time Tracking**: Punch in/out system for accurate time recording
- **Payroll Integration**: Automatic pay calculation based on hours worked
- **Availability Scheduling**: Weekly availability management for employees
- **User Authentication**: Secure login with Google and Facebook OAuth support
- **Role-Based Access**: Users can only access their own records

## Technology Stack

- **Backend**: ASP.NET Core 8.0, Entity Framework Core
- **Database**: SQL Server (LocalDB for development, SQL Server for production)
- **Authentication**: ASP.NET Core Identity with OAuth 2.0
- **Frontend**: Razor Views, Bootstrap 5, jQuery
- **Security**: HTTPS enforcement, security headers, antiforgery tokens

## Entity-Relationship Diagram (ERD)

```
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│   AspNetUsers   │         │     Shifts      │         │  ShiftDetails   │
│   (Identity)    │<────────┤                 │<────────┤                 │
├─────────────────┤         ├─────────────────┤         ├─────────────────┤
│ Id (PK)         │         │ Id (PK)         │         │ Id (PK)         │
│ UserName        │         │ EmployeeId (FK) │         │ ShiftId (FK)    │
│ Email           │         │ StartTime       │         │ TaskDescription │
│ EmailConfirmed  │         │ EndTime         │         │ TaskStartTime   │
│ PasswordHash    │         │ IsSwapRequested │         │ TaskEndTime     │
│ SecurityStamp   │         │ IsGivenAway     │         │ TaskType        │
│ ConcurrencyStamp│         │ IsAbsent        │         │ Notes           │
└─────────────────┘         └─────────────────┘         │ IsCompleted     │
         ^                                              └─────────────────┘
         │ 1:N
         │
┌─────────────────┐         ┌─────────────────┐
│  Availabilities │         │    PayStubs     │
├─────────────────┤         ├─────────────────┤
│ Id (PK)         │         │ Id (PK)         │
│ EmployeeId (FK) │         │ EmployeeId (FK) │
│ Day             │         │ HoursWorked     │
│StartAvailability│         │ HourlyRate      │
│ EndAvailability │         │ TotalPay        │
└─────────────────┘         │ PayDate         │
                            └─────────────────┘
```

### Entity Attributes:

**AspNetUsers (Identity)**
- Id (PK): Unique identifier for each user
- UserName: User's login username
- Email: User's email address
- EmailConfirmed: Boolean indicating email verification status
- PasswordHash: Encrypted password
- SecurityStamp: Security token for password changes
- ConcurrencyStamp: Optimistic concurrency control

**Shifts**
- Id (PK): Unique identifier for each shift
- EmployeeId (FK): References AspNetUsers.Id
- StartTime: Shift start date and time
- EndTime: Shift end date and time
- IsSwapRequested: Boolean for shift swap requests
- IsGivenAway: Boolean for shifts given to other employees
- IsAbsent: Boolean for absence tracking

**ShiftDetails** **NEW - Child Entity**
- Id (PK): Unique identifier for each shift detail
- ShiftId (FK): References Shifts.Id (Parent)
- TaskDescription: Description of the specific task
- TaskStartTime: When the task starts within the shift
- TaskEndTime: When the task ends within the shift
- TaskType: Category of task (Setup, Service, Cleanup, etc.)
- Notes: Additional information about the task
- IsCompleted: Boolean indicating task completion status

**Punches**
- Id (PK): Unique identifier for each punch record
- EmployeeId (FK): References AspNetUsers.Id
- PunchInTime: Time when employee started work
- PunchOutTime: Time when employee ended work (nullable)

**Availabilities**
- Id (PK): Unique identifier for each availability record
- EmployeeId (FK): References AspNetUsers.Id
- Day: Day of the week (enum)
- StartAvailability: Earliest time available on that day
- EndAvailability: Latest time available on that day

**PayStubs**
- Id (PK): Unique identifier for each pay stub
- EmployeeId (FK): References AspNetUsers.Id
- HoursWorked: Total hours worked for the pay period
- HourlyRate: Employee's hourly wage rate
- TotalPay: Calculated total pay (HoursWorked × HourlyRate)
- PayDate: Date when pay stub was generated

### Relationships:
- **AspNetUsers** ↔ **Shifts**: One-to-Many (one user can have many shifts)
- **Shifts** ↔ **ShiftDetails**: **One-to-Many (one shift can have many task details)**
- **AspNetUsers** ↔ **Punches**: One-to-Many (one user can have many punch records)
- **AspNetUsers** ↔ **Availabilities**: One-to-Many (one user can have multiple availability records)
- **AspNetUsers** ↔ **PayStubs**: One-to-Many (one user can have multiple pay stubs)

## Application Workflow

### 1. User Authentication Flow
```
User visits site → Login/Register → OAuth (Google/Facebook) or Local Account → Authenticated Session
```

### 2. Shift Management Workflow
```
Employee logs in → Views available shifts → Creates new shift → Sets start/end times → Saves shift
Employee can edit/delete their own shifts → Mark shifts for swap requests → Track attendance
```

### 3.**Shift Detail Management Workflow (NEW)**
```
Employee creates shift → Adds task details → Specifies task types and times → Tracks completion status
Each shift can have multiple detailed tasks (Setup, Service, Cleanup, etc.) → Better shift organization
```

### 4. Time Tracking Workflow
```
Employee arrives → Punches in (creates punch record) → Works shift → Punches out (updates punch record)
System calculates total hours worked for pay stub generation
```

### 5. Availability Management Workflow
```
Employee sets weekly availability → Specifies days and time ranges → System uses this for shift scheduling
Managers can view employee availability when creating optimal schedules
```

### 6. Payroll Generation Workflow
```
System aggregates punch data → Calculates total hours → Applies hourly rate → Generates pay stub
Pay stubs show hours worked, rate, and total compensation
```

### 7. Data Flow Architecture
```
User Interface (Views) → Controllers → Entity Framework → SQL Server Database
Database changes → Entity Framework → Controllers → Views (real-time updates)
```

## How to Use the Application

### For Employees:
1. **Account Setup**: Register with email or use Google/Facebook OAuth
2. **Shift Management**: Create, view, and manage your work shifts
3. **Task Detail Management**: Add specific tasks to each shift with descriptions and completion tracking
4. **Time Tracking**: Punch in/out to record actual work hours
5. **Availability**: Set your weekly availability preferences
6. **Payroll**: View your pay stubs and earnings

### For Managers:
1. **Employee Overview**: View all employee shifts and availability
2. **Schedule Management**: Create and assign shifts based on availability
3. **Task Monitoring**: Track detailed task completion within shifts
4. **Attendance Tracking**: Monitor punch records and attendance
5. **Payroll Review**: Review and approve employee pay stubs

### Data Entry Process:
1. **Shifts**: Enter start/end times, mark special conditions (swap requests, absences)
2. **Shift Details**: Add specific tasks within each shift (Setup, Service, Cleanup, etc.)
3. **Punches**: Record actual arrival/departure times for accurate payroll
4. **Availability**: Set weekly schedule preferences for optimal shift assignment
5. **Pay Stubs**: System automatically calculates pay based on hours and rate

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB for development, SQL Server for production)
- Visual Studio 2022 or VS Code

## Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ShiftSchedularApplication
   ```

2. **Configure Database Connection**
   - **Development**: Uses LocalDB by default
   - **Production**: Update `appsettings.Production.json` with your SQL Server details

3. **Install Dependencies**
   ```bash
   dotnet restore
   ```

4. **Run Database Migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

## Configuration

### Database Connection Strings

- **Development**: `Server=(localdb)\mssqllocaldb;Database=ShiftSchedularApplication;Trusted_Connection=true;MultipleActiveResultSets=true`
- **Production**: Update with your SQL Server instance details

### Authentication

Configure OAuth providers in `appsettings.json`:
- Google Client ID and Secret
- Facebook App ID and Secret

## Default User

The application creates a default user on startup. Contact your system administrator for credentials.

## Security Features

- Password requirements: 8+ characters, mixed case, numbers, symbols
- Account lockout after 5 failed attempts
- 4-hour sliding session expiration
- HTTPS enforcement in production
- Security headers (XSS protection, content type options, frame options)

## Deployment

### Render Deployment
This application is configured for deployment on Render.com. See [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) for detailed deployment instructions.

### Docker Support
The application includes Docker support for containerized deployment.

## License

This project is licensed under the MIT License. 