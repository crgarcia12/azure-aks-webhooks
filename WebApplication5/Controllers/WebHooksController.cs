using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHooksController : BaseCacheController
    {

        // GET: api/<WebHooksController>
        [HttpGet()]
        public async Task<string> Get()
        {
            await WaitMinutes(1);
            return "OK";
        }

        // GET: api/<WebHooksController>
        [HttpGet("{minutes}")]
        public async Task<string> Get(int minutes)
        {
            await WaitMinutes(minutes);
            return "OK";
        }
    }
}
