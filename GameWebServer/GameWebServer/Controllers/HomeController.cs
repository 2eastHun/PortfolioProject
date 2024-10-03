using GameWebServer.Data;
using GameWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;


namespace GameWebServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PlayerData()
        {
            List<PlayerData> playerDataList = _db.playerDatas.ToList();
            ViewBag.playerList = playerDataList;
            ViewBag.playerDataCount = playerDataList.Count();

            return View();

        }

        public IActionResult RoomData()
        {
            List<RoomData> roomDataList = _db.roomDatas.ToList();
            ViewBag.roomList = roomDataList;
            ViewBag.roomDataCount = roomDataList.Count();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
