using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    public class MessagesController : Controller
    {
        DBconte
        public IActionResult Index()
        {
            // Nhan vao id cuoc tro chuyen, neu co thi luu id vao TempData roi chuyen sang trang ChatSession
            // de hien thi cuoc hoi thoai cu the (lam cach nay de khong hien duong dan cu the tren thanh)
            // ...

            return RedirectToAction("ChatSession", "Messages");
        }

        [HttpGet]
        public IActionResult ChatSession()
        {
            int id = 1;
            List<ChatSession> chatSessions = ;
            // Kiem tra tempdata xem co luu id cua chat session khong, neu co thi render ra lich su tin nhan
            // Nhan vao id cuoc tro chuyen, chuyen toi trang message cua cuoc tro chuyen do
            return View();
        }
    }
}
