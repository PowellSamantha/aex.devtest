using Newtonsoft.Json;
using System;

namespace aex.devtest.domain.Vehicle.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }

        public VehicleType Type { get; set; }

        [JsonIgnore]
        public string TypeAsString { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public short Year { get; set; }

        public byte WheelCount { get; set; }

        public FuelType FuelType { get; set; }

        public bool Active { get; set; }

        public float AnnualTaxableLevy { get; set; }

        public byte? RoadworthyTestInterval { get; set; }
    }
}
