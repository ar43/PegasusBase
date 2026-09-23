using System.Collections.Concurrent;

namespace MasterServer.Helpers
{
	public class LoginCooldownTracker
	{
		// Tracks when an IP address is allowed to try logging in again
		private readonly ConcurrentDictionary<string, DateTime> _cooldowns = new();

		public bool IsInCooldown(string ipAddress, out TimeSpan remaining)
		{
			if (_cooldowns.TryGetValue(ipAddress, out var cooldownUntil))
			{
				if (DateTime.UtcNow < cooldownUntil)
				{
					remaining = cooldownUntil - DateTime.UtcNow;
					return true;
				}
				// Cooldown expired, clean up entry
				_cooldowns.TryRemove(ipAddress, out _);
			}

			remaining = TimeSpan.Zero;
			return false;
		}

		public void RegisterFailedAttempt(string ipAddress, TimeSpan cooldownDuration)
		{
			_cooldowns[ipAddress] = DateTime.UtcNow.Add(cooldownDuration);
		}
	}
}
