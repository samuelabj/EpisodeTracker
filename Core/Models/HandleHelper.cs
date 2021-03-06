﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EpisodeTracker.Core.Models {
	public class HandleHelper {

		public HandleHelper() {
			WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
		}

		public string WorkingDirectory { get; set; }
		public string Output { get; private set; }

		public List<HandleItem> GetProcesses() {
			var proc = new Process();
			proc.StartInfo.FileName = "handle.exe";
			proc.StartInfo.WorkingDirectory = WorkingDirectory;
			proc.StartInfo.Arguments = "/accepteula";
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.CreateNoWindow = true;
			proc.Start();

			var outsb = new StringBuilder();
			do {
				outsb.Append(proc.StandardOutput.ReadToEnd());
			} while(!proc.HasExited);

			if(proc.ExitCode != 0) {
				var err = proc.StandardError.ReadToEnd();
				throw new ApplicationException(err);
			}

			var output = outsb.ToString();
			if(output.Contains("Make sure that you are an administrator")) {
				throw new ApplicationException("Must run as administrator");
			}

			Output = output;
			return ParseOutput(output);
		}

		public static List<HandleItem> ParseOutput(string output) {
			var list = new List<HandleItem>();
			using(var sr = new StringReader(output)) {
				string line = null;
				bool start = false;
				HandleItem proc = null;

				do {
					line = sr.ReadLine();		
					if(line == null || line.StartsWith("-")) {
						if(proc != null) {
							list.Add(proc);
							proc = null;
						}
						if(line != null) start = true;
					} else if(start) {
						start = false;						
						var match = Regex.Match(line, @"(?<name>.+)\spid:\s(?<pid>\d+)\s");
						var name = match.Groups["name"].Value;

						if(name != "<Non-existant Process>") {
							proc = new HandleItem();
							try {
								proc.ProcessName = Path.GetFileNameWithoutExtension(name);
							} catch(Exception e) {
								throw new ApplicationException("Could not get process name from match: " + name, e);
							}
							proc.PID = int.Parse(match.Groups["pid"].Value);
						}
					} else if(proc != null) {
						var match = Regex.Match(line, @"\sFile\s+?\(.+?\)\s+(?<file>[^\r\n]+)");
						if(match.Success) {
							proc.Files.Add(match.Groups["file"].Value);
						}
					}
				} while(line != null);
			}
			return list;
		}
	}

	public class HandleItem {
		public HandleItem() {
			Files = new List<string>();
		}

		public string ProcessName { get; set; }
		public int PID { get; set; }
		public List<string> Files { get; set; }
	}
}
