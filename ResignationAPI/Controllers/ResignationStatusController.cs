﻿using Microsoft.AspNetCore.Authorization;
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
                // Call the GetByIdAsync method from the repository to retrieve the existing resignation
                var updateResign = await _resignationRepository.GetByIdAsync(id);
                if (updateResign == null)
                {
                    return NotFound(_response.ErrorResponse("Resignation Not Found", HttpStatusCode.NotFound));
                }
                // Validations on resignUpdateDTO
                if (resignUpdateDTO.Status != updateResign.Status)
                {
                    if (resignUpdateDTO.Status >= 5 || resignUpdateDTO.Status <= 0)
                    {
                        return BadRequest(_response.ErrorResponse("Please Enter the Valid Status number", HttpStatusCode.BadRequest));
                    }
                }
                if (resignUpdateDTO.RevelationDate == DateTime.MinValue)
                {
                    resignUpdateDTO.RevelationDate = updateResign.RevelationDate;
                }
                else
                {
                    if (resignUpdateDTO.RevelationDate != updateResign.RevelationDate)
                    {
                        if (resignUpdateDTO.RevelationDate < DateTime.Now)
                        {
                            return BadRequest(_response.ErrorResponse("Please Enter the Valid Date", HttpStatusCode.BadRequest));
                        }
                    }
                }

                // Update the resignation details
                updateResign.Status = resignUpdateDTO.Status;
                updateResign.RevelationDate = resignUpdateDTO.RevelationDate;
                updateResign.ApprovedBy = userId;
                updateResign.UpdatedAt = DateTime.Now;

                // Call the UpdateAsync method from the _resignationRepository to update the resignation.
                await _resignationRepository.UpdateAsync(id, updateResign);
                _response.Message = "Updated the Status of Resignation";
                _response.Data = resignUpdateDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);
                return _response.ErrorResponse();
            }         
        }
    }
}
