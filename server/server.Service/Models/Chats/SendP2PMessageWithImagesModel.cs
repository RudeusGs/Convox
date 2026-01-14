namespace server.Service.Models.Chats
{
    public class SendP2PMessageWithImagesModel : SendMessageWithImagesModel
    {
        public int ReceiverId { get; set; }
    }

}
