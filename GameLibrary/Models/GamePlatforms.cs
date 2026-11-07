using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLibrary.Models
{
    public class GamePlatforms
    {
        [ForeignKey("Games")]
        [Required]
        public int Game_Id { get; set; }
        public Games Games { get; set; }

        [ForeignKey("Platforms")]
        [Required]
        public int Platform_Id { get; set; }
        public Platforms Platforms { get; set; }
    }
}
