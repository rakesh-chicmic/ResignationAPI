using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ResignationAPI.Models;
using ResignationAPI.Repository.IRepository;

namespace ResignationAPI.Repository
{
    public class ResignationRepository : IResignationRepository
    {
        // mongodb collection 
        private readonly IMongoCollection<Resignation> _resignationCollection;
        public ResignationRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _resignationCollection = mongoDatabase.GetCollection<Resignation>(databaseSettings.Value.CollectionName);
        }

        // Get resignation by id
        public async Task<Resignation?> GetByIdAsync(string id)
        {
            return await _resignationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // get resignation based on different filters 
        public async Task<List<Resignation>> GetAsync(int? limit, int? index, string? sortKey, string? sortDirection, string? id, string? status, string? userId)
        {
            limit ??= 0;
            index ??= 0;
            sortKey ??= "CreatedAT";
            sortDirection ??= "asc";
            id ??= "";
            status ??= "";
            userId ??= "";
            var sortDefinition = Builders<Resignation>.Sort.Ascending(sortKey);
            var searchFilter = Builders<Resignation>.Filter.Empty;
            if (!string.IsNullOrEmpty(status))
            {
                var statusFilter = Builders<Resignation>.Filter.Regex("Status", new BsonRegularExpression(status, "i"));
                searchFilter &= statusFilter;
            }
            if (!string.IsNullOrEmpty(id))
            {
                var idFilter = Builders<Resignation>.Filter.Eq(r => r.Id, id);
                searchFilter &= idFilter;
            }
            if (!string.IsNullOrEmpty(userId))
            {
                var userIdFilter = Builders<Resignation>.Filter.Eq(r => r.UserId, userId);
                searchFilter &= userIdFilter;
            }

            if (sortDirection.ToLower() == "desc")
            {
                sortDefinition = Builders<Resignation>.Sort.Descending(sortKey);
            }

            if (sortDirection.ToLower() == "asc")
            {
                sortDefinition = Builders<Resignation>.Sort.Ascending(sortKey);
            }

            var resignations = await _resignationCollection.Find(searchFilter)
                                          .Sort(sortDefinition)
                                          .Skip((index - 1) * limit)
                                          .Limit(limit)
                                          .ToListAsync();
            return resignations;
        }

        // create a resignation request
        public async Task CreateAsync(Resignation resignRequest)
        {
            await _resignationCollection.InsertOneAsync(resignRequest);
        }

        // update the resignation request
        public async Task UpdateAsync(string id, Resignation updatedResign)
        {
            await _resignationCollection.ReplaceOneAsync(x => x.Id == id, updatedResign);
        }

        // delete the resignation request
        public async Task RemoveAsync(string id)
        {
            await _resignationCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
