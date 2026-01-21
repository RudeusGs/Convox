using Microsoft.EntityFrameworkCore;
using server.Domain.Entities.Chats;
using server.Infrastructure.Persistence;
using server.Service.Common.IServices;
using server.Service.Interfaces;
using server.Service.Models;
using server.Service.Services;

namespace server.Service.Services.Chats
{
    public class ReactionService : BaseService, IReactionService
    {
        public ReactionService(DataContext dataContext, IUserService userService) 
            : base(dataContext, userService) { }

        #region Room Reactions

        public async Task<ApiResult> ToggleReactionInRoom(int messageId, int userId, string emoji)
        {
            var message = await _dataContext.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            var existingReaction = await _dataContext.MessageReactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.DeletedDate == null);

            if (existingReaction != null)
            {
                if (existingReaction.Emoji == emoji)
                {
                    // Same emoji => remove reaction
                    existingReaction.DeletedDate = Now;
                    await SaveChangesAsync();
                    
                    var reactionsAfterRemove = await GetReactionsSummaryForRoom(messageId);
                    return ApiResult.Success(new
                    {
                        MessageId = messageId,
                        Action = "removed",
                        Emoji = emoji,
                        UserId = userId,
                        Reactions = reactionsAfterRemove
                    }, "?ã g? reaction");
                }
                else
                {
                    // Different emoji => update
                    existingReaction.Emoji = emoji;
                    existingReaction.UpdatedDate = Now;
                    await SaveChangesAsync();
                    
                    var reactionsAfterUpdate = await GetReactionsSummaryForRoom(messageId);
                    return ApiResult.Success(new
                    {
                        MessageId = messageId,
                        Action = "updated",
                        Emoji = emoji,
                        UserId = userId,
                        Reactions = reactionsAfterUpdate
                    }, "?ã c?p nh?t reaction");
                }
            }

            // Add new reaction
            var newReaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji,
                CreatedDate = Now
            };

            _dataContext.MessageReactions.Add(newReaction);
            await SaveChangesAsync();

