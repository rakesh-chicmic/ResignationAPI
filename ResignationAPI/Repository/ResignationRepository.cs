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
        // MongoDB collection for the "Resignation" documents
        private readonly IMongoCollection<Resignation> _resignationCollection;

        // Constructor to initialize the repository with MongoDB settings
        public ResignationRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            // Create a MongoClient instance using the provided connection string
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            // Get the MongoDB database based on the provided database name
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            // Get the MongoDB collection for the "Resignation" documents based on the provided collection name
            _resignationCollection = mongoDatabase.GetCollection<Resignation>(databaseSettings.Value.CollectionName);
        }

        // Get a single resignation document by its id
        public async Task<Resignation?> GetByIdAsync(string id)
        {
            return await _resignationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Get resignation documents based on different filters (limit, index, sorting, etc.)
        public async Task<List<Resignation>> GetAsync(int? limit, int? index, string? sortKey, string? sortDirection, string? id, int? status, string? userId)
        {
            limit ??= 0;
            index ??= 0;
            sortKey ??= "CreatedAT";
            sortDirection ??= "asc";
            id ??= "";
            status ??= null;
            userId ??= "";

            // Define the sorting criteria based on the provided sortKey and sortDirection
            var sortDefinition = Builders<Resignation>.Sort.Ascending(sortKey);

            // Define the filter for the query based on the provided filter criteria
            var searchFilter = Builders<Resignation>.Filter.Empty;

            if (status != null)
            {
                var statusFilter = Builders<Resignation>.Filter.Eq(r => r.Status, status);
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

            // Perform the database query using the filter and sorting criteria, and apply pagination
            var resignations = await _resignationCollection.Find(searchFilter).Sort(sortDefinition).Skip((index - 1) * limit).Limit(limit).ToListAsync();
            return resignations;
        }

        // Create a new resignation request document in the database
        public async Task CreateAsync(Resignation resignRequest)
        {
            await _resignationCollection.InsertOneAsync(resignRequest);
        }

        // Update an existing resignation document in the database
        public async Task UpdateAsync(string id, Resignation updatedResign)
        {
            await _resignationCollection.ReplaceOneAsync(x => x.Id == id, updatedResign);
        }

        // Delete a resignation document from the database based on its id
        public async Task RemoveAsync(string id)
        {
            await _resignationCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
