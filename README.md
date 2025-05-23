🧭 Convox – Real-Time Chat Platform
Convox is a modern, lightweight, and feature-rich real-time chat application inspired by Discord. Built with ASP.NET Core Web API and Vue.js, Lynqis enables seamless text-based communication and peer-to-peer voice calls, offering a scalable and modular platform for communities and teams.

✨ Features
🔐 Secure Authentication: Register and log in with JWT-based user authentication.
💬 Real-Time Messaging: Instant text communication powered by SignalR.
📁 Channel-Based Conversations: Create public or private channels for group discussions.
📞 Voice Calls: Peer-to-peer voice calls using WebRTC for crystal-clear audio.
👤 User Profiles: Customizable profiles with avatars for a personalized experience.
🔔 Live Notifications: Stay updated with real-time alerts for messages and events.
🧱 Scalable Architecture: Modular design for easy maintenance and extensibility.
🧰 Tech Stack
Frontend: Vue.js, TypeScript, Pinia, Bootstrap
Backend: ASP.NET Core Web API, SignalR
Authentication: JSON Web Token (JWT)
Voice Calls: WebRTC
Database: SQL Server
Deployment: Docker, Vercel (Frontend), Azure/Render (Backend)
📁 Project Structure
Convox/
├── client/          # Vue.js frontend
├── server/          # ASP.NET Core backend
├── docs/            # Documentation and planning
└── README.md        # Project overview
🚀 Getting Started
Prerequisites
.NET 7 SDK
Node.js & npm
SQL Server (or PostgreSQL for future support)
Visual Studio or VS Code
Installation
1. Clone the Repository
git clone https://github.com/RudeusGs/Convox.git
cd lynqis
2. Set Up the Backend
cd server
dotnet restore
dotnet run
Configure the database connection in appsettings.json.
Run migrations (if applicable): dotnet ef database update.
3. Set Up the Frontend
cd client
npm install
npm run dev
The frontend will be available at http://localhost:5173 (or as configured).
Environment Configuration
Create a .env file in the client folder for frontend environment variables.
Update appsettings.json in the server folder for backend settings (e.g., JWT secrets, database connection strings).
Deployment
Frontend: Deploy to Vercel or any static hosting platform.
Backend: Deploy to Azure, Render, or a Docker container.
Docker: Use the provided Dockerfile for containerized deployment.
📸 Screenshots
Coming soon! Screenshots showcasing the user interface and features will be added.

🛡️ License
This project is licensed under the MIT License.

🤝 Contributing
We welcome contributions! To get started:

Fork the repository.
Create a feature branch (git checkout -b feature/your-feature).
Commit your changes (git commit -m 'Add your feature').
Push to the branch (git push origin feature/your-feature).
Open a pull request.
For major changes, please open an issue first to discuss your ideas.

📬 Contact
Have questions or suggestions? Reach out via GitHub Issues or connect with us on Twitter/X.

🌟 Acknowledgments
Inspired by Discord’s intuitive design.
Built with love using open-source tools and libraries.
