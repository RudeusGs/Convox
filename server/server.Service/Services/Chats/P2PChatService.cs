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

            var pageMessages = await _dataContext.ChatP2Ps
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
                    m.ImageUrl,
                    m.CreatedDate,
                    m.UpdatedDate,
                    IsEdited = m.IsEdited,
                    m.ReplyToMessageId,
                    m.IsForwarded,
                    m.ForwardedFromMessageId,
                    m.ForwardedFromSource
                })
                .ToListAsync();

            var replyIds = pageMessages
                .Where(m => m.ReplyToMessageId.HasValue)
                .Select(m => m.ReplyToMessageId!.Value)
                .Distinct()
                .ToList();

            var replyLookup = replyIds.Count == 0
                ? new Dictionary<int, object>()
                : await _dataContext.ChatP2Ps
                    .Where(m => replyIds.Contains(m.Id) && m.DeletedDate == null)
                    .Select(m => new
                    {
                        m.Id,
                        SenderId = m.SenderId,
                        m.Message,
                        m.MessageType,
                        ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                        m.CreatedDate
                    })
                    .ToDictionaryAsync(x => x.Id, x => (object)x);

            var msgIds = pageMessages.Select(m => m.Id).ToList();
            var reactionSummary = msgIds.Count == 0
                ? new List<dynamic>()
                : await _dataContext.MessageReactionP2Ps
                    .Where(r => msgIds.Contains(r.MessageId) && r.DeletedDate == null)
                    .GroupBy(r => new { r.MessageId, r.Emoji })
                    .Select(g => new
                    {
                        g.Key.MessageId,
                        Reaction = new
                        {
                            Emoji = g.Key.Emoji,
                            Count = g.Count(),
                            UserIds = g.Select(x => x.UserId).ToList()
                        }
                    })
                    .ToListAsync<dynamic>();

            var reactionLookup = reactionSummary
                .GroupBy(x => (int)x.MessageId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => (object)x.Reaction).ToList());

            var messages = pageMessages
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
                    m.IsEdited,
                    m.ReplyToMessageId,
                    ReplyToMessage = m.ReplyToMessageId.HasValue && replyLookup.TryGetValue(m.ReplyToMessageId.Value, out var reply)
                        ? reply
                        : null,
                    Reactions = reactionLookup.TryGetValue(m.Id, out var reactions) ? reactions : new List<object>(),
                    m.IsForwarded,
                    m.ForwardedFromMessageId,
                    m.ForwardedFromSource
                })
                .ToList();

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

            // Validate reply if provided
            object? replyToMessage = null;
            if (model.ReplyToMessageId.HasValue)
            {
                var replyMessage = await _dataContext.ChatP2Ps
                    .Where(m => m.Id == model.ReplyToMessageId.Value && m.DeletedDate == null)
                    .Select(m => new { m.Id, m.SenderId, m.Message, m.CreatedDate })
                    .FirstOrDefaultAsync();

                if (replyMessage == null)
                    return ApiResult.Fail("Tin nhắn reply không tồn tại", "REPLY_MESSAGE_NOT_FOUND");
                
                replyToMessage = replyMessage;
            }

            var messageType = ChatMessageHelper.DetermineMessageType(model.MessageContent, model.ImageUrls);
            var imageUrlsString = ChatMessageHelper.JoinImageUrls(model.ImageUrls);

            var chatP2P = new ChatP2P
            {
                SenderId = model.SenderId,
                ReceiverId = model.ReceiverId,
                Message = model.MessageContent ?? "",
                MessageType = messageType,
                ImageUrl = imageUrlsString,
                ReplyToMessageId = model.ReplyToMessageId,
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
                chatP2P.ReplyToMessageId,
                ReplyToMessage = replyToMessage,
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

            object? replyToMessage = null;
            if (chatMessage.ReplyToMessageId.HasValue)
            {
                replyToMessage = await _dataContext.ChatP2Ps
                    .Where(m => m.Id == chatMessage.ReplyToMessageId.Value && m.DeletedDate == null)
                    .Select(m => new
                    {
                        m.Id,
                        SenderId = m.SenderId,
                        m.Message,
                        m.MessageType,
                        ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                        m.CreatedDate
                    })
                    .FirstOrDefaultAsync();
            }

            var reactions = await _dataContext.MessageReactionP2Ps
                .Where(r => r.MessageId == chatMessage.Id && r.DeletedDate == null)
                .GroupBy(r => r.Emoji)
                .Select(g => new
                {
                    Emoji = g.Key,
                    Count = g.Count(),
                    UserIds = g.Select(x => x.UserId).ToList()
                })
                .ToListAsync<object>();

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
                IsEdited = chatMessage.IsEdited,
                chatMessage.ReplyToMessageId,
                ReplyToMessage = replyToMessage,
                Reactions = reactions
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