            var reactions = await GetReactionsSummaryForRoom(messageId);
            return ApiResult.Success(new
            {
                MessageId = messageId,
                Action = "added",
                Emoji = emoji,
                UserId = userId,
                Reactions = reactions
            }, "?ã th? reaction");
        }

        public async Task<ApiResult> GetReactionsForRoomMessage(int messageId)
        {
            var message = await _dataContext.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            var reactions = await GetReactionsSummaryForRoom(messageId);
            var userReactions = await GetUserReactionsForRoom(messageId);

            return ApiResult.Success(new
            {
                MessageId = messageId,
                Reactions = reactions,
                UserReactions = userReactions
            });
        }

        private async Task<List<object>> GetReactionsSummaryForRoom(int messageId)
        {
            return await _dataContext.MessageReactions
                .Where(r => r.MessageId == messageId && r.DeletedDate == null)
                .GroupBy(r => r.Emoji)
                .Select(g => new
                {
                    Emoji = g.Key,
                    Count = g.Count(),
                    UserIds = g.Select(r => r.UserId).ToList()
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> GetUserReactionsForRoom(int messageId)
        {
            return await _dataContext.MessageReactions
                .Where(r => r.MessageId == messageId && r.DeletedDate == null)
                .Select(r => new
                {
                    r.UserId,
                    r.Emoji,
                    r.CreatedDate
                })
                .ToListAsync<object>();
        }

        #endregion

        #region Breakroom Reactions

        public async Task<ApiResult> ToggleReactionInBreakroom(int messageId, int userId, string emoji)
        {
            var message = await _dataContext.ChatMessageBreakoutRooms
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            var existingReaction = await _dataContext.MessageReactionBreakrooms
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.DeletedDate == null);

            if (existingReaction != null)
            {
                if (existingReaction.Emoji == emoji)
                {
                    existingReaction.DeletedDate = Now;
                    await SaveChangesAsync();
                    
                    var reactionsAfterRemove = await GetReactionsSummaryForBreakroom(messageId);
                    return ApiResult.Success(new
                    {
                        MessageId = messageId,
                        Action = "removed",
                        Emoji = emoji,
                        UserId = userId,
                        Reactions = reactionsAfterRemove
                    }, "?ã g? reaction");
                }
                else
                {
                    existingReaction.Emoji = emoji;
                    existingReaction.UpdatedDate = Now;
                    await SaveChangesAsync();
                    
                    var reactionsAfterUpdate = await GetReactionsSummaryForBreakroom(messageId);
                    return ApiResult.Success(new
                    {
                        MessageId = messageId,
                        Action = "updated",
                        Emoji = emoji,
                        UserId = userId,
                        Reactions = reactionsAfterUpdate
                    }, "?ã c?p nh?t reaction");
                }
            }

            var newReaction = new MessageReactionBreakroom
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji,
                CreatedDate = Now
            };

            _dataContext.MessageReactionBreakrooms.Add(newReaction);
            await SaveChangesAsync();

            var reactions = await GetReactionsSummaryForBreakroom(messageId);
            return ApiResult.Success(new
            {
                MessageId = messageId,
                Action = "added",
                Emoji = emoji,
                UserId = userId,
                Reactions = reactions
            }, "?ã th? reaction");
        }

        public async Task<ApiResult> GetReactionsForBreakroomMessage(int messageId)
        {
            var message = await _dataContext.ChatMessageBreakoutRooms
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            var reactions = await GetReactionsSummaryForBreakroom(messageId);
            var userReactions = await GetUserReactionsForBreakroom(messageId);

            return ApiResult.Success(new
            {
                MessageId = messageId,
                Reactions = reactions,
                UserReactions = userReactions
            });
        }

        private async Task<List<object>> GetReactionsSummaryForBreakroom(int messageId)
        {
            return await _dataContext.MessageReactionBreakrooms
                .Where(r => r.MessageId == messageId && r.DeletedDate == null)
                .GroupBy(r => r.Emoji)
                .Select(g => new
                {
                    Emoji = g.Key,
                    Count = g.Count(),
                    UserIds = g.Select(r => r.UserId).ToList()
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> GetUserReactionsForBreakroom(int messageId)
        {
            return await _dataContext.MessageReactionBreakrooms
                .Where(r => r.MessageId == messageId && r.DeletedDate == null)
                .Select(r => new
                {
                    r.UserId,
                    r.Emoji,
                    r.CreatedDate
                })
                .ToListAsync<object>();
        }

        #endregion

        #region P2P Reactions

        public async Task<ApiResult> ToggleReactionP2P(int messageId, int userId, string emoji)
        {
            var message = await _dataContext.ChatP2Ps
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            var existingReaction = await _dataContext.MessageReactionP2Ps
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId && r.DeletedDate == null);

            if (existingReaction != null)
            {
                if (existingReaction.Emoji == emoji)
                {
                    existingReaction.DeletedDate = Now;
                    await SaveChangesAsync();
                    
                    var reactionsAfterRemove = await GetReactionsSummaryForP2P(messageId);
                    return ApiResult.Success(new
                    {
                        MessageId = messageId,
                        Action = "removed",
                        Emoji = emoji,
                        UserId = userId,
                        Reactions = reactionsAfterRemove
                    }, "?ã g? reaction");
                }
                else
                {
                    existingReaction.Emoji = emoji;
                    existingReaction.UpdatedDate = Now;
                    await SaveChangesAsync();
                    
                    var reactionsAfterUpdate = await GetReactionsSummaryForP2P(messageId);
                    return ApiResult.Success(new
                    {
                        MessageId = messageId,
                        Action = "updated",
                        Emoji = emoji,
                        UserId = userId,
                        Reactions = reactionsAfterUpdate
                    }, "?ã c?p nh?t reaction");
                }
            }

            var newReaction = new MessageReactionP2P
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji,
                CreatedDate = Now
            };

            _dataContext.MessageReactionP2Ps.Add(newReaction);
            await SaveChangesAsync();

            var reactions = await GetReactionsSummaryForP2P(messageId);
            return ApiResult.Success(new
            {
                MessageId = messageId,
                Action = "added",
                Emoji = emoji,
                UserId = userId,
                Reactions = reactions
            }, "?ã th? reaction");
        }

        public async Task<ApiResult> GetReactionsForP2PMessage(int messageId)
        {
            var message = await _dataContext.ChatP2Ps
                .FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedDate == null);

            if (message == null)
                return ApiResult.Fail("Tin nh?n không t?n t?i", "MESSAGE_NOT_FOUND");

            var reactions = await GetReactionsSummaryForP2P(messageId);
            var userReactions = await GetUserReactionsForP2P(messageId);

            return ApiResult.Success(new
            {
                MessageId = messageId,
                Reactions = reactions,
                UserReactions = userReactions
            });
        }

        private async Task<List<object>> GetReactionsSummaryForP2P(int messageId)
        {
            return await _dataContext.MessageReactionP2Ps
                .Where(r => r.MessageId == messageId && r.DeletedDate == null)
                .GroupBy(r => r.Emoji)
                .Select(g => new
                {
                    Emoji = g.Key,
                    Count = g.Count(),
                    UserIds = g.Select(r => r.UserId).ToList()
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> GetUserReactionsForP2P(int messageId)
        {
            return await _dataContext.MessageReactionP2Ps
                .Where(r => r.MessageId == messageId && r.DeletedDate == null)
                .Select(r => new
                {
                    r.UserId,
                    r.Emoji,
                    r.CreatedDate
                })
                .ToListAsync<object>();
        }

        #endregion
    }
}
