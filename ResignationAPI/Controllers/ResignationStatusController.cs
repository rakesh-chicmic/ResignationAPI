using Microsoft.AspNetCore.Authorization;
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
        private readonly ILoggingRepository _loggingRepository;
        APIResponse _response;
        public ResignationStatusController(IResignationRepository resignationRepository, ILoggingRepository loggingRepository)
        {
            _resignationRepository = resignationRepository;
            _loggingRepository = loggingRepository;
            _response = new();
        }

        [HttpPut("{id:length(24)}"),Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Update(string id, ResignationStatusDTO resignUpdateDTO)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "_id")?.Value;
            try
            {
                var updateResign = await _resignationRepository.GetByIdAsync(id);
                if (updateResign == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Resignation Not Found";
                    return _response;
                }

                if (resignUpdateDTO.Status == null)
                {
                    resignUpdateDTO.Status = updateResign.Status;
                }
                if (resignUpdateDTO.RevealingDate == DateTime.MinValue)
                {
                    resignUpdateDTO.RevealingDate = updateResign.RevealingDate;
                }
                else
                {
                    if (resignUpdateDTO.RevealingDate != updateResign.RevealingDate)
                    {
                        if (resignUpdateDTO.RevealingDate < DateTime.Now)
                        {
                            _response.Message = "Please Enter the valid date";
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            return _response;
                        }
                    }
                }

                // updated the details
                updateResign.Status = resignUpdateDTO.Status;
                updateResign.RevealingDate = resignUpdateDTO.RevealingDate;
                updateResign.ApprovedBy = userId;
                updateResign.UpdatedAt = DateTime.Now;

                // update status service called
                await _resignationRepository.UpdateAsync(id, updateResign);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Updated the Status of Resignation";
                _response.Data = resignUpdateDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // save the error in log
                _loggingRepository.LogError(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = "Error retrieving data from the database. Check the logs";
                _response.Status = false;
                return _response;
            }         
        }
    }
}
