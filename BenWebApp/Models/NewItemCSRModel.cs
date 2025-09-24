using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenWebApp.Models
{
    [Table("NewItemCSR")]
    public class NewItemCSRModel
    {
        public int Id { get; set; }
        [Required]
        public long ItemCode { get; set; }
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string ItemCategory { get; set; } = null!;
        public int? Conversion { get; set; } 
        [Range(0.1, Double.MaxValue, ErrorMessage = "Amount dapat dako pa sa zero")]
        public Double UnitCost { get; set; }

        [Required(ErrorMessage = "Small unit is required")]
        public string? SmallUnit { get; set; }
        public string? BigUnit { get; set; }
        public int MarkUp { get; set; }
        [Range(0.1, Double.MaxValue, ErrorMessage = "Amount dapat dako pa sa zero")]
        public Double SellingPrice { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? RequestedBy { get; set; }
        public string IT_Status { get; set; } = "Open";
    }
}
