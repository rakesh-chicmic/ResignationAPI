using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ResignationAPI.Models;
using ResignationAPI.Repository.IRepository;
using System.Linq;

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
        public async Task<Resignation?> GetByIdAsync(string id="")
        {
            return await _resignationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Get resignation documents based on different filters (limit, index, sorting, etc.)
        public async Task<DataList> GetAsync( string? sortKey, int? sortDirection, string? id, int? status, string? userId, int limit = 10, int index = 1)
        {
            /*limit ??= 0;
            index ??= 0;*/
            sortKey ??= "createdAt";
            sortDirection ??= 1;
            id ??= "";
            status ??= null;
            userId ??= "";

            // pipeline stages
            BsonDocument pipelineStage1 = new BsonDocument{
                {
                    "$match", new BsonDocument{
                        { "status", status }

                    }
                }
            };
            ObjectId.TryParse(userId, out var objUserId);
            BsonDocument pipelineStage2 = new BsonDocument{
                {
                    "$match", new BsonDocument{
                        { "userId",  objUserId }

                    }
                }
            };
            ObjectId.TryParse(id, out var objId);
            BsonDocument pipelineStage3 = new BsonDocument{
                {
                    "$match", new BsonDocument{
                        { "_id", objId }

                    }
                }
            };

            BsonDocument pipelineStage4 = new BsonDocument{
                {
                    "$sort", new BsonDocument{
                        { char.ToLower(sortKey[0]) + sortKey.Substring(1), sortDirection }

                    }
                }
            };

            BsonDocument pipelineStage5 = new BsonDocument{
                {
                    "$lookup", new BsonDocument{
                        { "from", "users" },
                        { "localField", "userId" },
                        { "foreignField", "_id" },
                        { "as", "userDetails" }
                    }
                }
            };

            BsonDocument pipelineStage6 = new BsonDocument{
                {
                    "$lookup", new BsonDocument{
                        { "from", "users" },
                        { "localField", "approvedBy" },
                        { "foreignField", "_id" },
                        { "as", "approverDetails" }
                    }
                }
            };

            var pipelineStage7 = new BsonDocument
            {
                {
                    "$project", new BsonDocument
                    {
                        {"userId",1 },
                        {"status",1 },
                        {"resignationDate" ,1},
                        { "relievingDate",1},
                        {"reason",1 },
                        {"comments",1 },
                        {"createdAt",1 },
                        {"approvedBy",1 },
                        { "userDetails.name", 1 },
                        { "userDetails.employeeId", 1 },
                        { "userDetails.email", 1 },
                         { "userDetails.hubstaffEmail", new BsonDocument("$arrayElemAt", new BsonArray { "$userDetails.hubstaffId.email", 0 }) },
                        { "approverDetails.name", 1 },
                        { "approverDetails.employeeId", 1 },
                        { "approverDetails.email", 1 },
                        { "approverDetails.hubstaffEmail", new BsonDocument("$arrayElemAt", new BsonArray { "$approverDetails.hubstaffId.email", 0 }) }
                    }
                }
            };

           BsonDocument pipelineStage8 = new BsonDocument("$skip",(index - 1) * limit);
           BsonDocument pipelineStage9 = new BsonDocument("$limit", limit);

            // aggregate pipeline
            var pipeline = new List<BsonDocument>();
            if (status != null)
            {
                pipeline.Add(pipelineStage1);
            }

            if (userId!="")
            {
                pipeline.Add(pipelineStage2);
            }

            if (id!="")
            {
                pipeline.Add(pipelineStage3);
            }
            
            pipeline.Add(pipelineStage4);             
            pipeline.Add(pipelineStage5);
            pipeline.Add(pipelineStage6);
            pipeline.Add(pipelineStage7);
            
            var pResults = await _resignationCollection.Aggregate<ResignationWithUser>(pipeline).ToListAsync();
            int count = pResults.Count();
            if (index >= 1 && limit >= 1)
            {
                pResults = pResults.Skip((index - 1) * limit).Take(limit).ToList();
            }
            DataList result = new DataList();
            result.TotalCount = count;
            result.Data = pResults;
            return result;   
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
