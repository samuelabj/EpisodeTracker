using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EpisodeTracker.WPF.Views.Shared;
using Hardcodet.Wpf.TaskbarNotification;
using MediaReign.EpisodeTracker.Data;

namespace EpisodeTracker.WPF {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		Mutex singleInstance;

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
		private const int SW_MAXIMIZE = 3;
		private const int SW_SHOWNORMAL = 1;

		protected override void OnStartup(StartupEventArgs e) {
			singleInstance = new Mutex(true, @"Global\EventTracker.WPF");
			
			if(singleInstance.WaitOne(TimeSpan.Zero, true)) {				
				base.OnStartup(e);
			} else {
				NativeMethods.PostMessage(
					(IntPtr)NativeMethods.HWND_BROADCAST,
					NativeMethods.WM_SHOWME,
					IntPtr.Zero,
					IntPtr.Zero
				);
				App.Current.Shutdown();
			}			
		}
	}
}
