using AutoMapper.Internal;
using ResignationAPI.Models;

namespace ResignationAPI.Repository.IRepository
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
