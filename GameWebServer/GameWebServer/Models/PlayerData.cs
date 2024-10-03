using System.ComponentModel.DataAnnotations;

namespace GameWebServer.Models
{
    public class PlayerData
    {
        [Key]
        public int player_id { get; set; }
        [Required]
        public string player_name { get; set; }
        public int server_session { get; set; }
        public bool is_login { get; set; }
    }
}
