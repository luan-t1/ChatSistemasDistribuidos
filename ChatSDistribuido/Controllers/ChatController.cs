using ChatSDistribuido.Entities;
using ChatSDistribuido.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatSDistribuido.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly RedisService _redisService;

        public ChatController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            await _redisService.SaveMessageAsync(message);
            return Ok();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var messages = await _redisService.GetMessagesAsync();
            return Ok(messages);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetChat()
        {
            await _redisService.ResetChatAsync();
            return Ok("Chat resetado com sucesso.");
        }
    }
}
