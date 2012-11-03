﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaReign.EpisodeTracker.Models {
	public class HandleHelper {

		public HandleHelper() {
			WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
		}

		public string WorkingDirectory { get; set; }

		public List<ProcessHandle> GetProcesses() {
			var proc = new Process();
			proc.StartInfo.FileName = "handle.exe";
			proc.StartInfo.WorkingDirectory = WorkingDirectory;
			proc.StartInfo.Arguments = "/accepteula";
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.Start();

			var outsb = new StringBuilder();
			do {
				outsb.Append(proc.StandardOutput.ReadToEnd());
			} while(!proc.HasExited);

			if(proc.ExitCode != 0) {
				var err = proc.StandardError.ReadToEnd();
				throw new ApplicationException(err);
			}

			return ParseOutput(outsb.ToString());
		}

		public static List<ProcessHandle> ParseOutput(string output) {
			var list = new List<ProcessHandle>();
			using(var sr = new StringReader(output)) {
				string line = null;
				bool start = false;
				ProcessHandle proc = null;

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
						proc = new ProcessHandle();
						var match = Regex.Match(line, @"(?<name>.+)\spid:\s(?<pid>\d+)\s");
						proc.Name = Path.GetFileNameWithoutExtension(match.Groups["name"].Value);
						proc.PID = int.Parse(match.Groups["pid"].Value);
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

	public class ProcessHandle {
		public ProcessHandle() {
			Files = new List<string>();
		}

		public string Name { get; set; }
		public int PID { get; set; }
		public List<string> Files { get; set; }
	}
}
