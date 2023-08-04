using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;
using ResignationAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json;

namespace ResignationAPI.Controllers
{
    [Route("api/resignationStatus")]
    [ApiController]
    public class ResignationStatusController : ControllerBase
    {
        private readonly IResignationRepository _resignationRepository;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IMailService _mailService;
        APIResponse _response;
        public ResignationStatusController(IResignationRepository resignationRepository, ILoggingRepository loggingRepository,IMailService mailService)
        {
            _resignationRepository = resignationRepository;
            _loggingRepository = loggingRepository;
            _mailService = mailService;
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
                    if (resignUpdateDTO.Status != 0)
                    {
                        if (resignUpdateDTO.Status >= 5 || resignUpdateDTO.Status <= 0)
                        {
                            return BadRequest(_response.ErrorResponse("Please Enter the Valid Status number", HttpStatusCode.BadRequest));
                        }
                    }
                    else
                    {
                        resignUpdateDTO.Status = updateResign.Status;
                    }   
                }
                if (resignUpdateDTO.RelievingDate == DateTime.MinValue)
                {
                    resignUpdateDTO.RelievingDate = updateResign.RelievingDate;
                }
                else
                {
                    if (resignUpdateDTO.RelievingDate != updateResign.RelievingDate)
                    {
                        if (resignUpdateDTO.RelievingDate < DateTime.Now)
                        {
                            return BadRequest(_response.ErrorResponse("Please Enter the Valid Date", HttpStatusCode.BadRequest));
                        }
                    }
                }

                // Update the resignation details
                updateResign.Status = resignUpdateDTO.Status;
                updateResign.RelievingDate = resignUpdateDTO.RelievingDate;
                updateResign.ApprovedBy = userId;
                updateResign.UpdatedAt = DateTime.Now;
                List<ResignationWithUser> resignation = await _resignationRepository.GetAsync(0, 0, null, 1, id,null , null);

                string userResponse = JsonConvert.SerializeObject(resignation[0].UserDetails);
                List<User> usersDetails = JsonConvert.DeserializeObject<List<User>>(userResponse)!;

                string approverResponse = JsonConvert.SerializeObject(resignation[0].ApproverDetails);
                List<User> approverDetails = JsonConvert.DeserializeObject<List<User>>(approverResponse)!;
                if (resignation[0].Status == 3)
                {
                    MailRequest mailRequest = new MailRequest()
                    {
                        ToEmail = usersDetails[0].HubstaffEmail,
                        Subject ="Resignation Approved",
                        Body = $"Hello {usersDetails[0].Name},\nYour Resignation  has been approved by {approverDetails[0].Name}.\n\nRegards,\nTeam ChicMic\n",                
                    };
                    await _mailService.SendEmailAsync(mailRequest);
                }

                // Call the UpdateAsync method from the _resignationRepository to update the resignation.
                await _resignationRepository.UpdateAsync(id, updateResign);
                _response.Message = "Updated the Status of Resignation";
                _response.Data = updateResign;
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
