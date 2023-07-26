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
        APIResponse _response;
        public ResignationStatusController(IResignationRepository resignationRepository)
        {
            _resignationRepository = resignationRepository;
            _response = new();
        }

        [HttpPut("{id:length(24)}"),Authorize]
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
            catch (Exception)
            {
                throw;
            }         
        }
    }
}
