using System.ComponentModel.DataAnnotations;

namespace GameWebServer.Models
{
    public class RoomData
    {
        [Key]
        public int room_id { get; set; }
        [Required]
        public string room_name { get; set; }
        public int player1_id { get; set; }
        public string player1_name { get; set; }
        public int player2_id { get; set; }
        public string player2_name { get; set; }
        public bool is_playing { get; set; }

        public int GetRoomID() {  return room_id; }
    }
}
