

namespace server.Service.Utilities
{
    public static class ChatMessageHelper
    {
        public static string DetermineMessageType(string? messageContent, List<string>? imageUrls)
        {
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
    }
}
