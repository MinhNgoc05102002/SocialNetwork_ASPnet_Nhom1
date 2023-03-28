using SocialNetwork.Models;

namespace SocialNetwork.ViewModels
{
    public class ChatSessionMessagesViewModel
    {
        SocialNetworkDbContext _context = new SocialNetworkDbContext();
        public List<ChatSession> listChatSessiton { get; set; }
        public List<Message> listMessage { get; set; }
        public Account partner { get;set; }
        public int chatID { get; set; }

        public ChatSessionMessagesViewModel(List<ChatSession> sessions, List<Message> messages, Account account)
        {
            listChatSessiton = sessions;
            listMessage = messages;
            partner = account;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
