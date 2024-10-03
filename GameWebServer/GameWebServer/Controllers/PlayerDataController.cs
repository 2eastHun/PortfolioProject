using GameWebServer.Data;
using GameWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameWebServer.Controllers
{
    public class PlayerDataController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PlayerDataController(ApplicationDbContext db)
        {
            _db = db;
        }

        public JsonResult Index()   
        {
            //List<PlayerData> playerDataList = _db.playerDatas.ToList();

            //string jsonString = JsonConvert.SerializeObject(playerDataList);

            //return Json(jsonString);

            var playerDataList = _db.playerDatas.ToList();
            return Json(playerDataList);
        }

        public JsonResult GetDataCount()
        {
            return Json(_db.playerDatas.Count());
        }

        public JsonResult GetPlayerByID(int playerID)
        {
            try
            {
                var playerData = _db.playerDatas.FirstOrDefault(n => n.player_id == playerID);

                if (playerData == null)
                {
                    return Json(new { error = "Player not found" });
                }

                return Json(playerData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetPlayerByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { error = "Player name is null or empty" });
                }

                var playerData = _db.playerDatas.FirstOrDefault(n => n.player_name.ToLower() == name.ToLower());

                if (playerData == null)
                {
                    return Json("null");
                }

                return Json(playerData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult RequestLogin([FromBody] PlayerData playerData)
        {
            var checkPlayer = _db.playerDatas.FirstOrDefault(n => n.player_name.ToLower() == playerData.player_name.ToLower());
            //Console.WriteLine(checkPlayer.player_id);
            if (checkPlayer == null)
            { 
                playerData.player_id = _db.playerDatas.Count() + 1;

                _db.playerDatas.Add(playerData);
                _db.SaveChanges();
                return Ok();
            } 
            else
            {
                //_db.playerDatas.Entry(checkPlayer).CurrentValues.SetValues(playerData);
                checkPlayer.server_session = playerData.server_session;
                checkPlayer.is_login = true;
                _db.SaveChanges();
                return Ok();
            }
        }

        [HttpPost]
        public IActionResult RequestLogout([FromBody] int playerID)
        {
            var checkPlayer = _db.playerDatas.FirstOrDefault(n => n.player_id == playerID);

            checkPlayer.server_session = 0;
            checkPlayer.is_login = false;

            _db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult InsertPlayer([FromBody] PlayerData playerData)
        {
            //List<PlayerData> playerDataList = _db.playerDatas.ToList();

            //PlayerData playerData = new PlayerData();

            //playerData.player_id = playerDataList.Count() + 1;
            //playerData.player_name = playerName;
            //playerData.server_session = 0;
            //playerData.is_login = true;

            //_db.playerDatas.Add(playerData);
            //_db.SaveChanges();

            //var playerData = new PlayerData
            //{
            //    player_id = _db.playerDatas.Count() + 1,
            //    player_name = playerName,
            //    server_session = 0,
            //    is_login = true
            //};

            //_db.playerDatas.Add(playerData);
            //_db.SaveChanges();

            //return Ok();

            if (playerData == null)
            {
                return BadRequest("Player data is null.");
            }

            playerData.player_id = _db.playerDatas.Count() + 1;
            //playerData.server_session = 0;
            //playerData.is_login = true;

            _db.playerDatas.Add(playerData);
            _db.SaveChanges();

            return Ok("Player data received successfully.");
        }

        //[HttpPost("InsertPlayer")]
        //public IActionResult InsertPlayer([FromBody] PlayerData playerData)
        //{
        //    if (playerData == null)
        //    {
        //        return BadRequest("Player data is null.");
        //    }

        //    // 데이터 처리 로직 (예: 데이터베이스에 저장)
        //    // 예시로 콘솔에 출력
        //    _db.playerDatas.Add(playerData);
        //    _db.SaveChanges();

        //    return Ok("Player data received successfully.");
        //}
    }
}
