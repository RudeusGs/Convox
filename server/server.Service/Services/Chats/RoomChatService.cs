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

            var pageMessages = await _dataContext.ChatMessages
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
                    m.ImageUrl,
                    m.CreatedDate,
                    m.UpdatedDate,
                    IsEdited = m.IsEdited,
                    m.ReplyToMessageId,
                    m.MentionedUserIds,
                    m.IsForwarded,
                    m.ForwardedFromMessageId,
                    m.ForwardedFromSource
                })
                .ToListAsync();

            var pinnedMessageIds = await _dataContext.PinnedMessages
                .Where(p => p.RoomId == roomId && p.DeletedDate == null)
                .Select(p => p.MessageId)
                .ToListAsync();

            var replyIds = pageMessages
                .Where(m => m.ReplyToMessageId.HasValue)
                .Select(m => m.ReplyToMessageId!.Value)
                .Distinct()
                .ToList();

            var replyLookup = replyIds.Count == 0
                ? new Dictionary<int, object>()
                : await _dataContext.ChatMessages
                    .Where(m => replyIds.Contains(m.Id) && m.RoomId == roomId && m.DeletedDate == null)
                    .Select(m => new
                    {
                        m.Id,
                        UserId = m.UserId,
                        m.Message,
                        m.MessageType,
                        ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                        m.CreatedDate
                    })
                    .ToDictionaryAsync(x => x.Id, x => (object)x);

            var msgIds = pageMessages.Select(m => m.Id).ToList();
            var reactionSummary = msgIds.Count == 0
                ? new List<dynamic>()
                : await _dataContext.MessageReactions
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
                    m.RoomId,
                    m.UserId,
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
                    MentionedUserIds = ChatMessageHelper.ParseMentionedUserIds(m.MentionedUserIds),
                    Reactions = reactionLookup.TryGetValue(m.Id, out var reactions) ? reactions : new List<object>(),
                    m.IsForwarded,
                    m.ForwardedFromMessageId,
                    m.ForwardedFromSource,
                    IsPinned = pinnedMessageIds.Contains(m.Id)
                })
                .ToList();

            var pinnedMessages = await _dataContext.PinnedMessages
                .Where(p => p.RoomId == roomId && p.DeletedDate == null)
                .OrderByDescending(p => p.CreatedDate)
                .Join(
                    _dataContext.ChatMessages.Where(m => m.RoomId == roomId && m.DeletedDate == null),
                    p => p.MessageId,
                    m => m.Id,
                    (p, m) => new
                    {
                        PinId = p.Id,
                        p.MessageId,
                        p.PinnedByUserId,
                        PinnedDate = p.CreatedDate,
                        Message = new
                        {
                            m.Id,
                            m.RoomId,
                            m.UserId,
                            m.Message,
                            m.MessageType,
                            ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                            m.CreatedDate
                        }
                    })
                .ToListAsync();

            return ApiResult.Success(new
            {
                RoomId = roomId,
                Messages = messages.OrderBy(m => m.CreatedDate).ToList(),
                PinnedMessages = pinnedMessages,
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

            // Validate reply if provided
            object? replyToMessage = null;
            if (model.ReplyToMessageId.HasValue)
            {
                var replyMessage = await _dataContext.ChatMessages
                    .Where(m => m.Id == model.ReplyToMessageId.Value && m.RoomId == model.RoomId && m.DeletedDate == null)
                    .Select(m => new { m.Id, m.UserId, m.Message, m.CreatedDate })
                    .FirstOrDefaultAsync();

                if (replyMessage == null)
                    return ApiResult.Fail("Tin nhắn reply không tồn tại hoặc không thuộc phòng này", "REPLY_MESSAGE_NOT_FOUND");
                
                replyToMessage = replyMessage;
            }

            var messageType = ChatMessageHelper.DetermineMessageType(model.MessageContent, model.ImageUrls);
            var imageUrlsString = ChatMessageHelper.JoinImageUrls(model.ImageUrls);
            var mentionedUserIds = ChatMessageHelper.JoinMentionedUserIds(model.MentionedUserIds);

            var chatMessage = new ChatMessage
            {
                RoomId = model.RoomId,
                UserId = model.SenderId,
                Message = model.MessageContent ?? "",
                MessageType = messageType,
                ImageUrl = imageUrlsString,
                ReplyToMessageId = model.ReplyToMessageId,
                MentionedUserIds = mentionedUserIds,
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
                chatMessage.ReplyToMessageId,
                ReplyToMessage = replyToMessage,
                MentionedUserIds = model.MentionedUserIds ?? new List<int>(),
                chatMessage.CreatedDate
            }, "Gửi tin nhắn thành công");
        }

        public async Task<ApiResult> EditMessageInRoom(int messageId, int userId, string newMessage, List<string>? imageUrls = null)
        {
            var chatMessage = await _dataContext.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (chatMessage == null)
                return ApiResult.Fail("Tin nhắn không tồn tại", "MESSAGE_NOT_FOUND");

            if (chatMessage.UserId != userId)
                return ApiResult.Fail("Bạn không có quyền sửa tin nhắn này", "UNAUTHORIZED");

            chatMessage.Message = newMessage;
            chatMessage.MarkUpdated();
            
            if (imageUrls != null)
            {
                chatMessage.ImageUrl = ChatMessageHelper.JoinImageUrls(imageUrls);
                chatMessage.MessageType = ChatMessageHelper.DetermineMessageType(newMessage, imageUrls);
            }

            await SaveChangesAsync();

            object? editReplyToMessage = null;
            if (chatMessage.ReplyToMessageId.HasValue)
            {
                editReplyToMessage = await _dataContext.ChatMessages
                    .Where(m => m.Id == chatMessage.ReplyToMessageId.Value && m.RoomId == chatMessage.RoomId && m.DeletedDate == null)
                    .Select(m => new
                    {
                        m.Id,
                        UserId = m.UserId,
                        m.Message,
                        m.MessageType,
                        ImageUrls = ChatMessageHelper.ParseImageUrls(m.ImageUrl),
                        m.CreatedDate
                    })
                    .FirstOrDefaultAsync();
            }

            var editReactions = await _dataContext.MessageReactions
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
                chatMessage.RoomId,
                chatMessage.UserId,
                chatMessage.Message,
                chatMessage.MessageType,
                ImageUrls = ChatMessageHelper.ParseImageUrls(chatMessage.ImageUrl),
                chatMessage.CreatedDate,
                chatMessage.UpdatedDate,
                IsEdited = chatMessage.IsEdited,
                chatMessage.ReplyToMessageId,
                ReplyToMessage = editReplyToMessage,
                MentionedUserIds = ChatMessageHelper.ParseMentionedUserIds(chatMessage.MentionedUserIds),
                Reactions = editReactions
            }, "Chỉnh sửa tin nhắn thành công");
        }

        public async Task<ApiResult> DeleteMessageInRoom(int messageId, int userId)
        {
            var chatMessage = await _dataContext.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (chatMessage == null)
                return ApiResult.Fail("Tin nhắn không tồn tại", "MESSAGE_NOT_FOUND");

            if (chatMessage.UserId != userId)
                return ApiResult.Fail("Bạn không có quyền xóa tin nhắn này", "UNAUTHORIZED");

            chatMessage.DeletedDate = Now;
            await SaveChangesAsync();

            return ApiResult.Success(new { MessageId = messageId, RoomId = chatMessage.RoomId }, "Xóa tin nhắn thành công");
        }
    }
}
