using ResignationAPI.Models;

namespace ResignationAPI.Repository.IRepository
{
    public interface IResignationRepository
    {
        public Task<List<Resignation>> GetAsync();
        public Task<Resignation?> GetAsync(string id);
        public Task CreateAsync(Resignation resignRequest);
        public Task UpdateAsync(string id, Resignation updatedResign);
        public Task RemoveAsync(string id);
    }
}
