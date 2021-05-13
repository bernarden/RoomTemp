using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace RoomTemp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if DEBUG
                // Make ui/api accessible by machine ip address.
                .UseKestrel(x => x.ListenAnyIP(3000))
#endif
                .UseStartup<Startup>();
    }
}