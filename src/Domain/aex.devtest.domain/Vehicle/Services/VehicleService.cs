using aex.devtest.domain.CSV.Interfaces;
using aex.devtest.domain.Vehicle.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aex.devtest.domain.CSV.Services
{
    public class VehicleService : IVehicleService
    {
        #region Class Variables
        private readonly ILogger<VehicleService> _logger;
        private readonly IVehicleRepository _vehicleRepository;
        #endregion

        #region Constructor
        public VehicleService(ILogger<VehicleService> logger, IVehicleRepository vehicleRepository)
        {
            _logger = logger;
            _vehicleRepository = vehicleRepository;
        }
        #endregion

        #region Public Members
        public Task UploadCSV(IFormFile file)
        {
            throw new NotImplementedException();

            //CalculateAnnualTaxableLevy(vehicle);
            //CalculateRoadworthyTestInterval(vehicle);
        }

        public List<Vehicle.Models.Vehicle> Search(VehicleFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            filter.Model = filter.Model?.Trim();
            filter.Make = filter.Make?.Trim();

            var query = _vehicleRepository.Query().Where(o => o.Active);

            if(filter.Type != Vehicle.VehicleType.None)
                query = query.Where(o => o.TypeAsString == filter.Type.ToString()); //recommended to add reference lookup entity

            if (!string.IsNullOrEmpty(filter.Make))
                query = query.Where(o => o.Make == filter.Make);

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(o => o.Model == filter.Model);

            return query.OrderBy(o => o.Make).ToList();
        }

        public async Task<Vehicle.Models.Vehicle> Add(VehicleRequest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Type == Vehicle.VehicleType.None)
                throw new ArgumentNullException(nameof(item.Type));

            item.Make = item.Make?.Trim();
            if (string.IsNullOrEmpty(item.Make))
                throw new ArgumentNullException(nameof(item.Make));

            item.Model = item.Model?.Trim();
            if (string.IsNullOrEmpty(item.Model))
                throw new ArgumentNullException(nameof(item.Model));

            if (item.Year < 1800)
                throw new ArgumentOutOfRangeException(nameof(item.Year));

            if (item.WheelCount < 1)
                throw new ArgumentOutOfRangeException(nameof(item.WheelCount));

            if (item.FuelType == Vehicle.FuelType.NotDefined)
                throw new ArgumentNullException(nameof(item.FuelType));

            var vehicle = new Vehicle.Models.Vehicle()
            {
                Type = item.Type,
                Make = item.Make,
                Model = item.Model,
                Year = item.Year,
                WheelCount = item.WheelCount,
                FuelType = item.FuelType,
                Active = true
            };

            CalculateAnnualTaxableLevy(vehicle);
            CalculateRoadworthyTestInterval(vehicle);

            vehicle = await _vehicleRepository.Create(vehicle);

            return vehicle;
        }

        public async Task<Vehicle.Models.Vehicle> Update(VehicleRequest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!item.Id.HasValue || item.Id == Guid.Empty)
                throw new ArgumentNullException(nameof(item.Id));

            if (item.Type == Vehicle.VehicleType.None)
                throw new ArgumentNullException(nameof(item.Type));

            item.Make = item.Make?.Trim();
            if (string.IsNullOrEmpty(item.Make))
                throw new ArgumentNullException(nameof(item.Make));

            item.Model = item.Model?.Trim();
            if (string.IsNullOrEmpty(item.Model))
                throw new ArgumentNullException(nameof(item.Model));

            if (item.Year < 1800)
                throw new ArgumentOutOfRangeException(nameof(item.Year));

            if (item.WheelCount < 1)
                throw new ArgumentOutOfRangeException(nameof(item.WheelCount));

            if (item.FuelType == Vehicle.FuelType.NotDefined)
                throw new ArgumentNullException(nameof(item.FuelType));

            var vehicle = _vehicleRepository.Query().SingleOrDefault(o => o.Id == item.Id && o.Active);
            if (vehicle == null)
                throw new ArgumentException($"Vehicle with id '{item.Id}' does not exist or has been deactivated");

            vehicle.Type = item.Type;
            vehicle.Make = item.Make;
            vehicle.Model = item.Model;
            vehicle.Year = item.Year;
            vehicle.WheelCount = item.WheelCount;
            vehicle.FuelType = item.FuelType;

            CalculateAnnualTaxableLevy(vehicle);
            CalculateRoadworthyTestInterval(vehicle);

            await _vehicleRepository.Update(vehicle);
            return vehicle;
        }

        public async Task Delete(Guid id)
        {
            var vehicle = _vehicleRepository.Query().SingleOrDefault(o => o.Id == id && o.Active);
            if (vehicle == null)
                throw new ArgumentException($"Vehicle with id '{id}' does not exist or has been deactivated");

            await _vehicleRepository.Delete(vehicle);
        }
        #endregion

        #region Private Members
        private void CalculateAnnualTaxableLevy(Vehicle.Models.Vehicle vehicle)
        {
            var fuelTypeFactorPerc = 0;
            if (vehicle.FuelType == Vehicle.FuelType.Petrol) fuelTypeFactorPerc = 20;

            var levi = 0;
            switch(vehicle.Type)
            {
                case Vehicle.VehicleType.Car:
                    levi = 1500;
                    break;

                case Vehicle.VehicleType.Boat:
                    if (vehicle.FuelType == Vehicle.FuelType.Petrol) fuelTypeFactorPerc = 15;
                    levi = 2000;
                    break;

                case Vehicle.VehicleType.Bicycle:
                    levi = 0;
                    break;

                case Vehicle.VehicleType.Plane:
                    levi = 5000;
                    break;

                case Vehicle.VehicleType.Bike:
                    levi = 1000;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(vehicle.Type), $"Type of '{vehicle.Type}' not supported");
            }

            levi = levi + (levi * fuelTypeFactorPerc / 100);
            vehicle.AnnualTaxableLevy = levi;
        }

        private void CalculateRoadworthyTestInterval(Vehicle.Models.Vehicle vehicle)
        {
            switch(vehicle.Type)
            {
                case Vehicle.VehicleType.Car:
                    vehicle.RoadworthyTestInterval = (byte?)(DateTime.Now.Year - vehicle.Year <= 10 ? 2 : 1);
                    break;

                case Vehicle.VehicleType.Bike:
                    vehicle.RoadworthyTestInterval = (byte?)(DateTime.Now.Year - vehicle.Year >= 5 ? 1 : 0.5);
                    break;

                case Vehicle.VehicleType.Boat:
                case Vehicle.VehicleType.Bicycle:
                case Vehicle.VehicleType.Plane:
                    vehicle.RoadworthyTestInterval = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(vehicle.Type), $"Type of '{vehicle.Type}' not supported");
            }
        }
        #endregion
    }
}

