using System.ComponentModel.DataAnnotations;

namespace ITInventoryJLS.Models
{
    public class DBUser
    {
        public int DBUserID { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string EmailAddress { get; set; } = string.Empty;

        // PBKDF2/Argon2/etc. hashes are stored as Base64 strings. Use generous lengths.
        [Required, StringLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string PasswordSalt { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required, StringLength(50)]
        public string UserRights { get; set; } = string.Empty;  
    }
}