namespace server.Service.Models.Chats
{
    public class SendRoomMessageWithImagesModel : SendMessageWithImagesModel
    {
        public int RoomId { get; set; }
    }
}
