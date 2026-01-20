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
                    m.CreatedDate,
                    m.UpdatedDate,
                    IsEdited = m.IsEdited
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

        public async Task<ApiResult> EditMessageP2P(int messageId, int userId, string newMessage, List<string>? imageUrls = null)
        {
            var chatMessage = await _dataContext.ChatP2Ps
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (chatMessage == null)
                return ApiResult.Fail("Tin nhắn không tồn tại", "MESSAGE_NOT_FOUND");

            if (chatMessage.SenderId != userId)
                return ApiResult.Fail("Bạn không có quyền sửa tin nhắn này", "UNAUTHORIZED");

            chatMessage.Message = newMessage;
            chatMessage.MarkUpdated();
            
            if (imageUrls != null)
            {
                chatMessage.ImageUrl = ChatMessageHelper.JoinImageUrls(imageUrls);
                chatMessage.MessageType = ChatMessageHelper.DetermineMessageType(newMessage, imageUrls);
            }

            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                chatMessage.Id,
                chatMessage.SenderId,
                chatMessage.ReceiverId,
                chatMessage.Message,
                chatMessage.MessageType,
                ImageUrls = ChatMessageHelper.ParseImageUrls(chatMessage.ImageUrl),
                chatMessage.CreatedDate,
                chatMessage.UpdatedDate,
                IsEdited = chatMessage.IsEdited
            }, "Chỉnh sửa tin nhắn thành công");
        }

        public async Task<ApiResult> DeleteMessageP2P(int messageId, int userId)
        {
            var chatMessage = await _dataContext.ChatP2Ps
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (chatMessage == null)
                return ApiResult.Fail("Tin nhắn không tồn tại", "MESSAGE_NOT_FOUND");

            if (chatMessage.SenderId != userId)
                return ApiResult.Fail("Bạn không có quyền xóa tin nhắn này", "UNAUTHORIZED");

            chatMessage.DeletedDate = Now;
            await SaveChangesAsync();

            return ApiResult.Success(new { MessageId = messageId }, "Xóa tin nhắn thành công");
        }
    }
}
