using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace TenantTunnel
{
	public class ResponseCorrelations
	{
		private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> CorrelationsIds;
		private readonly RandomNumberGenerator rng;

		public ResponseCorrelations()
		{
			this.CorrelationsIds = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
			this.rng = RandomNumberGenerator.Create();
		}

		private string Random()
		{
			var bytes = new byte[64];
			rng.GetBytes(bytes);
			return Convert.ToBase64String(bytes) + DateTime.Now.Ticks;
		}

		public Task<string> Response(out string correlationId)
		{
			var cId = Random();

			var taskCompletionSource = new TaskCompletionSource<string>();
			const int timeoutMs = 20000;
			var ct = new CancellationTokenSource(timeoutMs);
			ct.Token.Register(() =>
			{
				taskCompletionSource.TrySetCanceled();
				this.CorrelationsIds.TryRemove(cId, out TaskCompletionSource<string> removed);
			}, useSynchronizationContext: false);

			this.CorrelationsIds.AddOrUpdate(cId, taskCompletionSource, (key, value) =>
			{
				throw new ArgumentException("key already used");
			});

			correlationId = cId;
			return taskCompletionSource.Task;
		}

		public void Yield(string correlationId, string result)
		{
			if (this.CorrelationsIds.ContainsKey(correlationId))
			{
				if (this.CorrelationsIds.TryRemove(correlationId, out TaskCompletionSource<string> taskCompletionSource))
				{
					taskCompletionSource.SetResult(result);
				}
			}
		}
	}
}
