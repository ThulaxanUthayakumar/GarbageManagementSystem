# ♻️ Garbage Management System

A modern full-stack Garbage Management System built with **React**, **ASP.NET Core 8 Web API**, and **SQL Server**. This application helps streamline waste management operations by providing an efficient platform for managing waste collection requests, tracking activities, and improving operational workflows.

---

## 🚀 Features

- 🔐 User Authentication & Authorization
- 👥 User Management
- 🗑️ Waste Collection Request Management
- 📊 Dashboard with Statistics
- 📅 Collection Scheduling
- ✏️ Create, Read, Update & Delete (CRUD)
- 📱 Responsive User Interface
- 🔗 RESTful API Integration
- 💾 SQL Server Database

---

## 🛠️ Tech Stack

### Frontend
- React
- Vite
- TypeScript
- CSS

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core
- C#

### Database
- SQL Server

### Tools
- Visual Studio
- Visual Studio Code
- Git & GitHub
- Postman

---

## 📂 Project Structure

```text
GarbageManagementSystem
│
├── frontend
│   ├── src
│   ├── public
│   └── package.json
│
├── backend
│   ├── GarbageManagementSystem.API
│   ├── GarbageManagementSystem.Application
│   ├── GarbageManagementSystem.Domain
│   ├── GarbageManagementSystem.Infrastructure
│   └── GarbageManagementSystem.Persistence
│
└── README.md
```

---

## ⚙️ Installation

### Clone the repository

```bash
git clone https://github.com/ThulaxanUthayakumar/GarbageManagementSystem.git
```

### Backend Setup

```bash
cd backend/GarbageManagementSystem.API
dotnet restore
dotnet ef database update
dotnet run
```

Swagger:

```
http://localhost:5001/swagger
```

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

Frontend:

```
http://localhost:5173
```

---

## 📈 Future Enhancements

- Email Notifications
- SMS Alerts
- Google Maps Integration
- Mobile Application
- Analytics Dashboard
- Report Export (PDF & Excel)

---

## 🤝 Contributing

Contributions are welcome!

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Push to your branch.
5. Open a Pull Request.

---

## 👨‍💻 Author

**Thulaxan Uthayakumar**

- GitHub: https://github.com/ThulaxanUthayakumar
- LinkedIn: https://www.linkedin.com/in/thulaxan-uthayakumar/

---

## ⭐ Support

If you found this project helpful, please consider giving it a ⭐ on GitHub.
