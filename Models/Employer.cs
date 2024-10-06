using System;
using System.ComponentModel.DataAnnotations;

namespace Lab1.Models
{
    public class Employer
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Employer Name is required")]
        [StringLength(100, ErrorMessage = "Employer Name cannot be longer than 100 characters")]
        [Display(Name = "Employer Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Website is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Incorporated Date")]
        public DateTime? IncorporatedDate { get; set; }
    }
}
