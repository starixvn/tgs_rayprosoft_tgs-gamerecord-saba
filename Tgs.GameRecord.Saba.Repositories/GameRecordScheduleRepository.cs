using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tgs.GameRecord.Saba.Core;
using Tgs.GameRecord.Saba.Core.Models;
using Tgs.GameRecord.Saba.Repositories.Extensions;
using Tgs.GameRecord.Saba.Repositories.Models;

namespace Tgs.GameRecord.Saba.Repositories;
internal class GameRecordScheduleRepository(
	ILogger<GameRecordScheduleRepository> logger,
	IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
	IMongoClient client) : IGameRecordScheduleRepository
{
	private readonly GameRecordInfoOptions _gameRecordInfoOptions = gameRecordInfoOptions.Value;

	public async ValueTask<GameRecordScheduleInfo> AddAsync(string operatorId, string vendorId, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var document = new GameRecordSchedule()
		{
			OperatorId = operatorId,
			VendorId = vendorId,
			VersionKey = 0,
			CreatedAt = DateTimeOffset.UtcNow.UtcDateTime,
			UpdatedAt = DateTimeOffset.UtcNow.UtcDateTime
		};

		await collection.InsertOneAsync(
			document: document,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameRecordScheduleInfo(
			OperatorId: document.OperatorId,
			VendorId: document.VendorId,
			VersionKey: document.VersionKey,
			CreatedAt: document.CreatedAt,
			UpdatedAt: document.UpdatedAt);
	}

	public async ValueTask<GameRecordScheduleInfo?> GetAsync(string operatorId, string vendorId, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var builder = Builders<GameRecordSchedule>.Filter;
		var filter = builder.Eq(x => x.OperatorId, operatorId)
			& builder.Eq(x => x.VendorId, vendorId);

		var gameRecordSchedule = (await collection.FindAsync<GameRecordSchedule>(
			filter: filter,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.FirstOrDefault(cancellationToken: cancellationToken);

		return gameRecordSchedule is null
			? null
			: new GameRecordScheduleInfo(
				OperatorId: gameRecordSchedule.OperatorId,
				VendorId: gameRecordSchedule.VendorId,
				VersionKey: gameRecordSchedule.VersionKey,
				CreatedAt: gameRecordSchedule.CreatedAt,
				UpdatedAt: gameRecordSchedule.UpdatedAt);
	}

	public async ValueTask InitializationAsync(CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();

		var tasks = new Task[]{
			Task.Run(async () =>
			{
				try
				{
					_ = await Index_UniqueAsync( collection: collection,   cancellationToken: cancellationToken).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message:This index creation failed.{index}",  DateTime.Now.ToString("u"),"Index_Unique");
				}
			}, cancellationToken: cancellationToken),
			Task.Run(async () =>
			{
				try
				{
					 _ = await Index_UpdatedAtAsync( collection: collection,   cancellationToken: cancellationToken).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message:This index creation failed.{index}",  DateTime.Now.ToString("u"),"Index_ExpireOn");
				}
			}, cancellationToken: cancellationToken)
		};

		await Task.WhenAll(tasks).ConfigureAwait(false);
	}

	public async ValueTask UpdateAsync(string operatorId, string vendorId, long versionKey, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var builder = Builders<GameRecordSchedule>.Filter;
		var filter = builder.Eq(x => x.OperatorId, operatorId)
			& builder.Eq(x => x.VendorId, vendorId);

		var update = Builders<GameRecordSchedule>.Update
			.Set(s => s.VersionKey, versionKey)
			.Set(s => s.UpdatedAt, DateTimeOffset.UtcNow.UtcDateTime);

		_ = await collection.UpdateOneAsync(
			filter: filter,
			update: update,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	private static Task<string> Index_UpdatedAtAsync(IMongoCollection<GameRecordSchedule> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<GameRecordSchedule>.IndexKeys
			.Ascending(m => m.UpdatedAt);

		var createIndexOptions = new CreateIndexOptions
		{
			ExpireAfter = TimeSpan.FromDays(7),
			Name = "idx_updated_at"
		};
		var createIndexModel = new CreateIndexModel<GameRecordSchedule>(index, createIndexOptions);

		return collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private static Task<string> Index_UniqueAsync(IMongoCollection<GameRecordSchedule> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<GameRecordSchedule>.IndexKeys
			.Ascending(m => m.OperatorId)
			.Ascending(m => m.VersionKey);

		var createIndexOptions = new CreateIndexOptions
		{
			Unique = true,
			Name = $"uni_{collection.CollectionNamespace.CollectionName}"
		};
		var createIndexModel = new CreateIndexModel<GameRecordSchedule>(index, createIndexOptions);

		return collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private IMongoCollection<GameRecordSchedule> GetCollection()
		=> client
		.GetDatabase($"game_provider_{_gameRecordInfoOptions.GameProviderCode}_master".ToLower())
		.GetCollection<GameRecordSchedule>(typeof(GameRecordSchedule).GetTableName());
}
