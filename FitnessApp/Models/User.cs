using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            RegistrationDate = DateTime.Now;
            IsActive = true;
        }
    }
}