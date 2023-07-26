using ResignationAPI.Models;

namespace ResignationAPI.Repository.IRepository
{
    public interface IResignationRepository
    {
        public Task<Resignation?> GetByIdAsync(string id);
        public Task<List<Resignation>> GetAsync(int? limit, int? index, string? sortKey, string? sortDirection, string? id, int? status, string? userId);
        public Task CreateAsync(Resignation resignRequest);
        public Task UpdateAsync(string id, Resignation updatedResign);
        public Task RemoveAsync(string id);
    }
}
