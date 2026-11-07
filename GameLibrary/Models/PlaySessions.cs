using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLibrary.Models
{
    public class PlaySessions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public int Minutes { get; set; }

        public DateTime? PlayedOn { get; set; }

        public string Comment { get; set; }

        public DateTime CreateAt { get; set; }

        [ForeignKey("Games")]
        [Required]
        public int Game_Id { get; set; }
        public Games Games { get; set; }
    }
}
