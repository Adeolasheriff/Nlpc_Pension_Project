This project is a Pension Management System built using ASP.NET Core (.NET 8), applying Clean Architecture, Domain-Driven Design (DDD), and SOLID principles. It features contributions tracking, benefit calculation, background job processing, and strong validation mechanisms.


API Documentation
The API follows RESTful design principles and uses proper HTTP status codes:

200 OK: Successful request
201 Created: Resource successfully created
204 No Content: Successful update/delete operation
400 Bad Request: Validation errors
404 Not Found: Resource not found
500 Internal Server Error: Unexpected errors
Available Endpoints
Members

GET /api/v1/members/{id} - Retrieve member details
POST /api/v1/members - Create new member
PUT /api/v1/members - Update member details
DELETE /api/v1/members/{id} - Delete member
Contributions

POST /api/v1/contributions - Process contribution
GET /api/v1/contributions/{id} - Retrieve contribution details
GET /api/v1/contributions/member/{memberId} - List member contributions
Benefits

GET /api/v1/benefits/{memberId} - Calculate member benefits
Database Schema
The system uses the following tables:

Members
Contributions
Benefits
Employers
Members Table includes Soft Delete
Design Decisions
Clean Architecture : The system follows Clean Architecture principles with clear separation of concerns between domain, application, infrastructure, and presentation layers.
Domain-Driven Design : The domain model contains rich business logic and validation rules.
Repository Pattern : Implemented to abstract data access logic.
Dependency Injection : All services are registered using constructor injection.
Error Handling : Custom middleware handles exceptions and returns appropriate error responses.
Background Processing : Hangfire is used for scheduled jobs like report generation and benefit updates.
Validation : FluentValidation ensures data integrity across all layers.
Future Enhancements

Add more sophisticated benefit calculation algorithms
Implement detailed audit logging
Add multi-tenancy support for multiple pension providers
Implement advanced reporting features



-- Initial migration script
CREATE TABLE [dbo].[Members] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [DateOfBirth] DATETIME NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

CREATE TABLE [dbo].[Contributions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ReferenceNumber] UNIQUEIDENTIFIER NOT NULL,
    [Type] INT NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [ContributionDate] DATETIME NOT NULL,
    [MemberId] INT NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (MemberId) REFERENCES Members(Id)
);

CREATE TABLE [dbo].[Benefits] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Type] INT NOT NULL,
    [CalculationDate] DATETIME NOT NULL,
    [EligibilityStatus] BIT NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [MemberId] INT NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (MemberId) REFERENCES Members(Id)
);

CREATE TABLE [dbo].[Employers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CompanyName] NVARCHAR(100) NOT NULL,
    [RegistrationNumber] NVARCHAR(50) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);  
