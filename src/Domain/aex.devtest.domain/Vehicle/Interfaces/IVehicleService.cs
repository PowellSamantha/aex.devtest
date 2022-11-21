using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aex.devtest.domain.CSV.Interfaces
{
    public interface IVehicleService
    {
        public Task<Vehicle.Models.Vehicle> Add(Vehicle.Models.VehicleRequest item);

        public Task Delete(Guid id);

        public Task<Vehicle.Models.Vehicle> Update(Vehicle.Models.VehicleRequest item);

        public List<Vehicle.Models.Vehicle> Search(Vehicle.Models.VehicleFilter filter);

        public Task UploadCSV(IFormFile file);
    }
}
