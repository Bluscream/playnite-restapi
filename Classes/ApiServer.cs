using Playnite.SDK;
using System;
using System.Linq;
using System.Threading.Tasks;
using WatsonWebserver.Core;
using WatsonWebserver.Lite;
using WatsonWebserver.Lite.Extensions.HostBuilderExtension;

namespace RestAPI {

    internal class ApiServer {
        internal const string DefaultIp = "0.0.0.0";
        private static readonly ILogger logger = LogManager.GetLogger();
        internal static WebserverLite server { get; set; }
        internal static IPlayniteAPI playniteAPI { get; set; }

        internal static void Start(IPlayniteAPI _playniteApi, string ip = DefaultIp, short port = 9000) {
            logger.Info($"Starting API server on {ip}:{port}...");
            server = new HostBuilder(ip, port, false, DefaultRoute)
                .MapStaticRoute(HttpMethod.GET, "/games", GetGamesRoute)
                .MapStaticRoute(HttpMethod.GET, "/sources", GetSourcesRoute)
                .MapStaticRoute(HttpMethod.GET, "/companies", GetCompaniesRoute)
                .MapParameteRoute(HttpMethod.GET, "/game/{id}", GetGameRoute)
            .Build();
            playniteAPI = _playniteApi;
            server.Start();
            logger.Info("API server started!");
        }

        internal static void Stop() {
            logger.Info("Stopping API server...");
            server.Stop();
            server.Dispose();
            logger.Info("API server stopped!");
        }

        private static async Task DefaultRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(new Responses.Default("playnite rest api server").ToJson());
            } catch (Exception e) {
                logger.Error(e, "DefaultRoute");
            }
        }

        private static async Task GetGameRoute(HttpContextBase ctx) {
            try {
                var id = ctx.Request.Url.Parameters["id"];
                var games = playniteAPI.Database.Games.Where(g => g.Id.ToString() == id).Union(playniteAPI.Database.Games.Where(g => g.GameId == id));
                await ctx.Response.Send(games.ToJson());
            } catch (Exception e) {
                logger.Error(e, "GetGameRoute");
            }
        }

        private static async Task GetGamesRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(playniteAPI.Database.Games.ToJson());
            } catch (Exception e) {
                logger.Error(e, "GetGamesRoute");
            }
        }

        private static async Task GetSourcesRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(playniteAPI.Database.Sources.ToJson());
            } catch (Exception e) {
                logger.Error(e, "GetSourcesRoute");
            }
        }

        private static async Task GetCompaniesRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(playniteAPI.Database.Companies.ToJson());
            } catch (Exception e) {
                logger.Error(e, "GetCompaniesRoute");
            }
        }
    }
}