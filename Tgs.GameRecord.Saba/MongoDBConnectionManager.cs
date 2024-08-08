using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tgs.GameRecord.Saba;

namespace Tgs.GameRecord.Saba;

internal class MongoDBConnectionManager
{
	public IMongoClient MongoClient { get; }

	public MongoDBConnectionManager(
		IOptions<MongoDBOptions> optionsAccessor)
	{
		var options = optionsAccessor.Value;
		var mongoUrl = new MongoUrl(options.ConnectionString);
		MongoClient = new MongoClient(mongoUrl);
	}
}
