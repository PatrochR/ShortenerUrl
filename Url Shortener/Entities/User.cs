using System.ComponentModel.DataAnnotations;

namespace Url_Shortener.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}
