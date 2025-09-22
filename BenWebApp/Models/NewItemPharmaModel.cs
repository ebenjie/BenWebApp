using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenWebApp.Models
{
    public class NewItemPharmaModel
    {
        public int Id { get; set; }
        [Required]
        public long ItemCode { get; set; }
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string GenericName { get; set; } = null!;
        [Range(0.1, Double.MaxValue, ErrorMessage ="Amount dapat dako pa sa zero")]
        public Double SellingPrice { get; set; }

        [Required(ErrorMessage = "Small unit is required")]
        public string? SmallUnit { get; set; }
        public string? BigUnit { get; set; } 
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? RequestedBy { get; set; }
        public string IT_Status { get; set; } = "Open";
    }
    public class CodePerDeptModel
    {
        public int Id { get; set; }
        public long Code { get; set; }
        public string Department { get; set; } = null!;
    }
}
