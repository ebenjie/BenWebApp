using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenWebApp.Models
{
    public class CodePerDeptModel
    {
       
            public int Id { get; set; }
            public long Code { get; set; }
            public string Department { get; set; } = null!;
       
    }
}
