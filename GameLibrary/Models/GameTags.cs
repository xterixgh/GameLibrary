using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLibrary.Models
{
    public class GameTags
    {
        [ForeignKey("Games")]
        [Required]
        public int Game_Id { get; set; }
        public Games Games { get; set; }

        [ForeignKey("Tags")]
        [Required]
        public int Tag_Id { get; set; }
        public Tags Tags { get; set; }
    }
}
