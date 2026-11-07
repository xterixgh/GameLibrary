using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLibrary.Models
{
    public class Notes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreateAt { get; set; }

        [ForeignKey("Games")]
        [Required]
        public int Game_Id { get; set; }
        public Games Games { get; set; }
    }
}
