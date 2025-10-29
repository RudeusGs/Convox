# 🧭 Convox – Real-Time Online Learning Platform

**Convox** is a modern, lightweight online learning platform inspired by Discord and Zoom. Built with **ASP.NET Core** and **Vue.js**, it delivers real-time messaging, video/audio conferencing, AI-assisted learning, collaborative study tools, and gamification, perfect for virtual classrooms, study groups, or training sessions.

![EduConvox Demo](docs/screenshots/demo.png)

![.NET](https://img.shields.io/badge/.NET-8-blueviolet)
![Vue.js](https://img.shields.io/badge/Vue.js-3-4FC08D)
![License](https://img.shields.io/badge/license-MIT-green)

---

## ✨ Features

### 1️⃣ Classroom & Room Management
- **Room Creation & Ownership**: Creator becomes the **room owner** with full privileges including mute/unmute, disable camera, kick users, ban re-entry, and assign group leader or assistant roles.  
- **Roles & Permissions**: Temporary or dynamic permissions for selected users (e.g., allow 5-minute presentation).  
- **Room Security**: Password-protected rooms to control access.  
- **Member Management**: Add users during creation via search, or invite via link/email after room creation.  
- **Breakout Rooms**: Divide students into small discussion groups with independent rooms and screen sharing.

### 2️⃣ Interactive Learning
- **Real-time Messaging & Voice/Video**: Chat, voice, and video communication.  
- **Raise Hand & Queue**: Students raise hand, teacher manages speaking queue.  
- **Polls & Quizzes**: Quick surveys or mini quizzes with live result aggregation.  
- **Reactions / Emojis**: Instant emoji reactions without disrupting the session.  

### 3️⃣ AI & Smart Assistance
- **AI Q&A**: Ask questions in shared chat; AI responds immediately.  
- **Highlights & Suggestions**: AI summarizes popular questions or flags key points for teacher review.

### 4️⃣ Documents & Assignment Management
- **Share Documents**: Upload and share learning materials with selective access.  
- **Version Control**: Track edits and previous versions of shared documents.  
- **Feedback & Grading**: Teacher can review and grade assignments directly.  
- **Resource Links**: Attach Google Docs, PDFs, videos, or URLs.  
- **Student Submission**: Students can upload their work for review or sharing.

### 5️⃣ Recording & Playback
- **Record Sessions**: Save full session video for later review.  
- **Bookmarks & Highlights**: Mark important moments in recordings.  
- **Offline Playback**: Watch recorded sessions with chat and shared resources intact.

### 6️⃣ Attendance & Analytics
- **Automatic Attendance**: Log user join/leave times.  
- **Chat & Q&A Logs**: Export session data to PDF for review.  
- **Notifications & Reminders**: Email, push, and calendar notifications.  
- **Multi-room Management**: Teacher can run multiple rooms simultaneously.

### 7️⃣ Learning Experience Enhancements
- **Virtual Backgrounds**: Custom backgrounds for clean visuals.  
- **Screen Annotations**: Pointer, highlight, and draw on shared screens or slides.  
- **Break Reminders**: Notify users to take breaks after long sessions.  
- **Screen Sharing Control**: Owner or assigned users can share their screens.

### 8️⃣ Gamification & Engagement
- **Badge & Points System**: Reward participation and activity.  
- **Leaderboard**: Track engagement and encourage interaction.  
- **Mini Learning Games**: Quick quizzes or interactive exercises.

### 9️⃣ Technical Core
- **Video Conference Engine**: WebRTC for real-time P2P streaming, optional SFU/TURN server for large rooms.  
- **Signaling & Event Handling**: SignalR manages room signaling and user events.  
- **Scalable & Modular Architecture**: Designed for easy feature expansion and multi-room support.  

---

## 🧰 Tech Stack

- **Frontend**: Vue.js 3, TypeScript, Pinia, Bootstrap  
- **Backend**: ASP.NET Core Web API, SignalR  
- **Authentication**: JWT (JSON Web Token)  
- **Voice/Video Calls**: WebRTC  
- **Database**: SQL Server  
- **Deployment**: Docker, Vercel (Frontend), Azure/Render (Backend)  

---

## 📁 Project Structure

- Lynqis/
- ├── client/ # Vue.js frontend
- ├── server/ # ASP.NET Core backend
- ├── docs/ # Documentation and planning
- └── README.md

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js & npm](https://nodejs.org/)
- SQL Server
- Visual Studio / VS Code
- Docker
  
### Installation
#### 1. Clone the Repository
```bash
git clone https://github.com/RudeusGs/Convox.git
cd Convox
```
#### 2. Backend
```bash
- cd server
- dotnet restore
- dotnet run
```
- Configure the database connection in appsettings.json.
- Run migrations (if applicable): dotnet ef database update.
#### 3. Frontend
```bash
- cd client
- npm install
- npm run dev
```
- 📸 Screenshots
Add screenshots of your app interface here (coming soon).

- 🛡️ License
This project is licensed under the MIT License – see the LICENSE file for details.

- 🤝 Contributing
Pull requests are welcome!
For major changes, please open an issue first to discuss what you would like to change.
- 📬 Contact
Have questions or suggestions? Reach out via GitHub Issues or connect with us on [Ngô Trần Nguyên Quân](https://www.facebook.com/rudeusgrey198/).

- 🌟 Acknowledgments
Inspired by Discord’s intuitive design.
Built with love using open-source tools and libraries.
