using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(100)]
        public string TrainerName { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }

        public virtual Service Service { get; set; }

        [NotMapped]
        public bool CanBook { get; set; }

        [NotMapped]
        public int FreeSpots => MaxParticipants - CurrentParticipants;

        public Schedule()
        {
            CurrentParticipants = 0;
        }
    }
}