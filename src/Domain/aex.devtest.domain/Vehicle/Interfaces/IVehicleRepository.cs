using System.Linq;
using System.Threading.Tasks;

namespace aex.devtest.domain.CSV.Interfaces
{
    public interface IVehicleRepository
    {
        IQueryable<Vehicle.Models.Vehicle> Query();
        Task<Vehicle.Models.Vehicle> Create(Vehicle.Models.Vehicle item);
        Task Update(Vehicle.Models.Vehicle item);
        Task Delete(Vehicle.Models.Vehicle item);
    }
}
