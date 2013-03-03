using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Models {
	public abstract class Service {
		Task checkTask;
		AutoResetEvent checkEvent = new AutoResetEvent(false);

		protected Service(TimeSpan interval) {
			Interval = interval;
		}

		protected TimeSpan Interval { get; set; }
		public bool IsRunning { get { return checkTask != null; } }

		public void Start() {
			checkTask = Task.Factory.StartNew(() => {
				var wait = Interval;

				do {
					OnInterval();
				} while(!checkEvent.WaitOne(wait));
			});
		}

		public void Stop() {
			checkEvent.Set();
			checkTask.Wait();
			checkTask = null;
		}

		public void Dispose() {
			if(IsRunning) Stop();
		}

		protected abstract void OnInterval();
	}
}
