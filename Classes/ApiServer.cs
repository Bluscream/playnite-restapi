using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Playnite.SDK;
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

        async static Task DefaultRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(new Responses.Default("playnite rest api server").ToJson());
            } catch (Exception e) {
                logger.Error(e, "DefaultRoute");
            }
        }

        async static Task GetGameRoute(HttpContextBase ctx) {
            try {
                var id = ctx.Request.Url.Parameters["id"];
                var games = playniteAPI.Database.Games.Where(g => g.Id.ToString() == id).Union(playniteAPI.Database.Games.Where(g => g.GameId == id));
                await ctx.Response.Send(JsonSerializer.Serialize(games));
            } catch (Exception e) {
                logger.Error(e, "GetGameRoute");
            }
        }
        async static Task GetGamesRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(JsonSerializer.Serialize(playniteAPI.Database.Games));
            } catch (Exception e) {
                logger.Error(e, "GetGamesRoute");
            }
        }
        async static Task GetSourcesRoute(HttpContextBase ctx) {
            try {
                await ctx.Response.Send(JsonSerializer.Serialize(playniteAPI.Database.Sources));
            } catch (Exception e) {
                logger.Error(e, "GetSourcesRoute");
            }
        }
    }
}
