using System.ComponentModel.DataAnnotations;

namespace ITInventoryJLS.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string OfficeName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Email { get; set; } = string.Empty;        

        public int? EquipmentID { get; set; }
    }
}