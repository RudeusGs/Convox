

using System.Text.RegularExpressions;

namespace server.Service.Utilities
{
    public static class ChatMessageHelper
    {
        public static string DetermineMessageType(string? messageContent, List<string>? imageUrls, bool isForwarded = false)
        {
            if (isForwarded) return "forwarded";
            
            bool hasText = !string.IsNullOrWhiteSpace(messageContent);
            bool hasImages = imageUrls != null && imageUrls.Count > 0;

            if (hasImages && hasText) return "mixed";
            if (hasImages) return "image";
            if (IsEmoji(messageContent)) return "emoji";
            return "text";
        }

        public static bool IsEmoji(string? text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            return text.Length <= 8 && text.All(c =>
                char.IsSurrogate(c) ||
                (c >= 0x1F600 && c <= 0x1F64F) ||
                (c >= 0x1F300 && c <= 0x1F5FF));
        }

        public static List<string> ParseImageUrls(string? imageUrlString)
        {
            if (string.IsNullOrWhiteSpace(imageUrlString))
                return new List<string>();

            return imageUrlString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(url => url.Trim())
                .Where(url => !string.IsNullOrEmpty(url))
                .ToList();
        }

        public static string? JoinImageUrls(List<string>? imageUrls)
        {
            if (imageUrls == null || imageUrls.Count == 0) return null;
            return string.Join(",", imageUrls);
        }

        #region Mention Helpers

        /// <summary>
        /// Parse message content to extract mentioned user IDs
        /// Format: @[userId] or @userId
        /// Example: "Hello @123 and @[456]" => [123, 456]
        /// </summary>
        public static List<int> ParseMentions(string? messageContent)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
                return new List<int>();

            var mentions = new List<int>();
            
            // Match @[123] or @123
            var regex = new Regex(@"@\[?(\d+)\]?", RegexOptions.Compiled);
            var matches = regex.Matches(messageContent);

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int userId))
                {
                    if (!mentions.Contains(userId))
                        mentions.Add(userId);
                }
            }

            return mentions;
        }

        /// <summary>
        /// Join mention user IDs to comma-separated string
        /// </summary>
        public static string? JoinMentionedUserIds(List<int>? userIds)
        {
            if (userIds == null || userIds.Count == 0) return null;
            return string.Join(",", userIds);
        }

        /// <summary>
        /// Parse comma-separated user IDs string to list
        /// </summary>
        public static List<int> ParseMentionedUserIds(string? mentionedUserIds)
        {
            if (string.IsNullOrWhiteSpace(mentionedUserIds))
                return new List<int>();

            return mentionedUserIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out int id) ? id : 0)
                .Where(id => id > 0)
                .ToList();
        }

        #endregion

        #region Forward Source Helpers

        /// <summary>
        /// Create forward source string
        /// </summary>
        public static string CreateForwardSource(string type, int id)
        {
            return $"{type}_{id}"; // "room_1", "p2p_2", "breakroom_3"
        }

        /// <summary>
        /// Parse forward source to get type and id
        /// </summary>
        public static (string Type, int Id) ParseForwardSource(string? source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return ("unknown", 0);

            var parts = source.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[1], out int id))
                return (parts[0], id);

            return ("unknown", 0);
        }

        #endregion
    }
}
