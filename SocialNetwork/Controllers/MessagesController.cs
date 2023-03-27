using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using System.Linq;

namespace SocialNetwork.Controllers
{
    
    public class MessagesController : Controller
    {
        SocialNetworkDbContext dbContext = new SocialNetworkDbContext();
        private readonly ILogger<MessagesController> _logger;
        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }
        
        [Route("Messages")]
        public IActionResult Index()
        {
            // Nhan vao id cuoc tro chuyen, neu co thi luu id vao TempData roi chuyen sang trang ChatSession
            // de hien thi cuoc hoi thoai cu the (lam cach nay de khong hien duong dan cu the tren thanh)
            // ...

            return RedirectToAction("ChatSession", "Messages");
        }

        [HttpGet]
        [Route("Messages/ChatSession")]
        public IActionResult ChatSession()
        {
            // Kiem tra tempdata xem co luu id cua chat session khong, neu co thi render ra lich su tin nhan
            // Nhan vao id cuoc tro chuyen, chuyen toi trang message cua cuoc tro chuyen do
            return View(CurrentAccount.Data.getListChatSession());
        }

        [HttpGet]
        [Route("Messages/ChatSession/{chatId:int}")]
        public IActionResult ChatSession(int chatId)
        {
            ChatSessionMessagesViewModel chatSessionMessagesViewModel = new ChatSessionMessagesViewModel(
                CurrentAccount.Data.getListChatSession(),
                dbContext.Messages.Where(x => x.ChatId == chatId).ToList());
            // Kiem tra tempdata xem co luu id cua chat session khong, neu co thi render ra lich su tin nhan
            // Nhan vao id cuoc tro chuyen, chuyen toi trang message cua cuoc tro chuyen do
            return View(chatSessionMessagesViewModel);
        }

        [HttpGet]
        [Route("Messages/account/{accountId:int}")]
        public IActionResult ChatSessionAccount(int accountId)
        {
            //lấy ra tài khoản đích đang muốn nhắn tin
            Account chatAccount = dbContext.Accounts.SingleOrDefault(x => x.AccountId == accountId);
            foreach (var b in chatAccount.Chats.ToList())
            {
                foreach (var a in CurrentAccount.Data.getListChatSession())
                {
                    if (a.ChatId == b.ChatId)
                    {
                        RedirectToAction("ChatSession/" + a.ChatId, "Messages");
                        return null;
                    }
                }
            }

            CreateNewChatSession(CurrentAccount.account, chatAccount);
            // Kiem tra tempdata xem co luu id cua chat session khong, neu co thi render ra lich su tin nhan
            // Nhan vao id cuoc tro chuyen, chuyen toi trang message cua cuoc tro chuyen do
            return Redirect(accountId.ToString());
        }

        public void CreateNewChatSession(Account sender, Account receiver)
        {
            ChatSession tmp = new ChatSession();
            tmp.Name = sender.FullName + ", " + receiver.FullName;
               
            dbContext.ChatSessions.Add(tmp);
            dbContext.SaveChanges();
            
            int newChatId = dbContext.ChatSessions.Max(x => x.ChatId);
            tmp = dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == newChatId);
            sender.Chats.Add(tmp);
            receiver.Chats.Add(tmp);
            dbContext.SaveChanges();
        }
    }
}
