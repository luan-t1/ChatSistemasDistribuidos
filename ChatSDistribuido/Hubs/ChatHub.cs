using ChatSDistribuido.Entities;
using ChatSDistribuido.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatSDistribuido.Hubs;

public class ChatHub : Hub
{
    private readonly RedisService _redisService;

    public ChatHub(RedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task SendMessage(string user, string message)
    {
        var chatMessage = new ChatMessage
        {
            Sender = user,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await _redisService.SaveMessageAsync(chatMessage);

        await Clients.All.SendAsync("ReceiveMessage", chatMessage);
    }

    public async Task<List<ChatMessage>> GetHistory()
    {
        return await _redisService.GetMessagesAsync();
    }
}
