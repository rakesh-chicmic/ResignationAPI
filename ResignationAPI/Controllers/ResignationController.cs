using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;
using ResignationAPI.Repository.IRepository;
using System.Data;
using System.Net;
using System.Security.Claims;

namespace ResignationAPI.Controllers
{
    [Route("api/resignation")]
    [ApiController]
    public class ResignationController : ControllerBase
    {
        private readonly IResignationRepository _resignationRepository;
        APIResponse _response ;

        public ResignationController(IResignationRepository resignationRepository)
        {
            _resignationRepository = resignationRepository;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<APIResponse>>> Get()
        {
            try
            {
                var res = await _resignationRepository.GetAsync();
                _response.Messages = "List of Resignations";
                _response.StatusCode = HttpStatusCode.OK;
                _response.Data = res;
                return Ok(_response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Get(string id)
        {
            try
            {
                var resignation = await _resignationRepository.GetAsync(id);

                if (resignation == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Messages = "Resignation Not Found";
                    return _response;
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Messages = "Resignation Details";
                _response.Data = resignation;
                return Ok(_response);
            }catch (Exception) {
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse>> Post(ResignationRequestDTO resignRequestDTO)
        {
            try
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;

                if (resignRequestDTO.ResignationDate == DateTime.MinValue)
                {
                    _response.Messages = "Please Enter the valid date";
                    _response.Data = null;
                    return _response;
                }
                if (resignRequestDTO.Reason == null)
                {
                    _response.Messages = "Please Enter the Reason";
                    _response.Data = null;
                    return _response;
                }
                if (resignRequestDTO.Details == null)
                {
                    _response.Messages = "Please Enter the Details";
                    _response.Data = null;
                    return _response;
                }
                Resignation request = new Resignation()
                {
                    UserId = resignRequestDTO.UserId,
                    Status = "Pending",
                    Reason = resignRequestDTO.Reason,
                    ResignationDate = resignRequestDTO.ResignationDate,
                    RevailingDate = resignRequestDTO.ResignationDate.AddMonths(2),
                    Details = resignRequestDTO.Details,
                    CreatedAT = DateTime.Now,
                    UpdatedAT = DateTime.Now,
                    ApprovedBY = null,
                };
                await _resignationRepository.CreateAsync(request);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Messages = "Created Sucessfully";
                _response.Data = resignRequestDTO;
                return Ok(_response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<APIResponse>> Update(string id, ResignationRequestDTO resignUpdateDTO)
        {
            try
            {
                var updateResign = await _resignationRepository.GetAsync(id);

                if (updateResign == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Messages = "Resignation Not Found";
                    return _response;
                }
                if (resignUpdateDTO.ResignationDate == DateTime.MinValue)
                {
                    resignUpdateDTO.ResignationDate = updateResign.ResignationDate;
                }
                if (resignUpdateDTO.Reason == null)
                {
                    resignUpdateDTO.Reason = updateResign.Reason;
                }
                if (resignUpdateDTO.Details == null)
                {
                    resignUpdateDTO.Details = updateResign.Details;
                }
                if (updateResign.UserId != resignUpdateDTO.UserId)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Messages = "This Resignation doesn't belong to this UserId";
                    return _response;
                }
                updateResign.Reason = resignUpdateDTO.Reason;
                updateResign.ResignationDate = resignUpdateDTO.ResignationDate;
                updateResign.Details = resignUpdateDTO.Details;
                updateResign.UpdatedAT = DateTime.Now;

                await _resignationRepository.UpdateAsync(id, updateResign);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Messages = "Updated the Resignation Details";
                _response.Data = resignUpdateDTO;
                return Ok(_response);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult<APIResponse>> Delete(string id)
        {
            try
            {
                var resignation = await _resignationRepository.GetAsync(id);

                if (resignation == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Messages = "Resignation Not Found";
                    return _response;
                }
                await _resignationRepository.RemoveAsync(id);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Messages = "Deleted the Resignation";
                _response.Data = null;
                return Ok(_response);
            }
            catch (Exception)
            {
                throw;
            }
           
        }
    }
}
