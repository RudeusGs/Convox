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
    public class ForwardMessageService : BaseService, IForwardMessageService
    {
        public ForwardMessageService(DataContext dataContext, IUserService userService) 
            : base(dataContext, userService) { }

        public async Task<ApiResult> ForwardToRoom(ForwardToRoomModel model)
        {
            var originalMessage = await GetOriginalMessageContent(model.SourceType, model.SourceMessageId);
            if (originalMessage == null)
                return ApiResult.Fail("Tin nh?n g?c không t?n t?i", "MESSAGE_NOT_FOUND");

            var targetRoom = await _dataContext.Rooms.FindAsync(model.TargetRoomId);
            if (targetRoom == null)
                return ApiResult.Fail("Phòng ?ích không t?n t?i", "ROOM_NOT_FOUND");

            var forwardedMessage = new ChatMessage
            {
                RoomId = model.TargetRoomId,
                UserId = model.ForwarderId,
                Message = originalMessage.Message,
                MessageType = "forwarded",
                ImageUrl = originalMessage.ImageUrl,
                IsForwarded = true,
                ForwardedFromMessageId = model.SourceMessageId,
                ForwardedFromSource = ChatMessageHelper.CreateForwardSource(model.SourceType, model.SourceId),
                CreatedDate = Now
            };

            _dataContext.ChatMessages.Add(forwardedMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                forwardedMessage.Id,
                forwardedMessage.RoomId,
                forwardedMessage.UserId,
                forwardedMessage.Message,
                forwardedMessage.MessageType,
                ImageUrls = ChatMessageHelper.ParseImageUrls(forwardedMessage.ImageUrl),
                forwardedMessage.IsForwarded,
                forwardedMessage.ForwardedFromMessageId,
                forwardedMessage.ForwardedFromSource,
                forwardedMessage.CreatedDate,
                OriginalMessage = originalMessage
            }, "?ã forward tin nh?n");
        }

        public async Task<ApiResult> ForwardToP2P(ForwardToP2PModel model)
        {
            var originalMessage = await GetOriginalMessageContent(model.SourceType, model.SourceMessageId);
            if (originalMessage == null)
                return ApiResult.Fail("Tin nh?n g?c không t?n t?i", "MESSAGE_NOT_FOUND");

            var receiver = await _dataContext.Users.FindAsync(model.TargetReceiverId);
            if (receiver == null)
                return ApiResult.Fail("Ng??i nh?n không t?n t?i", "USER_NOT_FOUND");

            var forwardedMessage = new ChatP2P
            {
                SenderId = model.ForwarderId,
                ReceiverId = model.TargetReceiverId,
                Message = originalMessage.Message,
                MessageType = "forwarded",
                ImageUrl = originalMessage.ImageUrl,
                IsForwarded = true,
                ForwardedFromMessageId = model.SourceMessageId,
                ForwardedFromSource = ChatMessageHelper.CreateForwardSource(model.SourceType, model.SourceId),
                CreatedDate = Now
            };

            _dataContext.ChatP2Ps.Add(forwardedMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                forwardedMessage.Id,
                forwardedMessage.SenderId,
                forwardedMessage.ReceiverId,
                forwardedMessage.Message,
                forwardedMessage.MessageType,
                ImageUrls = ChatMessageHelper.ParseImageUrls(forwardedMessage.ImageUrl),
                forwardedMessage.IsForwarded,
                forwardedMessage.ForwardedFromMessageId,
                forwardedMessage.ForwardedFromSource,
                forwardedMessage.CreatedDate,
                OriginalMessage = originalMessage
            }, "?ã forward tin nh?n");
        }

        public async Task<ApiResult> ForwardToBreakroom(ForwardToBreakroomModel model)
        {
            var originalMessage = await GetOriginalMessageContent(model.SourceType, model.SourceMessageId);
            if (originalMessage == null)
                return ApiResult.Fail("Tin nh?n g?c không t?n t?i", "MESSAGE_NOT_FOUND");

            var targetBreakroom = await _dataContext.BreakoutRooms.FindAsync(model.TargetBreakroomId);
            if (targetBreakroom == null)
                return ApiResult.Fail("Phòng con ?ích không t?n t?i", "BREAKROOM_NOT_FOUND");

            var forwardedMessage = new ChatMessageBreakoutRoom
            {
                BreakoutRoomId = model.TargetBreakroomId,
                UserId = model.ForwarderId,
                Message = originalMessage.Message,
                MessageType = "forwarded",
                ImageUrl = originalMessage.ImageUrl,
                IsForwarded = true,
                ForwardedFromMessageId = model.SourceMessageId,
                ForwardedFromSource = ChatMessageHelper.CreateForwardSource(model.SourceType, model.SourceId),
                CreatedDate = Now
            };

            _dataContext.ChatMessageBreakoutRooms.Add(forwardedMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                forwardedMessage.Id,
                forwardedMessage.BreakoutRoomId,
                forwardedMessage.UserId,
                forwardedMessage.Message,
                forwardedMessage.MessageType,
                ImageUrls = ChatMessageHelper.ParseImageUrls(forwardedMessage.ImageUrl),
                forwardedMessage.IsForwarded,
                forwardedMessage.ForwardedFromMessageId,
                forwardedMessage.ForwardedFromSource,
                forwardedMessage.CreatedDate,
                OriginalMessage = originalMessage
            }, "?ã forward tin nh?n");
        }

        public async Task<ApiResult> GetOriginalMessage(string sourceType, int sourceId, int messageId)
        {
            var message = await GetOriginalMessageContent(sourceType, messageId);
            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            return ApiResult.Success(message);
        }

        private async Task<OriginalMessageDto?> GetOriginalMessageContent(string sourceType, int messageId)
        {
            switch (sourceType.ToLower())
            {
                case "room":
                    var roomMessage = await _dataContext.ChatMessages
                        .Where(m => m.Id == messageId && m.DeletedDate == null)
                        .Select(m => new OriginalMessageDto
                        {
                            Id = m.Id,
                            SenderId = m.UserId,
                            Message = m.Message,
                            MessageType = m.MessageType,
                            ImageUrl = m.ImageUrl,
                            CreatedDate = m.CreatedDate,
                            SourceType = "room",
                            SourceId = m.RoomId
                        })
                        .FirstOrDefaultAsync();
                    return roomMessage;

                case "p2p":
                    var p2pMessage = await _dataContext.ChatP2Ps
                        .Where(m => m.Id == messageId && m.DeletedDate == null)
                        .Select(m => new OriginalMessageDto
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            Message = m.Message,
                            MessageType = m.MessageType,
                            ImageUrl = m.ImageUrl,
                            CreatedDate = m.CreatedDate,
                            SourceType = "p2p",
                            SourceId = m.ReceiverId
                        })
                        .FirstOrDefaultAsync();
                    return p2pMessage;

                case "breakroom":
                    var breakroomMessage = await _dataContext.ChatMessageBreakoutRooms
                        .Where(m => m.Id == messageId && m.DeletedDate == null)
                        .Select(m => new OriginalMessageDto
                        {
                            Id = m.Id,
                            SenderId = m.UserId,
                            Message = m.Message,
                            MessageType = m.MessageType,
                            ImageUrl = m.ImageUrl,
                            CreatedDate = m.CreatedDate,
                            SourceType = "breakroom",
                            SourceId = m.BreakoutRoomId
                        })
                        .FirstOrDefaultAsync();
                    return breakroomMessage;

                default:
                    return null;
            }
        }

        private class OriginalMessageDto
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public string Message { get; set; } = string.Empty;
            public string MessageType { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string SourceType { get; set; } = string.Empty;
            public int SourceId { get; set; }
        }
    }
}
