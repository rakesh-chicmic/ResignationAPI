using MongoDB.Bson;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;

namespace ResignationAPI.Repository.IRepository
{
    public interface IResignationRepository
    {
        public Task<Resignation?> GetByIdAsync(string id);
        public Task<List<ResignationWithUser>> GetAsync(int? limit, int? index, string? sortKey, int? sortDirection, string? id, int? status, string? userId);
        public Task CreateAsync(Resignation resignRequest);
        public Task UpdateAsync(string id, Resignation updatedResign);
        public Task RemoveAsync(string id);
    }
}
