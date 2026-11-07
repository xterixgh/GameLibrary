using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLibrary.Models
{
    public class GameGenres
    {
        [ForeignKey("Games")]
        [Required]
        public int Game_Id { get; set; }
        public Games Games { get; set; }

        [ForeignKey("Genres")]
        [Required]
        public int Genre_Id { get; set; }
        public Genres Genres { get; set; }
    }
}
