using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class UserService
    {
        [Key]
        public int UserServiceId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }

        // Навигационные свойства
        public virtual User User { get; set; }
        public virtual Service Service { get; set; }

        public UserService()
        {
            PurchaseDate = DateTime.Now;
            IsActive = true;
        }
    }
}