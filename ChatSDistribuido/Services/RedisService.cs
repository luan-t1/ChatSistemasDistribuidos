using ChatSDistribuido.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace ChatSDistribuido.Services;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private const string ChatMessagesKey = "chat:messages";

    public RedisService(IConfiguration configuration)
    {
        _redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
        _db = _redis.GetDatabase();
    }

    public async Task SetAsync(string key, string value)
    {
        await _db.StringSetAsync(key, value);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }

    public async Task SaveMessageAsync(ChatMessage message)
    {
        string json = JsonSerializer.Serialize(message);
        await _db.ListRightPushAsync(ChatMessagesKey, json);

        await _db.ListTrimAsync(ChatMessagesKey, -50, -1);
    }

    public async Task<List<ChatMessage>> GetMessagesAsync()
    {
        var messages = await _db.ListRangeAsync(ChatMessagesKey);
        return messages.Select(msg => JsonSerializer.Deserialize<ChatMessage>(msg!)).ToList()!;
    }

    public async Task ResetChatAsync()
    {
        await _db.KeyDeleteAsync(ChatMessagesKey);
    }
}
