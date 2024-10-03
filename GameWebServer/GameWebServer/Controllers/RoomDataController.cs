using GameWebServer.Data;
using GameWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameWebServer.Controllers
{
    public class RoomDataController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoomDataController(ApplicationDbContext db)
        {
            _db = db;
        }

        public JsonResult Index()
        {
            List<RoomData> roomDataList = _db.roomDatas.ToList();

            string jsonString = JsonConvert.SerializeObject(roomDataList);

            return Json(jsonString);
        }
    }
}
