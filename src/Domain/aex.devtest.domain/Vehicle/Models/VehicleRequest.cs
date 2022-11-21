using System;
using System.ComponentModel.DataAnnotations;

namespace aex.devtest.domain.Vehicle.Models
{
    public class VehicleRequest
    {
        public Guid? Id { get; set; }

        [Required]
        public VehicleType Type { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public short Year { get; set; }

        [Required]
        public byte WheelCount { get; set; }

        [Required]
        public FuelType FuelType { get; set; }
    }
}
