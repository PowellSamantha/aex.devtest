using aex.devtest.domain.Core;
using aex.devtest.domain.CSV.Interfaces;
using aex.devtest.domain.Vehicle.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace aex.devtest.domain.CSV.Services
{
    public class VehicleService : IVehicleService
    {
        #region Class Variables
        private readonly ILogger<VehicleService> _logger;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMemoryCache _memoryCache;
        #endregion

        #region Constructor
        public VehicleService(ILogger<VehicleService> logger, IVehicleRepository vehicleRepository, IMemoryCache memoryCache)
        {
            _logger = logger;
            _vehicleRepository = vehicleRepository;
            _memoryCache = memoryCache;
        }
        #endregion

        #region Public Members
        public async Task UploadCSV(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var vehicles = new List<Vehicle.Models.Vehicle>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var props = CSVProperties();
                    if (values.Count() != props.Count())
                        throw new Exception($"Line values vs. expected properties mismatch");

                    var vehicle = new Vehicle.Models.Vehicle();

                    for (int i = 0; i < values.Count(); i++)
                    {
                        if (props[i].PropertyType == typeof(Guid))
                            props[i].SetValue(vehicle, Guid.Parse(values[i].ToString()));
                        else if (props[i].PropertyType.IsEnum)
                            props[i].SetValue(vehicle, Enum.Parse(props[i].PropertyType, values[i].ToString(), true));
                        else if (props[i].PropertyType == typeof(short))
                            props[i].SetValue(vehicle, short.Parse(values[i].ToString()));
                        else if (props[i].PropertyType == typeof(byte))
                            props[i].SetValue(vehicle, byte.Parse(values[i].ToString()));
                        else if (props[i].PropertyType == typeof(bool))
                            props[i].SetValue(vehicle, bool.Parse(values[i].ToString()));
                        else if (props[i].PropertyType == typeof(string))
                            props[i].SetValue(vehicle, values[i].ToString());
                        else
                            throw new Exception($"Property of type '{props[i].PropertyType.Name}' not supported");
                    }

                    CalculateAnnualTaxableLevy(vehicle);
                    CalculateRoadworthyTestInterval(vehicle);

                    vehicles.Add(vehicle);
                }
            }

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in vehicles)
                {
                    var isNew = false;
                    var vehicle = _vehicleRepository.Query().SingleOrDefault(o => o.Id == item.Id);
                    if (vehicle == null)
                    {
                        vehicle = new Vehicle.Models.Vehicle();
                        isNew = true;
                    }

                    vehicle.Id = item.Id;
                    vehicle.Type = item.Type;
                    vehicle.Make = item.Make;
                    vehicle.Model = item.Model;
                    vehicle.Year = item.Year;
                    vehicle.WheelCount = item.WheelCount;
                    vehicle.FuelType = item.FuelType;
                    vehicle.Active = item.Active;

                    if (isNew)
                        await _vehicleRepository.Create(vehicle);
                    else
                        await _vehicleRepository.Update(vehicle);
                }
                scope.Complete();
            }
        }

        public List<Vehicle.Models.Vehicle> Search(VehicleFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            filter.Model = filter.Model?.Trim();
            filter.Make = filter.Make?.Trim();

            var query = _vehicleRepository.Query().Where(o => o.Active);

            if (filter.Type != Vehicle.VehicleType.None)
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
            switch (vehicle.Type)
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
            switch (vehicle.Type)
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

        private List<PropertyInfo> CSVProperties()
        {
            var result = _memoryCache.GetOrCreate("VehicleCSVProperties", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return typeof(Vehicle.Models.Vehicle).GetProperties().Where(p => Attribute.IsDefined(p, typeof(CSVPropertyAttribute))).OrderBy(p => ((CSVPropertyAttribute)p.GetCustomAttributes(typeof(CSVPropertyAttribute), false).Single()).Order).ToList();
            });

            return result;
        }
        #endregion
    }
}

