using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text;
using WebApplication5.Services;

namespace WebApplication5.Controllers
{
    public class HomeController : BaseCacheController
    {
        private readonly ILogger<HomeController> _logger;
        private static IConfiguration Configuration { get; set; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            if (Configuration == null)
                Configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            if (Connection == null)
            {
                await InitializeAsync();
            }

            IDatabase cache = await GetDatabaseAsync();

            RedisValue currentKey = await cache.StringGetAsync("currentKey");
            if(currentKey == RedisValue.Null || currentKey == RedisValue.EmptyString)
            {
                await cache.StringSetAsync("currentKey", 1);
                currentKey = await cache.StringGetAsync("currentKey");
            }
            ViewBag.CurrentKey = currentKey.ToString();
            ViewBag.CurrentValue = (await cache.StringGetAsync(currentKey.ToString())).ToString();

            return View("Index");
        }

        public async Task<IActionResult> RefreshKey()
        {
            if (Connection == null)
            {
                await InitializeAsync();
            }

            IDatabase cache = await GetDatabaseAsync();
            var ask = (await cache.StringGetAsync("currentKey")).ToString();
            int key = 0;
            try
            {
                key = Int32.Parse(ask);
            }catch(Exception ex)
            {

            }
            await cache.StringSetAsync("currentKey", key + 2);
            cache.StringAppendAsync((key + 2).ToString(),
                $"{DateTime.UtcNow.ToString()} - {Environment.GetEnvironmentVariable("MY_POD_NAME")} - NEW-KEY;").ToString();

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Terminate()
        {
            TerminateService.cancellationtokenSource.Cancel();
            TerminateService.cancellationtokenSource = new CancellationTokenSource();
            return RedirectToAction("Index");
        }
        

        public async Task<ActionResult> RedisCache()
        {
            ViewBag.Message = "A simple example with Azure Cache for Redis on ASP.NET Core.";

            if (Connection == null)
            {
                await InitializeAsync();
            }

            IDatabase cache = await GetDatabaseAsync();

            // Perform cache operations using the cache object...

            // Simple PING command
            ViewBag.command1 = "PING";
            ViewBag.command1Result = cache.Execute(ViewBag.command1).ToString();

            // Simple get and put of integral data types into the cache
            ViewBag.command2 = "GET Message";
            string cacheResult = cache.StringGet("Message").ToString();
            ViewBag.command2Result = cacheResult;

            ViewBag.command3 = "SET Message \"Hello! The cache is working from ASP.NET Core!\"";
            ViewBag.command3Result = cache.StringSet("Message", "Hello! The cache is working from ASP.NET Core!").ToString();

            // Demonstrate "SET Message" executed as expected...
            ViewBag.command4 = "GET Message";
            ViewBag.command4Result = cache.StringGet("Message").ToString();

            // Get the client list, useful to see if connection list is growing...
            // Note that this requires allowAdmin=true in the connection string
            ViewBag.command5 = "CLIENT LIST";
            StringBuilder sb = new StringBuilder();
            var endpoint = (System.Net.DnsEndPoint)(await GetEndPointsAsync())[0];
            IServer server = await GetServerAsync(endpoint.Host, endpoint.Port);
            ClientInfo[] clients = await server.ClientListAsync();

            sb.AppendLine("Cache response :");
            foreach (ClientInfo client in clients)
            {
                sb.AppendLine(client.Raw);
            }

            ViewBag.command5Result = sb.ToString();

            return View();
        }

    }
}