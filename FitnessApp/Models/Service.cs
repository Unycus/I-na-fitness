using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Duration { get; set; } // в минутах

        [StringLength(50)]
        public string Category { get; set; }

        public string ImageUrl { get; set; }
    }
}