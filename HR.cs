using System;
using System.ComponentModel.DataAnnotations;

namespace MonthlyClaimSystem.Models
{
    public class HR
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Department")]
        public string Department { get; set; } = "Human Resources";

        [Phone]
        [Display(Name = "Phone number")]
        public string Phone { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Hire date")]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public string Notes { get; set; } = string.Empty;
    }
}