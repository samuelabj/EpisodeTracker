﻿using System;
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
using Microsoft.Shell;

namespace EpisodeTracker.WPF {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, ISingleInstanceApp {
		private const string Unique = "MediaReign.EpisodeTracker";

		[STAThread]
		public static void Main() {
			if(SingleInstance<App>.InitializeAsFirstInstance(Unique)) {
				var application = new App();

				application.InitializeComponent();
				application.Run();

				// Allow single instance code to perform cleanup operations
				SingleInstance<App>.Cleanup();
			}
		}

		#region ISingleInstanceApp Members

		public bool SignalExternalCommandLineArgs(IList<string> args) {
			MainWindow.Show();
			MainWindow.WindowState = WindowState.Maximized;
			return true;
		}

		#endregion
	}
}
