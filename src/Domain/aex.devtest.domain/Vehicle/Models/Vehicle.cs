using aex.devtest.domain.Core;
using Newtonsoft.Json;
using System;

namespace aex.devtest.domain.Vehicle.Models
{
    public class Vehicle
    {
        [Order(0)]
        public Guid Id { get; set; }

        [Order(1)]
        public VehicleType Type { get; set; }

        [JsonIgnore]
        public string TypeAsString { get; set; }

        [Order(3)]
        public string Make { get; set; }

        [Order(4)]
        public string Model { get; set; }

        [Order(4)]
        public short Year { get; set; }

        [Order(5)]
        public byte WheelCount { get; set; }

        [Order(6)]
        public FuelType FuelType { get; set; }

        [Order(7)]
        public bool Active { get; set; }

        public float AnnualTaxableLevy { get; set; }

        public byte? RoadworthyTestInterval { get; set; }
    }
}
