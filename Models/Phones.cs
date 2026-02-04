using System.ComponentModel.DataAnnotations;

namespace ITInventoryJLS.Models
{
    public class Phone
    {
        [Key]
        public int PhoneID { get; set; }

        [Required, StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string Model { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string Condition { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string MACAddress { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string DeploymentLocation { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string UserAssigned { get; set; } = string.Empty;
        
    }
}