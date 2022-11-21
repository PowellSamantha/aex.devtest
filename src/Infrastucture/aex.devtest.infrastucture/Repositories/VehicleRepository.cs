using aex.devtest.domain.CSV.Interfaces;
using aex.devtest.domain.Vehicle;
using aex.devtest.domain.Vehicle.Models;
using aex.devtest.infrastucture.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aex.devtest.infrastucture.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        #region Class Variables
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<Entities.Vehicle> _entitySet;
        #endregion

        #region Constructors
        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
            _entitySet = context.Set<Entities.Vehicle>();
        }
        #endregion

        #region Public Members
        public IQueryable<Vehicle> Query()
        {
            return _context.Vehicle.Select(entity => new Vehicle()
            {
                Id = entity.Id,
                Type = Enum.Parse<VehicleType>(entity.Type, true),
                TypeAsString = entity.Type,
                Make = entity.Make,
                Model = entity.Model,
                Year = entity.Year,
                WheelCount = entity.WheelCount,
                FuelType = Enum.Parse<FuelType>(entity.FuelType, true),
                Active = entity.Active,
                AnnualTaxableLevy = entity.AnnualTaxableLevy,
                RoadworthyTestInterval = entity.RoadworthyTestInterval
            });
        }

        public async Task<Vehicle> Create(Vehicle item)
        {
            var entity = new Entities.Vehicle()
            {
                Id = item.Id,
                Type = item.Type.ToString(),
                Make = item.Make,
                Model = item.Model,
                Year = item.Year,
                WheelCount = item.WheelCount,
                FuelType = item.FuelType.ToString(),
                Active = item.Active,
                AnnualTaxableLevy = item.AnnualTaxableLevy,
                RoadworthyTestInterval = item.RoadworthyTestInterval
            };

            _context.Vehicle.Add(entity);

            await _context.SaveChangesAsync();

            item.Id = entity.Id;
            return item;
        }

        public async Task Update(Vehicle item)
        {
            var entity = _context.Vehicle.Where(o => o.Id == item.Id).SingleOrDefault();

            if (entity == null)
                throw new Exception($"Vehicle with id '{item.Id}' does not exist");

            entity.Type = item.Type.ToString();
            entity.Make = item.Make;
            entity.Model = item.Model;
            entity.Year = item.Year;
            entity.WheelCount = item.WheelCount;
            entity.FuelType = item.FuelType.ToString();
            entity.Active = item.Active;
            entity.AnnualTaxableLevy = item.AnnualTaxableLevy;
            entity.RoadworthyTestInterval = item.RoadworthyTestInterval;
            _context.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(Vehicle item)
        {
            var entity = _context.Vehicle.Where(o => o.Id == item.Id).SingleOrDefault();

            if (entity == null)
                throw new Exception($"Vehicle with id '{item.Id}' does not exist");

            entity.Active = false;
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
