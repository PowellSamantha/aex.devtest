using aex.devtest.domain.Core;
using Newtonsoft.Json;
using System;

namespace aex.devtest.domain.Vehicle.Models
{
    public class Vehicle
    {
        [CSVProperty(0)]
        public Guid Id { get; set; }

        [CSVProperty(1)]
        public VehicleType Type { get; set; }

        [JsonIgnore]
        public string TypeAsString { get; set; }

        [CSVProperty(3)]
        public string Make { get; set; }

        [CSVProperty(4)]
        public string Model { get; set; }

        [CSVProperty(4)]
        public short Year { get; set; }

        [CSVProperty(5)]
        public byte WheelCount { get; set; }

        [CSVProperty(6)]
        public FuelType FuelType { get; set; }

        [CSVProperty(7)]
        public bool Active { get; set; }

        public float AnnualTaxableLevy { get; set; }

        public byte? RoadworthyTestInterval { get; set; }
    }
}
