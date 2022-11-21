using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aex.devtest.application.Models
{
    public class Fault
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Messasge { get; set; }
        [DefaultValue(null)]
        public int? CodeInternal { get; set; }
    }
}
