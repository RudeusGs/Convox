using Microsoft.EntityFrameworkCore;
using server.Domain.Entities.Chats;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Chats;
using server.Service.Utilities;

namespace server.Service.Services.Chats
{
    public class RoomChatService : BaseService, IRoomChatService
    {
        public RoomChatService(DataContext dataContext, IUserService userService) : base(dataContext, userService) { }

        public async Task<ApiResult> GetHistoryOfRoomChat(int roomId, int page = 1, int pageSize = 50)
        {
            var room = await _dataContext.Rooms.FindAsync(roomId);
            if (room == null)
                return ApiResult.Fail("Phòng không tồn tại", "ROOM_NOT_FOUND");

            var totalMessages = await _dataContext.ChatMessages
                .Where(m => m.RoomId == roomId && m.DeletedDate == null)
                .CountAsync();

            var messages = await _dataContext.ChatMessages
                .Where(m => m.RoomId == roomId && m.DeletedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.RoomId,
                    m.UserId,
                    m.Message,
                    m.MessageType,
                    ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                    m.CreatedDate
                })
                .ToListAsync();

            return ApiResult.Success(new
            {
                RoomId = roomId,
                Messages = messages.OrderBy(m => m.CreatedDate).ToList(),
                TotalMessages = totalMessages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalMessages / (double)pageSize)
            });
        }

        public async Task<ApiResult> SendMessageWithImagesToRoom(SendRoomMessageWithImagesModel model)
        {
            var room = await _dataContext.Rooms.FindAsync(model.RoomId);
            if (room == null)
                return ApiResult.Fail("Phòng không tồn tại", "ROOM_NOT_FOUND");

            var userRoom = await _dataContext.UserRooms
                .FirstOrDefaultAsync(ur => ur.RoomId == model.RoomId
                                        && ur.UserId == model.SenderId
                                        && !ur.IsBan
                                        && ur.DeletedDate == null);

            if (userRoom == null)
                return ApiResult.Fail("Bạn không phải thành viên của phòng này hoặc đã bị ban", "ACCESS_DENIED");

            var messageType = ChatMessageHelper.DetermineMessageType(model.MessageContent, model.ImageUrls);
            var imageUrlsString = ChatMessageHelper.JoinImageUrls(model.ImageUrls);

            var chatMessage = new ChatMessage
            {
                RoomId = model.RoomId,
                UserId = model.SenderId,
                Message = model.MessageContent ?? "",
                MessageType = messageType,
                ImageUrl = imageUrlsString,
                CreatedDate = Now
            };

            _dataContext.ChatMessages.Add(chatMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                chatMessage.Id,
                chatMessage.RoomId,
                chatMessage.UserId,
                chatMessage.Message,
                chatMessage.MessageType,
                ImageUrls = model.ImageUrls ?? new List<string>(),
                chatMessage.CreatedDate
            }, "Gửi tin nhắn thành công");
        }
    }
}
