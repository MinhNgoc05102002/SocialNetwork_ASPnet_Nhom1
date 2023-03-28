using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;
using System;
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
            ChatSessionMessagesViewModel chatSessionMessagesViewModel = new ChatSessionMessagesViewModel(
                CurrentAccount.Data.getListChatSession(),
                new List<Message>(), null);
            return View(chatSessionMessagesViewModel);
        }

        [HttpGet]
        [Route("Messages/ChatSession/{chatId:int}")]
        public IActionResult ChatSession(int chatId)
        {
            Account partner = new Account();
            foreach (Account a in GetChatMember(chatId))
            {
                if (a.AccountId != CurrentAccount.account.AccountId)
                {
                    partner = a;
                    break;
                }
            }
            ChatSessionMessagesViewModel chatSessionMessagesViewModel = new ChatSessionMessagesViewModel(
                CurrentAccount.Data.getListChatSession(),
                dbContext.Messages.Where(x => x.ChatId == chatId).ToList(),
                partner
                );
            System.Diagnostics.Debug.WriteLine(GetChatMember(chatId).Count() + " " + partner.AccountId);
            chatSessionMessagesViewModel.chatID = chatId;
            return View(chatSessionMessagesViewModel);
        }

        [HttpGet]
        [Route("Messages/account/{accountId:int}")]
        public IActionResult ChatSessionAccount(int accountId)
        {
            //lấy ra tài khoản đích đang muốn nhắn tin
            Account partner = dbContext.Accounts.SingleOrDefault(x => x.AccountId == accountId);
            foreach (var b in partner.Chats.ToList())
            {
                foreach (var a in CurrentAccount.Data.getListChatSession())
                {
                    if (a.ChatId == b.ChatId)
                    {
                        return ChatSession(a.ChatId);
                    }
                }
            }
            CreateNewChatSession(CurrentAccount.account, partner);

            return ChatSessionAccount(accountId);
        }

        public void CreateNewChatSession(Account currAcc, Account partner)
        {
            ChatSession tmp = new ChatSession();
            tmp.Name = currAcc.FullName + ", " + partner.FullName;
               
            dbContext.ChatSessions.Add(tmp);
            dbContext.SaveChanges();
            
            int newChatId = dbContext.ChatSessions.Max(x => x.ChatId);
            tmp = dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == newChatId);
            tmp.Accounts.Add(currAcc);
            tmp.Accounts.Add(partner);
            
            currAcc.Chats.Add(tmp);
            partner.Chats.Add(tmp);
            dbContext.SaveChanges();
        }

        [HttpPost]
        public IActionResult SendMessage(string mess, int chatID)
        {
            Message message = new Message();
            message.ChatId = chatID;
            message.MessageContent = mess;
            message.CreateAt = DateTime.Now;
            message.AccountId = chatID;

            dbContext.Messages.Add(message);
            dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == chatID).Messages.Add(message);
            //CurrentAccount.account.Messages.Add(message);
            dbContext.Accounts.SingleOrDefault(x => x.AccountId == CurrentAccount.account.AccountId).Messages.Add(message);
            
            dbContext.SaveChanges();
            return RedirectToAction("ChatSession", new {chatID = chatID });
        }

        [HttpGet]
        [Route("/Messages/DeleteChatSession/{chatID}")]
        public IActionResult DeleteChatSession(int chatID)
        {
            dbContext.Messages.RemoveRange(dbContext.Messages.Where(x => x.ChatId == chatID));
            dbContext.ChatSessions.Remove(dbContext.ChatSessions.SingleOrDefault(x => x.ChatId==chatID));
            dbContext.SaveChanges();
            return RedirectToAction("ChatSession");
        }

        public List<Account> GetChatMember(int chatID)
        {
            ChatSession chat = dbContext.ChatSessions.SingleOrDefault(x => x.ChatId == chatID);
            
            return dbContext.ChatSessions.Where(x => x.ChatId == chatID).SelectMany(x=>x.Accounts).ToList();
        }
    }
}
