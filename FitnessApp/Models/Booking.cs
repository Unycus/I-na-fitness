using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }

        public DateTime BookingDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        // Навигационные свойства
        public virtual User User { get; set; }
        public virtual Schedule Schedule { get; set; }

        public Booking()
        {
            BookingDate = DateTime.Now;
            Status = "Active";
        }
    }
}