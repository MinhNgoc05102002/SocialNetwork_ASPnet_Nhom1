using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class ChatSessionMessagesViewModel
    {
        SocialNetworkDbContext _context = new SocialNetworkDbContext();
        public List<ChatSession> listChatSessiton { get; set; }
        public List<Message> listMessage { get; set; }

        public ChatSessionMessagesViewModel(List<ChatSession> sessions, List<Message> messages)
        {
            listChatSessiton = sessions;
            listMessage = messages;
        }

    }
}
