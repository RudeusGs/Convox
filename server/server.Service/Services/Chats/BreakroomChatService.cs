using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Domain.Entities.Chats;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Common.Services;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Models.Chats;
using server.Service.Utilities;

namespace server.Service.Services.Chats
{
    public class BreakroomChatService : BaseService, IBreakroomChatService
    {
        public BreakroomChatService(DataContext dataContext, IUserService userService) : base(dataContext, userService) { }

        public async Task<ApiResult> GetHistoryOfBreakroomChat(int breakroomId, int page = 1, int pageSize = 50)
        {
            var breakroom = await _dataContext.BreakoutRooms.FindAsync(breakroomId);
            if (breakroom == null)
                return ApiResult.Fail("Phòng con không tồn tại", "BREAKROOM_NOT_FOUND");

            var totalMessages = await _dataContext.ChatMessageBreakoutRooms
                .Where(m => m.BreakoutRoomId == breakroomId && m.DeletedDate == null)
                .CountAsync();

            var messages = await _dataContext.ChatMessageBreakoutRooms
                .Where(m => m.BreakoutRoomId == breakroomId && m.DeletedDate == null)
                .OrderByDescending(m => m.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.BreakoutRoomId,
                    m.UserId,
                    m.Message,
                    m.MessageType,
                    ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                    m.CreatedDate
                })
                .ToListAsync();

            return ApiResult.Success(new
            {
                BreakroomId = breakroomId,
                Messages = messages.OrderBy(m => m.CreatedDate).ToList(),
                TotalMessages = totalMessages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalMessages / (double)pageSize)
            });
        }

        public async Task<ApiResult> SendMessageWithImagesToBreakroom(SendBreakroomMessageWithImagesModel model)
        {
            var breakroom = await _dataContext.BreakoutRooms.FindAsync(model.BreakroomId);
            if (breakroom == null)
                return ApiResult.Fail("Phòng con không tồn tại", "BREAKROOM_NOT_FOUND");

            if (breakroom.ExpireAt.HasValue && breakroom.ExpireAt.Value < Now)
                return ApiResult.Fail("Phòng con đã hết hạn", "BREAKROOM_EXPIRED");

            var messageType = ChatMessageHelper.DetermineMessageType(model.MessageContent, model.ImageUrls);
            var imageUrlsString = ChatMessageHelper.JoinImageUrls(model.ImageUrls);

            var chatMessage = new ChatMessageBreakoutRoom
            {
                BreakoutRoomId = model.BreakroomId,
                UserId = model.SenderId,
                Message = model.MessageContent ?? "",
                MessageType = messageType,
                ImageUrl = imageUrlsString,
                CreatedDate = Now
            };

            _dataContext.ChatMessageBreakoutRooms.Add(chatMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                chatMessage.Id,
                chatMessage.BreakoutRoomId,
                chatMessage.UserId,
                chatMessage.Message,
                chatMessage.MessageType,
                ImageUrls = model.ImageUrls ?? new List<string>(),
                chatMessage.CreatedDate
            }, "Gửi tin nhắn thành công");
        }
    }
}
