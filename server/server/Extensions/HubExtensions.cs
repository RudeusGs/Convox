using server.Hubs;

namespace server.Infrastructure.Extensions
{
    public static class HubExtensions
    {
        public static void MapRealtimeHubs(this WebApplication app)
        {
            app.MapHub<ChatHub>("/hubs/chat");
        }
    }
}
