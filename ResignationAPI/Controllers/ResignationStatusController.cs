using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;
using ResignationAPI.Repository.IRepository;
using System.Net;

namespace ResignationAPI.Controllers
{
    [Route("api/resignationStatus")]
    [ApiController]
    public class ResignationStatusController : ControllerBase
    {
        private readonly IResignationRepository _resignationRepository;
        APIResponse _response;
        public ResignationStatusController(IResignationRepository resignationRepository)
        {
            _resignationRepository = resignationRepository;
            _response = new();
        }

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<APIResponse>> Update(string id, ResignationStatusDTO resignUpdateDTO)
        {
            try
            {
                var updateResign = await _resignationRepository.GetAsync(id);

                if (resignUpdateDTO.Status == null)
                {
                    resignUpdateDTO.Status = updateResign.Status;
                }
                if (resignUpdateDTO.RevailingDate == DateTime.MinValue)
                {
                    resignUpdateDTO.RevailingDate = updateResign.RevailingDate;
                }

                if (resignUpdateDTO.ApprovedBY == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Messages = "Please enter the ApprovedBy Id";
                    _response.IsSuccess = false;
                    return _response;
                }
                if (updateResign == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Messages = "Resignation Not Found";
                    return _response;
                }
                updateResign.Status = resignUpdateDTO.Status;
                updateResign.RevailingDate = resignUpdateDTO.RevailingDate;
                updateResign.ApprovedBY = resignUpdateDTO.ApprovedBY;
                updateResign.UpdatedAT = DateTime.Now;

                await _resignationRepository.UpdateAsync(id, updateResign);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Messages = "Updated the Status of Resignation";
                _response.Data = resignUpdateDTO;
                return Ok(_response);
            }
            catch (Exception)
            {

                throw;
            }         
        }
    }
}
