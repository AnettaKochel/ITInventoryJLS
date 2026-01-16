using System.ComponentModel.DataAnnotations;

namespace ITInventoryJLS.Models
{
    public class Location
    {
        public int LocationId { get; set; }

        [Required, StringLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LocationAccountingID { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string StreetAddress { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string CityStateZip { get; set; } = string.Empty;

        [StringLength(50)]
        public string FWIP { get; set; } = string.Empty;

    }
}