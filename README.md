# QuickService 🔧

> A full-stack appliance service booking and management platform built with ASP.NET Core MVC 8.

QuickService is a web application for managing appliance repair/service bookings from both the public website and internal management panels. The application includes customer booking, authenticated staff operations, administrative controls, secure authentication, booking management, notifications, file handling, and service-related workflows.

---

## 📌 Overview

The application is divided into three main areas:

```text
                     QuickService
                         │
          ┌──────────────┼──────────────┐
          │              │              │
          ▼              ▼              ▼
      Public Site    Admin Panel    Staff Panel
          │              │              │
          ▼              ▼              ▼
      Customers       Admins         Staff
          │              │              │
          └──────────────┼──────────────┘
                         ▼
                     SQL Server
```

Customers can submit service requests through the public booking flow, while authenticated Admin and Staff users can manage and process bookings according to their permissions.

---

## ✨ Key Features

### 👨‍🔧 Customer / Public Booking

- Multi-step service booking form
- Appliance category and sub-category selection
- Brand and issue information
- Warranty status
- Customer contact information
- Address and location details
- Priority selection
- Booking source detection
- Public booking without requiring an internal account
- Booking information validation
- Responsive booking experience

### 👑 Admin Panel

- Dedicated Admin dashboard
- Admin authentication
- Admin profile/settings
- Staff management
- Booking management
- Manual booking creation
- Booking status management
- Booking filtering and searching
- Booking sharing
- Operational controls and administrative workflows

### 👷 Staff Panel

- Dedicated Staff dashboard
- Staff authentication
- Staff profile/settings
- Staff-specific booking operations
- Booking details
- Booking status updates
- Manual booking support
- Booking sharing

### 📋 Booking Management

The booking management system supports:

- Pending / completed booking status
- Latest-first (LIFO) booking listing
- Booking ID search
- Status filtering
- Date-based filtering
- Today
- Yesterday
- Last hour
- Last 7 days
- Last 30 days
- Role/source-based filtering
- Pagination
- Multiple filter combinations
- Booking details view
- Copy booking details
- WhatsApp sharing

Bookings created from authenticated internal panels can be associated with the logged-in role, while public/unknown bookings are identified separately.

---

## 🔐 Authentication & Security

Security is an important part of the application architecture.

### Authentication

- Session-based authentication
- Cookie-based authentication
- Role-based authorization
- Admin and Staff access separation
- Login / logout
- Sliding session expiration
- Protected administrative and staff areas

### Web Security

- CSRF protection
- XSS mitigation
- `HttpOnly` cookies
- `Secure` cookies
- `SameSite` cookie configuration
- Server-side validation
- Client-side validation
- Password hashing
- Request rate limiting

### Login Protection

Repeated incorrect login attempts are handled separately from general request rate limiting.

```text
Incorrect Login Attempts
          │
          ▼
Configured Attempt Limit
          │
          ▼
Temporary Lockout
```

This helps reduce brute-force login attempts.

### Request Rate Limiting

General request rate limiting is used to restrict excessive requests.

When the configured request threshold is exceeded, further requests can be temporarily restricted.

### OTP Protection

The password recovery flow uses OTP-based verification.

- OTP validity: **10 minutes**
- Maximum OTP sends within the configured window: **3**
- Excessive OTP requests result in a temporary block
- Temporary OTP block duration: **1 hour**

---

## 📧 Email & Password Recovery

SMTP-based email functionality is used for password recovery and OTP-related operations.

The application supports:

```text
Forgot Password
      │
      ▼
OTP Generation
      │
      ▼
Email via SMTP
      │
      ▼
OTP Verification
      │
      ▼
Password Recovery
```

---

## 🖼️ File Uploads

The application contains a controlled image/file upload system.

Uploaded images are processed using:

- File validation
- MIME type detection
- Image processing
- PNG conversion

The application does not rely solely on the file extension supplied by the client when processing uploaded files.

---

## 🧩 Application Layouts

Separate Razor layouts are used for different areas of the application:

```text
Views/
│
├── Shared/
│   └── _Layout.cshtml
│
├── Admin/
│   └── _AdminLayout.cshtml
│
└── Staff/
    └── _StaffLayout.cshtml
```

| Layout | Purpose |
|---|---|
| `_Layout.cshtml` | Public-facing website |
| `_AdminLayout.cshtml` | Admin panel |
| `_StaffLayout.cshtml` | Staff panel |

This keeps public, administrative, and staff interfaces separated.

---

## 🗄️ Database

QuickService uses **Microsoft SQL Server** with **Entity Framework Core**.

The database contains entities for areas such as:

- Administrators
- Staff
- Booking-related data

Entity Framework Core is used to work with the application's relational data model.

---

## 🏗️ Application Architecture

The application follows the ASP.NET Core MVC architecture.

