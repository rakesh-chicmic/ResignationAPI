using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;
using ResignationAPI.Repository.IRepository;
using System.Collections.Generic;
using System;
using System.Data;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

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

        [HttpGet,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Get(int? limit, int? index, string? sortKey, string? sortDirection, string? id, string? status, string? userId)
        {
            try
            {
                var resignation = await _resignationRepository.GetAsync(limit, index , sortKey , sortDirection, id, status, userId);

                if (resignation == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Resignation Not Found";
                    return _response;
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Resignation Details";
                _response.Data = resignation;
                return Ok(_response);
            }catch (Exception) {
                throw;
            }
        }

        [HttpPost,Authorize]
        public async Task<ActionResult<APIResponse>> Post(ResignationRequestDTO resignRequestDTO)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "_id")?.Value;
            try
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Status = false;

                if (resignRequestDTO.ResignationDate < DateTime.Now)
                {
                    _response.Message = "Please Enter the valid date";
                    _response.Data = null;
                    return _response;
                }
                if (resignRequestDTO.Reason == null)
                {
                    _response.Message = "Please Enter the Reason";
                    _response.Data = null;
                    return _response;
                }             
                Resignation request = new Resignation()
                {
                    UserId = userId,
                    Status = "Pending",
                    Reason = resignRequestDTO.Reason,
                    ResignationDate = resignRequestDTO.ResignationDate,
                    RevealingDate = resignRequestDTO.ResignationDate.AddMonths(2),
                    Comments = resignRequestDTO.Comments,
                    CreatedAT = DateTime.Now,
                    UpdatedAT = DateTime.Now,
                    ApprovedBY = null,
                };
                await _resignationRepository.CreateAsync(request);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Status = true;
                _response.Message = "Created Sucessfully";
                _response.Data = resignRequestDTO;
                return Ok(_response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("{id:length(24)}"),Authorize]
        public async Task<ActionResult<APIResponse>> Update(string id, ResignationRequestDTO resignUpdateDTO)
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
                if (resignUpdateDTO.ResignationDate == DateTime.MinValue)
                {
                    resignUpdateDTO.ResignationDate = updateResign.ResignationDate;
                }
                if (resignUpdateDTO.Reason == null)
                {
                    resignUpdateDTO.Reason = updateResign.Reason;
                }
                if (resignUpdateDTO.Comments == null)
                {
                    resignUpdateDTO.Comments = updateResign.Comments;
                }
                updateResign.Reason = resignUpdateDTO.Reason;
                updateResign.ResignationDate = resignUpdateDTO.ResignationDate;
                updateResign.Comments = resignUpdateDTO.Comments;
                updateResign.UpdatedAT = DateTime.Now;

                await _resignationRepository.UpdateAsync(id, updateResign);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Updated the Resignation Details";
                _response.Data = resignUpdateDTO;
                return Ok(_response);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpDelete("{id:length(24)}"), Authorize]
        public async Task<ActionResult<APIResponse>> Delete(string id)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "_id")?.Value;
            try
            {
                var resignation = await _resignationRepository.GetByIdAsync(id);

                if (resignation == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Resignation Not Found";
                    _response.Status = false;
                    return _response;
                }

                await _resignationRepository.RemoveAsync(id);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Deleted the Resignation";
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
