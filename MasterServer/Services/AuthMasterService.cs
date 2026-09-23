using Google.Protobuf;
using Grpc.Core;
using LibPegasus.Enums;
using MasterServer.Channel;
using MasterServer.DB;
using MasterServer.Helpers;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Protos;

namespace MasterServer.Services
{
	public class AuthMasterService : AuthMaster.AuthMasterBase
	{
		private readonly DatabaseManager _databaseManager;
		private readonly ChannelManager _channelManager;
		private readonly LoginCooldownTracker _cooldownTracker;

		public AuthMasterService(DatabaseManager databaseManager, ChannelManager channelManager, LoginCooldownTracker cooldownTracker)
		{
			_databaseManager = databaseManager;
			_channelManager = channelManager;
			_cooldownTracker = cooldownTracker;
		}

		public override Task<RegisterAccountReply> Register(RegisterAccountRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called Register");
			var code = _databaseManager.AccountManager.RequestRegister(request.Username, request.Password);
			Serilog.Log.Information("Registration return code: " + code.Result);
			return Task.FromResult(new RegisterAccountReply
			{
				InfoCode = (uint)code.Result
			});
		}

		public override Task<SessionReply> CreateSession(SessionRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called CreateSession");
			var code = _databaseManager.WorldSessionManager.Create(request.AuthKey, (UInt16)request.UserId, (byte)request.ChannelId, (byte)request.ServerId, request.AccountId);
			//Serilog.Log.Information("Registration return code: " + code.Result);
			return Task.FromResult(new SessionReply
			{
				//InfoCode = (uint)code.Result
				Result = (uint)code.Result
			});
		}

		public override Task<SessionReply> CreateLoginSession(SessionRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called CreateLoginSession");
			var code = _databaseManager.LoginSessionManager.Create(request.AuthKey, (UInt16)request.UserId, (byte)request.ChannelId, (byte)request.ServerId, request.AccountId);
			//Serilog.Log.Information("Registration return code: " + code.Result);
			return Task.FromResult(new SessionReply
			{
				//InfoCode = (uint)code.Result
				Result = (uint)code.Result
			});
		}

		[EnableRateLimiting("LoginConcurrencyQueue")]
		public override async Task<LoginAccountReply> Login(LoginAccountRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called Login");
			AuthResult status = AuthResult.NONE;
			var httpContext = context.GetHttpContext();
			var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			if (_cooldownTracker.IsInCooldown(ipAddress, out TimeSpan remaining))
			{
				Serilog.Log.Warning("Rejected login attempt from throttled IP {IP}. Cooldown remaining: {Seconds}s",
					ipAddress, Math.Ceiling(remaining.TotalSeconds));

				return new LoginAccountReply
				{
					Status = (uint)AuthResult.TOO_MANY_FAIL,
					AccountId = 0,
					ServerCount = (uint)0,
					CharData = ByteString.Empty,
				};
			}


			var accountId = await _databaseManager.AccountManager.RequestLogin(request.Username, request.Password);
			//Serilog.Log.Information("Login return code: " + success.Result);

			var serverData = _channelManager.GetSerializedServerData();
			var serverCount = 0;

			//TODO: send bad result if acc is already logged in
			if (accountId > 0)
			{
				status = AuthResult.SUCCESS;

				var charCountData = await _databaseManager.CharacterManager.GetCharacterCount((int)accountId);
				if (serverData != null)
				{
					serverCount = serverData.Length / 2;
					for (int i = 0; i < serverData.Length; i += 2)
					{
						charCountData.TryGetValue(serverData[i], out int charCount);
						serverData[i + 1] = (Byte)charCount;
					}
				}
			}
			else
			{
				_cooldownTracker.RegisterFailedAttempt(ipAddress, TimeSpan.FromSeconds(1));
				status = AuthResult.INCORRECT;
			}

			return new LoginAccountReply
			{
				Status = (uint)status,
				AccountId = accountId,
				ServerCount = (uint)serverCount,
				CharData = ByteString.CopyFrom(serverData),
			};
		}
	}
}
