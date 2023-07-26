using AutoMapper;
using log4net;
using log4net.Config;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;
using ResignationAPI.Repository.IRepository;
using System.Net;
using System.Reflection;

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
            _resignationRepository = resignationRepository;
            _loggingRepository = loggingRepository;
            _mapper = mapper;         
            _response = new();
        }

        [HttpGet,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Get(int? limit, int? index, string? sortKey, string? sortDirection, string? status, string? id, string? userId)
        {
            try
            {
                // Call the GetAsync method from the _resignationRepository to retrieve resignation details based on provided filters
                var resignation = await _resignationRepository.GetAsync(limit, index , sortKey , sortDirection, id, status, userId);
                if (resignation == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Resignation Not Found";
                    return _response;
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Resignation Details";
                _response.Status = true;
                _response.Data = resignation;
                return Ok(_response);
            }
            catch (Exception ex) {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = "Error retrieving data from the database. Check the logs";
                _response.Status = false;
                return _response;
               
            }
        }

        [HttpPost,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Post(ResignationRequestDTO resignRequestDTO)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "_id")?.Value;
            try
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                // Validations on resignRequestDTO
                if (resignRequestDTO.ResignationDate < DateTime.Now)
                {
                    _response.Message = "Please Enter the valid date";
                    return _response;
                }
                if (string.IsNullOrEmpty(resignRequestDTO.Reason))
                {
                    _response.Message = "Please Enter the Reason";
                    return _response;
                }
                // Mapping resignRequestDTO into Resignation
                Resignation request = _mapper.Map<Resignation>(resignRequestDTO);
                request.UserId = userId;
                request.Status = "Pending";
                request.RevealingDate = resignRequestDTO.ResignationDate.AddMonths(2);
                request.CreatedAt = DateTime.Now;
                request.UpdatedAt = DateTime.Now;
                request.ApprovedBy = null;

                // Call the CreateAsync method from the _resignationRepository to create a new resignation request
                await _resignationRepository.CreateAsync(request);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Status = true;
                _response.Message = "Created Sucessfully";
                _response.Data = resignRequestDTO;
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

        [HttpPut("{id:length(24)}"),Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Update(string id, ResignationRequestDTO resignUpdateDTO)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "_id")?.Value;
            try
            {
                // Call the GetByIdAsync method from the repository to retrieve the existing resignation
                var updateResign = await _resignationRepository.GetByIdAsync(id);
                if (updateResign == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Resignation Not Found";
                    return _response;
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
                            _response.Message = "Please Enter the valid date";
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            return _response;
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
                _response.StatusCode = HttpStatusCode.OK;
                _response.Status = true;
                _response.Message = "Updated the Resignation Details";
                _response.Data = resignUpdateDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = "Error retrieving data from the database. Check the logs";
                _response.Status = false;
                return _response;
            }
        }

        [HttpDelete("{id:length(24)}"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Delete(string id)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "_id")?.Value;
            try
            {
                // Call the GetByIdAsync method from the _resignationRepository to retrieve the existing resignation
                var resignation = await _resignationRepository.GetByIdAsync(id);
                if (resignation == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Resignation Not Found";
                    return _response;
                }
                // Call the RemoveAsync method from the _resignationRepository to delete the resignation 
                await _resignationRepository.RemoveAsync(id);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Status = true;
                _response.Message = "Deleted the Resignation";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Save the error in log
                _loggingRepository.LogError(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = "Error retrieving data from the database. Check the logs";
                _response.Status = false;
                return _response;
            }
        }
    }
}
