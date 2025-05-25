using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
} 