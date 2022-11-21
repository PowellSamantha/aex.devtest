using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aex.devtest.infrastucture.Entities
{
    [Table("vehicles", Schema = "entity")]
    [Index(nameof(Type), nameof(Make), nameof(Model))]
    public class Vehicle
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Type { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public short Year { get; set; }

        [Required]
        public byte WheelCount { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string FuelType { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public float AnnualTaxableLevy { get; set; }

        public byte? RoadworthyTestInterval { get; set; }
    }
}
