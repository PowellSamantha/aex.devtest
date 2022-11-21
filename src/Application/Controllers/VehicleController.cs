using aex.devtest.application.Models;
using aex.devtest.domain.CSV.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace aex.devtest.Controllers
{
    [ApiController]
    [Route("api/v1/vehicles")]
    public class VehicleController : ControllerBase
    {
        #region Class Variables
        private readonly ILogger<VehicleController> _logger;
        private readonly IVehicleService _csvService;
        #endregion

        #region Constructor
        public VehicleController(ILogger<VehicleController> logger, IVehicleService csvService)
        {
            _logger = logger;
            _csvService = csvService;
        }
        #endregion

        #region Public Members
        [SwaggerOperation(Summary = "Search vehicles based on the supplied filter")]
        [HttpPost("search")]
        [ProducesResponseType(typeof(ControllerResult<List<domain.Vehicle.Models.Vehicle>>), (int)HttpStatusCode.OK)]
        public IActionResult Search([FromBody] domain.Vehicle.Models.VehicleFilter filter)
        {
            _logger.LogInformation($"Handling request {nameof(Search)} ({nameof(filter)}: {filter})");

            var result = _csvService.Search(filter);

            _logger.LogInformation($"Request {nameof(Search)} handled: Response ({result})");

            return StatusCode((int)HttpStatusCode.OK, new ControllerResult<List<domain.Vehicle.Models.Vehicle>>() { Data = result, Status = (int)HttpStatusCode.OK });
        }

        [SwaggerOperation(Summary = "Add a vehicle")]
        [HttpPost("")]
        [ProducesResponseType(typeof(ControllerResult<domain.Vehicle.Models.Vehicle>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddVehicle([FromBody] domain.Vehicle.Models.VehicleRequest item)
        {
            _logger.LogInformation($"Handling request {nameof(AddVehicle)} ({nameof(item)}: {item})");

            var result = await _csvService.Add(item);

            _logger.LogInformation($"Request {nameof(AddVehicle)} handled: Response ({result})");

            return StatusCode((int)HttpStatusCode.OK, new ControllerResult<domain.Vehicle.Models.Vehicle>() { Data = result, Status = (int)HttpStatusCode.OK });
        }

        [SwaggerOperation(Summary = "Update a vehicle")]
        [HttpPost("update")]
        [ProducesResponseType(typeof(ControllerResult<domain.Vehicle.Models.Vehicle>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateVehicle([FromBody] domain.Vehicle.Models.VehicleRequest item)
        {
            _logger.LogInformation($"Handling request {nameof(UpdateVehicle)} ({nameof(item)}: {item})");

            var result = await _csvService.Update(item);

            _logger.LogInformation($"Request {nameof(UpdateVehicle)} handled: Response ({result})");

            return StatusCode((int)HttpStatusCode.OK, new ControllerResult<domain.Vehicle.Models.Vehicle>() { Data = result, Status = (int)HttpStatusCode.OK });
        }

        [SwaggerOperation(Summary = "Delete / deactivate a vehicle by id")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ControllerResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteVehicle([FromRoute] Guid id)
        {
            _logger.LogInformation($"Handling request {nameof(DeleteVehicle)} ({nameof(id)}: {id})");

            await _csvService.Delete(id);

            _logger.LogInformation($"Request {nameof(DeleteVehicle)} handled");

            return StatusCode((int)HttpStatusCode.OK, new ControllerResult() { Status = (int)HttpStatusCode.OK });
        }

        [SwaggerOperation(Summary = "Upload CSV file / data")]
        [HttpPost("upload")]
        [ProducesResponseType(typeof(ControllerResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadCSV([Required] IFormFile file)
        {
            _logger.LogInformation($"Handling request {nameof(UploadCSV)} ({file.Name})");

            await _csvService.UploadCSV(file);

            _logger.LogInformation($"Request {nameof(UploadCSV)} handled");

            return StatusCode((int)HttpStatusCode.NotFound, new ControllerResult() { Status = (int)HttpStatusCode.OK });
        }
        #endregion
    }
}