```text
Browser
   │
   ▼
ASP.NET Core Middleware
   │
   ├── Authentication
   ├── Authorization
   ├── Session
   ├── Security
   └── Rate Limiting
   │
   ▼
Controllers
   │
   ├── Admin
   ├── Staff
   ├── Booking
   ├── Authentication
   └── Other Application Features
   │
   ▼
Services / Business Logic
   │
   ▼
Entity Framework Core
   │
   ▼
SQL Server
```

---

## 🛠️ Tech Stack

### Backend

- **C#**
- **ASP.NET Core MVC 8**
- **Entity Framework Core**
- Razor Views

### Frontend

- HTML5
- CSS3
- JavaScript
- Tailwind CSS v4

### Database

- Microsoft SQL Server

### Security

- Cookie Authentication
- Session Authentication
- CSRF Protection
- XSS Mitigation
- Rate Limiting
- Password Hashing
- HttpOnly Cookies
- Secure Cookies
- SameSite Cookies

### Supporting Technologies

- SMTP
- Service Worker
- PWA
- Git
- GitHub

---

## 📱 PWA Support

The application includes basic Progressive Web App support.

A Service Worker is used primarily to assist with resource/file loading.

> This application does not currently use IndexedDB-based application storage or an offline-first caching architecture.

---

## 🔎 SEO & Public Website

The public-facing website includes SEO-oriented features such as:

- Dynamic page metadata
- Dynamic titles/descriptions
- Canonical URLs
- Structured data / Schema markup
- Service-related SEO content
- PWA metadata
- Social sharing metadata

Application configuration is used to manage dynamic metadata where applicable.

---

## 📞 Customer Support

The public application provides customer support/contact functionality.

For the application workflow, email/calls/WhatsApp based support is the primary implemented communication channel.

---

## ⚙️ Configuration

Application-specific configuration is maintained through ASP.NET Core configuration files and environment-specific settings.

Typical configuration includes:

- SQL Server connection string
- Authentication settings
- Session settings
- Cookie configuration
- SMTP configuration
- Application URLs
- Upload paths
- Security-related settings

> Never commit production credentials, SMTP passwords, connection strings containing secrets, or other sensitive configuration values to the repository.

---

## 🚀 Local Development

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022/2026
- Microsoft SQL Server
- SQL Server Management Studio
- Node.js / npm for Tailwind CSS tooling

### Clone

```bash
git clone <repository-url>
cd QuickService
```

### Restore dependencies

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

The application can then be accessed through the local ASP.NET Core development URL.

---

## 🌐 Deployment

The application is designed to run as an ASP.NET Core MVC web application and can be hosted using an ASP.NET Core-compatible hosting environment.

Production deployment requires environment-specific configuration for:

- Database
- HTTPS
- SMTP
- Application secrets
- Environment Variables
- Upload directories
- Authentication/cookie settings

### Production Checklist

Before deployment, verify:

- [ ] Production connection string
- [ ] HTTPS enabled
- [ ] Secure cookie configuration
- [ ] SMTP credentials
- [ ] Upload directories
- [ ] Application secrets
- [ ] Database availability
- [ ] Error handling/logging
- [ ] Production configuration

---

## 📁 Project Structure

A simplified project structure:

```text
QuickService/
│
├── Controllers/
├── Helpers/
├── Middleware/
├── Models/
├── Services/
├── Views/
│   ├── Admin/
│   ├── Staff/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── uploads/
│
├── Properties/
├── Program.cs
├── appsettings.json
├── QuickService.csproj
└── QuickService.sln
```

The structure may evolve as new application features are added.

---

## 🔄 Booking Workflow

A simplified booking flow:

```text
Customer
   │
   ▼
Select Appliance
   │
   ▼
Select Service / Issue
   │
   ▼
Enter Customer Details
   │
   ▼
Enter Address
   │
   ▼
Submit Booking
   │
   ▼
Booking Created
   │
   ▼
Admin / Staff Panel
   │
   ▼
Process Service Request
   │
   ▼
Update Booking Status
```

Bookings can originate from the public booking interface or internal panel-based booking functionality.

---

## 📝 Development Notes

This repository contains the development codebase for the QuickService application.

When making changes:

1. Understand the existing authentication and authorization flow before modifying protected areas.
2. Check role permissions before adding administrative functionality.
3. Avoid exposing private upload paths or sensitive information.
4. Do not commit production secrets.
5. Test public booking and authenticated booking flows separately.
6. Test both Admin and Staff access after authorization-related changes.
7. Verify mobile/responsive behavior after UI changes.
8. Test security-sensitive changes independently before deployment.

---

## 🛠️ Project Technologies

**Alok Maurya**

Software Development Engineer

**Core interests:**
C# • ASP.NET Core • JavaScript • React • Next.js • SQL Server • NoSQL • Cloud & Deployment

---

⭐ If you like the portfolio or find the code useful, feel free to star the repository.
