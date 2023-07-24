using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ResignationAPI.Models;
using ResignationAPI.Repository.IRepository;

namespace ResignationAPI.Repository
{
    public class ResignationRepository : IResignationRepository
    {
        private readonly IMongoCollection<Resignation> _resignationCollection;
        public ResignationRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _resignationCollection = mongoDatabase.GetCollection<Resignation>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Resignation>> GetAsync()
        {
            return await _resignationCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Resignation?> GetAsync(string id)
        {
            return await _resignationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Resignation resignRequest)
        {
            await _resignationCollection.InsertOneAsync(resignRequest);
        }

        public async Task UpdateAsync(string id, Resignation updatedResign)
        {
            await _resignationCollection.ReplaceOneAsync(x => x.Id == id, updatedResign);
        }

        public async Task RemoveAsync(string id)
        {
            await _resignationCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
