using MasterServer.Channel;
using MasterServer.DB;
using MasterServer.Services;
using MasterServer.Sync;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace MasterServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using var log = new LoggerConfiguration().WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information).MinimumLevel.Is(Serilog.Events.LogEventLevel.Information).CreateLogger();

			Log.Logger = log;
			Log.Information("Starting Pegasus MasterServer...");

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Configuration.AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);
			builder.Configuration.AddJsonFile("chatsettings.json", optional: false, reloadOnChange: true);
			builder.Host.UseSerilog();
			builder.Services.AddGrpc();
			builder.Services.AddSingleton<DatabaseManager>();
			builder.Services.AddSingleton<ChannelManager>();
			builder.Services.AddSingleton<SyncManager>();
			builder.Services.AddHostedService<TimedChannelService>();

			builder.Services.AddRateLimiter(options =>
			{
				options.AddConcurrencyLimiter("LoginConcurrencyQueue", limiterOptions =>
				{
					limiterOptions.PermitLimit = 50;        // Max 50 active login executions at once
					limiterOptions.QueueLimit = 200;       // Up to 200 requests wait in line
					limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				});

				// Customize reaction when queue overflows (e.g., Return gRPC ResourceExhausted status)
				options.OnRejected = (context, token) =>
				{
					context.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
					return ValueTask.CompletedTask;
				};
			});

			var app = builder.Build();
			app.UseRouting();
			app.UseRateLimiter();
			app.MapGrpcService<ChannelMasterService>();
			app.MapGrpcService<AuthMasterService>();
			app.MapGrpcService<CharacterMasterService>();
			app.MapGet("/", () => "PegasusCabal MasterServer");
			

			app.Run();
		}
	}
}