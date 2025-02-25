# VaultX - A Demo Banking App 🏦

VaultX is a demonstration banking application built to showcase **C#**, **.NET**, **Entity Framework**, and **React** skills. It features a multi-account system with support for different currencies, providing a real-world example of modern full-stack development practices.

## 🚀 Technologies Used

### **Backend:**
- C# with .NET 8/9
- Entity Framework Core
- PostgreSQL
- OData (Open Data Protocol)

### **Frontend:**
- React with TypeScript
- TailwindCSS (or another styling framework)
- Axios for API calls

## 🔹 Features
- 🌍 Multi-currency support for multiple accounts
- 🔐 Secure authentication and account management
- 📊 Transaction history and balance tracking
- 📡 RESTful & OData API with filtering and pagination

## ⚡ Getting Started

### **Prerequisites**
- .NET SDK 8/9
- Node.js & npm/yarn
- PostgreSQL Database

### **Setup Instructions**
1. **Clone the Repository**  

```sh
   git clone https://github.com/your-username/VaultX.git
   cd VaultX
```

2. **Backend Setup**

```sh
  cd Backend
  dotnet restore
  dotnet ef migrations add CreateDbStructure
  dotnet ef database update
  dotnet run
```
