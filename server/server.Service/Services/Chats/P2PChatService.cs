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
    public class P2PChatService : BaseService, IP2PChatService
    {
        public P2PChatService(DataContext dataContext, IUserService userService) : base(dataContext, userService) { }

        public async Task<ApiResult> GetHistoryOfChatP2P(int userId1, int userId2, int page = 1, int pageSize = 50)
        {
            var totalMessages = await _dataContext.ChatP2Ps
                .Where(m => m.DeletedDate == null &&
                    ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                     (m.SenderId == userId2 && m.ReceiverId == userId1)))
                .CountAsync();

            var messages = await _dataContext.ChatP2Ps
                .Where(m => m.DeletedDate == null &&
                    ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                     (m.SenderId == userId2 && m.ReceiverId == userId1)))
                .OrderByDescending(m => m.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.ReceiverId,
                    m.Message,
                    m.MessageType,
                    ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                    m.CreatedDate
                })
                .ToListAsync();

            return ApiResult.Success(new
            {
                UserId1 = userId1,
                UserId2 = userId2,
                Messages = messages.OrderBy(m => m.CreatedDate).ToList(),
                TotalMessages = totalMessages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalMessages / (double)pageSize)
            });
        }

        public async Task<ApiResult> SendMessageWithImagesToP2P(SendP2PMessageWithImagesModel model)
        {
            var receiver = await _dataContext.Users.FindAsync(model.ReceiverId);
            if (receiver == null)
                return ApiResult.Fail("Người nhận không tồn tại", "USER_NOT_FOUND");

            var messageType = ChatMessageHelper.DetermineMessageType(model.MessageContent, model.ImageUrls);
            var imageUrlsString = ChatMessageHelper.JoinImageUrls(model.ImageUrls);

            var chatP2P = new ChatP2P
            {
                SenderId = model.SenderId,
                ReceiverId = model.ReceiverId,
                Message = model.MessageContent ?? "",
                MessageType = messageType,
                ImageUrl = imageUrlsString,
                CreatedDate = Now
            };

            _dataContext.ChatP2Ps.Add(chatP2P);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                chatP2P.Id,
                chatP2P.SenderId,
                chatP2P.ReceiverId,
                chatP2P.Message,
                chatP2P.MessageType,
                ImageUrls = model.ImageUrls ?? new List<string>(),
                chatP2P.CreatedDate
            }, "Gửi tin nhắn thành công");
        }
    }
}
