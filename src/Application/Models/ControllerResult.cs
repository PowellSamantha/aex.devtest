using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aex.devtest.application.Models
{
    public class ControllerResult
    {
        [Required]
        [DefaultValue("(int)HttpStatusCode")]
        public int Status { get; set; }
        public List<Fault> Faults { get; set; }
    }

    public class ControllerResult<T> : ControllerResult
    {
        public T Data { get; set; }
    }
}
