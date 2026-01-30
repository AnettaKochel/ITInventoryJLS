using System.ComponentModel.DataAnnotations;

namespace ITInventoryJLS.Models
{
    public class Waps
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string Model { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string Condition { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string DeploymentLocation { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string DeviceName { get; set; } = string.Empty;
        
        public DateOnly ReceivedDate { get; set; }

        public DateOnly DeployedDate { get; set; }

        public DateOnly EOL { get; set; }
    }
}