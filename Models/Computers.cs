using System.ComponentModel.DataAnnotations;

namespace ITInventoryJLS.Models
{
    public class Computer
    {
        [Key]
        public int DeviceID { get; set; }

        [Required, StringLength(50)]
        public string DeviceName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string OSVersionNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string OSVersion { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string DeploymentLocation { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string PrimaryUserEmailAddress { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string PrimaryUserDisplayName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string ManagedBy { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string JoinType { get; set; } = string.Empty;

        public DateOnly PurchasedDate { get; set; }

        public DateOnly DeployedDate { get; set; }

        [Required, StringLength(50)]
        public string DeviceStatus { get; set; } = string.Empty;
    }
}
