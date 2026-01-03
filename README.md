# TitheSync - Church Financial Management System

A production-grade desktop financial management system built with WPF for church administration, tithe tracking, and donor management. Features clean architecture, immutable domain models, and comprehensive financial reporting.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-MVVM-0078D4?style=flat-square)](https://github.com/dotnet/wpf)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2025-CC2927?style=flat-square)](https://www.microsoft.com/sql-server)
[![Dapper](https://img.shields.io/badge/Dapper-Micro--ORM-orange?style=flat-square)](https://github.com/DapperLib/Dapper)

## Overview

TitheSync streamlines church financial operations by managing member information, tithe payments, donor tracking, and comprehensive financial reporting. Built with accountability and transparency in mind using N-tier architecture, MVVM pattern, and immutable domain models.

**Key Highlights:**
- Clean N-tier architecture with clear separation of concerns
- Member and donor management with organizational hierarchy
- Tithe and payment tracking with complete audit trail
- Bible class and organization categorization
- Advanced comparative financial reporting
- Excel export functionality for detailed analysis
- Leadership designation and role tracking

---

## Features

### Member Management
- Comprehensive member profiles with contact information
- Gender and address tracking
- Leadership designation system
- Organizational affiliation (Men's Fellowship, Women's Fellowship, Youth Fellowship, etc.)
- Bible class assignment (ProfDanso, EmeliaAkrofi, MrLartey, etc.)
- Hierarchical organization structure

### Payment & Tithe Tracking
- Individual payment recording with date tracking
- Member-linked transaction history
- Decimal precision for financial accuracy
- Complete audit trail of all contributions
- Payment search and filtering capabilities

### Financial Reporting
- Bible Class performance comparison
- Quarter-over-quarter payment analysis
- Individual member contribution tracking
- Top contributors leaderboard with visual rankings
- Year and period-based comparative views
- Multi-quarter financial trend analysis
- **Excel export for advanced reporting and analysis**

### Visual Analytics
- Pie chart visualization for Bible Class contributions
- Color-coded performance indicators
- Top 10 contributors dashboard
- Period comparison side-by-side views
- Interactive report generation

---

## Architecture

### Tech Stack

- **.NET 9.0** - Latest framework features and performance improvements
- **WPF + MVVM** - Separation of UI and business logic
- **C# Classes** - Domain models with initialization syntax
- **Dapper** - High-performance micro-ORM
- **SQL Server 2025** - Enterprise-grade database
- **Async/Await** - Non-blocking operations throughout

### N-Tier Structure

```
┌─────────────────────────────────┐
│  Presentation (WPF + MVVM)      │  Views, ViewModels, Commands
├─────────────────────────────────┤
│  Application Services           │  Business Logic, Validation, Reports
├─────────────────────────────────┤
│  Domain Models                  │  Entities, Enums, Business Rules
├─────────────────────────────────┤
│  Data Access (Dapper)           │  Repositories, Async Operations
├─────────────────────────────────┤
│  SQL Server 2025                │  Relational Database
└─────────────────────────────────┘
```

---

## Domain Models

### Member
```csharp
public class Member(
    int memberId,
    string firstName,
    string lastName,
    string contact,
    string gender,
    bool isLeader,
    string address,
    OrganizationEnum organization,
    BibleClassEnum bibleClass)
{
    public int MemberId { get; init; } = memberId;
    public string FirstName { get; init; } = firstName;
    public string LastName { get; init; } = lastName;
    public string Contact { get; init; } = contact;
    public string Gender { get; init; } = gender;
    public bool IsLeader { get; init; } = isLeader;
    public string Address { get; init; } = address;
    public OrganizationEnum Organization { get; init; } = organization;
    public BibleClassEnum BibleClass { get; init; } = bibleClass;
}
```

**Key Features:**
- Primary constructor with immutable properties
- Leadership designation tracking
- Organizational and Bible class categorization
- Complete contact and address information
- Equality comparison based on MemberId

### Payment
```csharp
public class Payment(
    int paymentId,
    int paymentMemberId,
    decimal amount,
    DateOnly datePaid)
{
    public int PaymentId { get; init; } = paymentId;
    public int PaymentMemberId { get; init; } = paymentMemberId;
    public decimal Amount { get; init; } = amount;
    public DateOnly DatePaid { get; init; } = datePaid;
}
```

**Key Features:**
- Linked to member records via PaymentMemberId
- Decimal precision for accurate financial tracking
- DateOnly for payment date tracking
- Immutable transaction records

### PaymentWithName
```csharp
public class PaymentWithName(
    int paymentId,
    int paymentMemberId,
    decimal amount,
    DateOnly datePaid,
    string firstName,
    string lastName) : Payment(paymentId, paymentMemberId, amount, datePaid)
{
    public string FirstName { get; init; } = firstName;
    public string LastName { get; init; } = lastName;
}
```

**Key Features:**
- Extends Payment with member name information
- Optimized for reporting and display
- Eliminates need for additional joins in queries
- Complete payment context in single object

---

## Enumerations

### OrganizationEnum
```csharp
- MensFellowship
- WomensFellowship
- YouthFellowship
- BoysAndGirlsBrigade
- GirlsFellowship
- Suvma
- Choir
```

### BibleClassEnum
```csharp
- ProfDanso
- EmeliaAkrofi
- MrLartey
- AtoPrempen
- MichaelKumi
```

---

## Database Schema

### Key Tables

**Members** - Core member information with organizational affiliations
**Payments** - Transaction records linked to members
**Organizations** - Reference table for fellowship groups
**BibleClasses** - Reference table for class assignments

### Key Constraints

```sql
-- Payment validation
CONSTRAINT CHK_Payments_Amount CHECK (Amount >= 0)

-- Member contact validation
CONSTRAINT CHK_Members_Contact CHECK (LEN(Contact) >= 10)

-- Date validation
CONSTRAINT CHK_Payments_DatePaid CHECK (DatePaid <= GETDATE())

-- Foreign key relationships
CONSTRAINT FK_Payments_Members FOREIGN KEY (PaymentMemberId) 
    REFERENCES Members(MemberId)
```

---

## Performance Features

### Async Operations
All database operations are asynchronous for responsive UI:
```csharp
await memberRepository.GetByIdAsync(memberId);
await paymentService.RecordPaymentAsync(payment);
await reportService.GenerateQuarterlyReportAsync(quarter, year);
```

### Optimized Reporting
- Aggregated queries for fast report generation
- Cached member and Bible class reference data
- Parameterized queries for SQL plan reuse
- Efficient Excel export with minimal memory footprint

### Dapper Optimization
- Direct mapping to domain models
- Batch operations for bulk imports
- Multi-mapping for complex queries (PaymentWithName)
- Stored procedures for complex financial calculations

---

## Getting Started

### Prerequisites
- Windows 10/11
- .NET 9.0 SDK
- SQL Server 2019+ (2025 recommended)

### Installation

1. **Clone repository**
   ```bash
   git clone https://github.com/maikelboatt/tithesync.git
   cd tithesync
   ```

2. **Setup database**
   ```bash
   sqlcmd -S your_server -i TitheSync.Database/Schema/CreateDatabase.sql
   sqlcmd -S your_server -d TitheSyncDB -i TitheSync.Database/Schema/CreateTables.sql
   ```

3. **Configure connection**
   
   Update `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "TitheSyncDatabase": "Server=localhost;Database=TitheSyncDB;Integrated Security=true;TrustServerCertificate=true"
     }
   }
   ```

4. **Run**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project TitheSync.UI
   ```

Default credentials: `admin` / `admin123` (change immediately)

---

## Project Structure

```
TitheSync/
├── TitheSync.UI/                 # WPF Views & ViewModels
│   ├── Views/                    # Member, Payment, Report Views
│   ├── Components/               # Reusable UI Components
│   ├── Forms/                    # Input Forms
│   ├── Resources/                # Styles, Themes
│   └── Assets/                   # Images, Icons
├── TitheSync.Business/           # Business Services & Validation
├── TitheSync.Domain/             # Domain Models & Enums
│   ├── Models/                   # Member, Payment, PaymentWithName
│   └── Enums/                    # OrganizationEnum, BibleClassEnum
├── TitheSync.DataAccess/         # Dapper Repositories
├── TitheSync.Infrastructure/     # Logging, Config, Security
└── TitheSync.Core/               # Shared Utilities
```

---

## Key Design Patterns

**MVVM** - Complete UI/logic separation  
**Repository Pattern** - Abstracted data access  
**Primary Constructors** - Concise model initialization  
**Immutable Properties** - Thread-safe init-only setters  
**Inheritance** - PaymentWithName extends Payment  
**Async/Await** - Non-blocking I/O throughout

---

## Example Usage

### Record Payment
```csharp
var payment = new Payment(
    paymentId: 0,
    paymentMemberId: 42,
    amount: 500.00m,
    datePaid: DateOnly.FromDateTime(DateTime.Now)
);

await paymentService.RecordPaymentAsync(payment);
```

### Generate Quarterly Report
```csharp
var report = await reportService.CompareQuartersAsync(
    year1: 2026,
    quarter1: "Q1 2026",
    year2: 2025,
    quarter2: "Q1 2025"
);

// Export to Excel for advanced analysis
await excelService.ExportComparisonAsync(report, "Q1_Comparison_2026_vs_2025.xlsx");
```

### Get Top Contributors
```csharp
var topMembers = await reportService.GetTopContributorsAsync(
    year: 2026,
    quarter: "Q1 2026",
    limit: 10
);

// Display in leaderboard with visual ranking
foreach (var member in topMembers)
{
    Console.WriteLine($"{member.FirstName} {member.LastName}: {member.Amount:C}");
}
```

### Compare Bible Class Performance
```csharp
var comparison = await reportService.CompareBibleClassPaymentsAsync(
    year1: 2026,
    period1: "Q1 2026",
    year2: 2025,
    period2: "Q1 2025"
);

// Generate pie chart visualization
chartService.GeneratePieChart(comparison);

// Export detailed breakdown to Excel
await excelService.ExportBibleClassComparisonAsync(comparison, "BibleClass_Q1_Analysis.xlsx");
```

---

## Reporting Features

### Visual Analytics
- **Pie Charts**: Bible class contribution distribution
- **Bar Charts**: Quarter-over-quarter trends
- **Leaderboards**: Top 10 contributors with rankings
- **Comparison Views**: Side-by-side period analysis

### Excel Export
After generating any comparison report, users can export to Excel for:
- Advanced pivot table analysis
- Custom chart creation
- Budget planning and forecasting
- Board presentations and financial reviews
- Detailed member contribution breakdowns
- Historical trend analysis

### Report Types
1. **Bible Class Comparison** - Compare contributions across classes
2. **Member Payments** - Individual contribution tracking
3. **Quarterly Analysis** - Period-over-period performance
4. **Top Contributors** - Recognition and appreciation reports
5. **Organization Summary** - Fellowship-level aggregations

---

## Security

- **Role-based access control** - Admin, Treasurer, Viewer roles
- **Password hashing** - Secure credential storage
- **Audit logging** - Complete transaction history
- **Parameterized queries** - SQL injection protection
- **Data privacy** - Member information protection

---

## Testing

```bash
dotnet test                                    # All tests
dotnet test --filter Category=Unit             # Unit tests only
dotnet test /p:CollectCoverage=true            # With coverage
```

**Coverage Areas:**
- Domain model initialization and equality
- Payment validation logic
- Report calculation accuracy
- Repository async operations
- Excel export functionality

---

## Roadmap

- [ ] Mobile companion app for payment recording
- [ ] SMS/Email payment reminders
- [ ] Automated quarterly report generation
- [ ] Budget tracking and forecasting
- [ ] Pledge management system
- [ ] Multi-church support for diocesan administration
- [ ] RESTful API for third-party integrations
- [ ] Cloud backup and synchronization
- [ ] Receipt generation with PDF export

---

## Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/NewFeature`)
3. Follow domain model patterns (primary constructors, init properties)
4. Add tests for new features
5. Commit changes (`git commit -m 'Add NewFeature'`)
6. Push to branch (`git push origin feature/NewFeature`)
7. Open Pull Request

---

## License

MIT License - see [LICENSE](LICENSE)

---

## Author

**Michael Boateng**  
GitHub: [@maikelboatt](https://github.com/maikelboatt)  
Email: boattmaikel@gmail.com

---

Built with WPF, .NET 9, Dapper, and SQL Server 2025

⭐ Star this repo if you find it useful!

---

## Acknowledgments

Special thanks to church administrators and treasurers who provided invaluable feedback on financial workflows and reporting requirements. This system was built to serve the transparency and accountability needs of faith-based organizations.
