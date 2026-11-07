using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GameLibrary.Models
{
    public class Games
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public bool IsWishlist { get; set; }
        public int PlaytimeMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        [ForeignKey("User")]
        [Required]
        public int User_Id { get; set; }
        public User User { get; set; }

        [ForeignKey("PlayStatuses")]
        [Required]
        public int PlayStatuses_Id { get; set; }
        public PlayStatuses PlayStatuses { get; set; }
    }
}
