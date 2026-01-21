using Microsoft.EntityFrameworkCore;
using server.Domain.Entities.Chats;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Services;
using server.Service.Utilities;

namespace server.Service.Services.Chats
{
    public class PinnedMessageService : BaseService, IPinnedMessageService
    {
        public PinnedMessageService(DataContext dataContext, IUserService userService) 
            : base(dataContext, userService) { }

        #region Room Pinned Messages

        public async Task<ApiResult> PinMessageInRoom(int roomId, int messageId, int userId)
        {
            var message = await _dataContext.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.RoomId == roomId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i trong phòng này", "MESSAGE_NOT_FOUND");

            var existingPin = await _dataContext.PinnedMessages
                .FirstOrDefaultAsync(p => p.MessageId == messageId && p.RoomId == roomId && p.DeletedDate == null);

            if (existingPin != null)
                return ApiResult.Fail("Tin nh?n ?ã ???c ghim", "ALREADY_PINNED");

            var pinnedMessage = new PinnedMessage
            {
                RoomId = roomId,
                MessageId = messageId,
                PinnedByUserId = userId,
                CreatedDate = Now
            };

            _dataContext.PinnedMessages.Add(pinnedMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                pinnedMessage.Id,
                pinnedMessage.RoomId,
                pinnedMessage.MessageId,
                pinnedMessage.PinnedByUserId,
                pinnedMessage.CreatedDate,
                Message = new
                {
                    message.Id,
                    message.UserId,
                    message.Message,
                    message.MessageType,
                    ImageUrls = ChatMessageHelper.ParseImageUrls(message.ImageUrl),
                    message.CreatedDate
                }
            }, "?ã ghim tin nh?n");
        }

        public async Task<ApiResult> UnpinMessageInRoom(int roomId, int messageId, int userId)
        {
            var pinnedMessage = await _dataContext.PinnedMessages
                .FirstOrDefaultAsync(p => p.MessageId == messageId && p.RoomId == roomId && p.DeletedDate == null);

            if (pinnedMessage == null)
                return ApiResult.Fail("Tin nh?n ch?a ???c ghim", "NOT_PINNED");

            pinnedMessage.DeletedDate = Now;
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                MessageId = messageId,
                RoomId = roomId,
                UnpinnedByUserId = userId
            }, "?ã g? ghim tin nh?n");
        }

        public async Task<ApiResult> GetPinnedMessagesInRoom(int roomId)
        {
            var room = await _dataContext.Rooms.FindAsync(roomId);
            if (room == null)
                return ApiResult.Fail("Phòng không t?n t?i", "ROOM_NOT_FOUND");

            var pinnedMessages = await _dataContext.PinnedMessages
                .Where(p => p.RoomId == roomId && p.DeletedDate == null)
                .OrderByDescending(p => p.CreatedDate)
                .Join(
                    _dataContext.ChatMessages.Where(m => m.DeletedDate == null),
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
                PinnedMessages = pinnedMessages,
                TotalPinned = pinnedMessages.Count
            });
        }

        #endregion

        #region Breakroom Pinned Messages

        public async Task<ApiResult> PinMessageInBreakroom(int breakroomId, int messageId, int userId)
        {
            var message = await _dataContext.ChatMessageBreakoutRooms
                .FirstOrDefaultAsync(m => m.Id == messageId && m.BreakoutRoomId == breakroomId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i trong phòng con này", "MESSAGE_NOT_FOUND");

            var existingPin = await _dataContext.PinnedMessageBreakrooms
                .FirstOrDefaultAsync(p => p.MessageId == messageId && p.BreakroomId == breakroomId && p.DeletedDate == null);

            if (existingPin != null)
                return ApiResult.Fail("Tin nh?n ?ã ???c ghim", "ALREADY_PINNED");

            var pinnedMessage = new PinnedMessageBreakroom
            {
                BreakroomId = breakroomId,
                MessageId = messageId,
                PinnedByUserId = userId,
                CreatedDate = Now
            };

            _dataContext.PinnedMessageBreakrooms.Add(pinnedMessage);
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                pinnedMessage.Id,
                pinnedMessage.BreakroomId,
                pinnedMessage.MessageId,
                pinnedMessage.PinnedByUserId,
                pinnedMessage.CreatedDate,
                Message = new
                {
                    message.Id,
                    message.UserId,
                    message.Message,
                    message.MessageType,
                    ImageUrls = ChatMessageHelper.ParseImageUrls(message.ImageUrl),
                    message.CreatedDate
                }
            }, "?ã ghim tin nh?n");
        }

        public async Task<ApiResult> UnpinMessageInBreakroom(int breakroomId, int messageId, int userId)
        {
            var pinnedMessage = await _dataContext.PinnedMessageBreakrooms
                .FirstOrDefaultAsync(p => p.MessageId == messageId && p.BreakroomId == breakroomId && p.DeletedDate == null);

            if (pinnedMessage == null)
                return ApiResult.Fail("Tin nh?n ch?a ???c ghim", "NOT_PINNED");

            pinnedMessage.DeletedDate = Now;
            await SaveChangesAsync();

            return ApiResult.Success(new
            {
                MessageId = messageId,
                BreakroomId = breakroomId,
                UnpinnedByUserId = userId
            }, "?ã g? ghim tin nh?n");
        }

        public async Task<ApiResult> GetPinnedMessagesInBreakroom(int breakroomId)
        {
            var breakroom = await _dataContext.BreakoutRooms.FindAsync(breakroomId);
            if (breakroom == null)
                return ApiResult.Fail("Phòng con không t?n t?i", "BREAKROOM_NOT_FOUND");

            var pinnedMessages = await _dataContext.PinnedMessageBreakrooms
                .Where(p => p.BreakroomId == breakroomId && p.DeletedDate == null)
                .OrderByDescending(p => p.CreatedDate)
                .Join(
                    _dataContext.ChatMessageBreakoutRooms.Where(m => m.DeletedDate == null),
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
                BreakroomId = breakroomId,
                PinnedMessages = pinnedMessages,
                TotalPinned = pinnedMessages.Count
            });
        }

        #endregion
    }
}
