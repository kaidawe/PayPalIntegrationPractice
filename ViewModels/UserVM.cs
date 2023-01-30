using System.ComponentModel.DataAnnotations;

namespace PaypalLab.ViewModels
{
    public class UserVM
    {

        [Key]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
