using AutoMapper;
using log4net;
using log4net.Config;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;
using ResignationAPI.Repository;
using ResignationAPI.Repository.IRepository;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace ResignationAPI.Controllers
{
    [Route("api/resignation")]
    [ApiController]
    public class ResignationController : ControllerBase
    {
        private readonly IResignationRepository _resignationRepository;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IMapper _mapper;   
        APIResponse _response ;

        // Inject the required dependencies: resignationRepository, mapper, loggingRepository
        public ResignationController(IResignationRepository resignationRepository,IMapper mapper, ILoggingRepository loggingRepository)
        {
            _resignationRepository = resignationRepository;
            _loggingRepository = loggingRepository;
            _mapper = mapper;         
            _response = new();
        }

        [HttpGet,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Get(int? limit, int? index, string? sortKey, int? sortDirection, int? status, string? id, string? userId)
        {

            try
            {
                // Call the GetAsync method from the _resignationRepository to retrieve resignation details based on provided filters
                List<ResignationWithUser> resignation =  await _resignationRepository.GetAsync(limit, index, sortKey, sortDirection, id, status, userId);
               
                if (resignation == null)
                {
                    return NotFound(_response.ErrorResponse("Resignation Not Found", HttpStatusCode.NotFound));
                }
                _response.Message = "Resignation Details";
                _response.Data = resignation;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);
                return _response.ErrorResponse();
            }
        }


        [HttpPost,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Post(ResignationRequestDTO resignRequestDTO)
        {
            var userId = GetUserIdFromClaims();
            try
            {
                // Validations on resignRequestDTO
                if (resignRequestDTO.ResignationDate < DateTime.Now)
                {
                    return BadRequest(_response.ErrorResponse("Please Enter the Valid Date", HttpStatusCode.BadRequest));
                }
                if (string.IsNullOrEmpty(resignRequestDTO.Reason))
                {
                    return BadRequest(_response.ErrorResponse("Please Enter the Reason", HttpStatusCode.BadRequest));
                }
                // Mapping resignRequestDTO into Resignation
                Resignation request = _mapper.Map<Resignation>(resignRequestDTO);
                request.UserId = userId;
                request.Status = 1;
                request.RelievingDate = resignRequestDTO.ResignationDate.AddMonths(2);
                request.CreatedAt = DateTime.Now;
                request.UpdatedAt = DateTime.Now;
                request.ApprovedBy = null;

                // Call the CreateAsync method from the _resignationRepository to create a new resignation request
                await _resignationRepository.CreateAsync(request);

                _response.Message = "Created Sucessfully";
                _response.Data = resignRequestDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);
                return _response.ErrorResponse();
            }
        }

        [HttpPut("{id:length(24)}"),Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Update(string id, ResignationRequestDTO resignUpdateDTO)
        {
            try
            {
                // Call the GetByIdAsync method from the repository to retrieve the existing resignation
                var updateResign = await _resignationRepository.GetByIdAsync(id);
                if (updateResign == null)
                {
                    return NotFound(_response.ErrorResponse("Resignation Not Found", HttpStatusCode.NotFound));
                }

                // Validations on resignUpdateDTO
                if (resignUpdateDTO.ResignationDate == DateTime.MinValue)
                {
                    resignUpdateDTO.ResignationDate = updateResign.ResignationDate;
                }
                else
                {
                    if(resignUpdateDTO.ResignationDate != updateResign.ResignationDate)
                    {
                        if (resignUpdateDTO.ResignationDate < DateTime.Now)
                        {
                            return BadRequest(_response.ErrorResponse("Please Enter the Valid Date", HttpStatusCode.BadRequest));
                        }
                    }
                }

                if (string.IsNullOrEmpty(resignUpdateDTO.Reason))
                {
                    resignUpdateDTO.Reason = updateResign.Reason;
                }
                if (string.IsNullOrEmpty(resignUpdateDTO.Comments))
                {
                    resignUpdateDTO.Comments = updateResign.Comments;
                }

                // Update the resignation details
                updateResign.Reason = resignUpdateDTO.Reason;
                updateResign.ResignationDate = resignUpdateDTO.ResignationDate;
                updateResign.Comments = resignUpdateDTO.Comments;
                updateResign.UpdatedAt = DateTime.Now;

                // Call the UpdateAsync method from the _resignationRepository to update the resignation.
                await _resignationRepository.UpdateAsync(id, updateResign);

                _response.Message = "Updated the Resignation Details";
                _response.Data = updateResign;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in logRevelationDate
                _loggingRepository.LogError(ex.Message);
                return _response.ErrorResponse();
            }
        }

        [HttpDelete("{id:length(24)}"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Delete(string id)
        {
            try
            {
                // Call the GetByIdAsync method from the _resignationRepository to retrieve the existing resignation
                var resignation = await _resignationRepository.GetByIdAsync(id);
                if (resignation == null)
                {
                    return NotFound(_response.ErrorResponse("Resignation Not Found", HttpStatusCode.NotFound));
                }
                // Call the RemoveAsync method from the _resignationRepository to delete the resignation 
                await _resignationRepository.RemoveAsync(id);
                _response.Message = "Deleted the Resignation";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);               
                return _response.ErrorResponse();
            }
        }

        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "_id")?.Value!;
        }
 
    }
}
