using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;
using MediaReign.EpisodeTracker.Data;
using NLog;

namespace MediaReign.EpisodeTracker.Monitors {
	public class ProcessMonitor {
		class MonitoredFile {
			public DateTime Start { get; set; }
			public string Filename { get; set; }
			public TvMatch Match { get; set; }
			public TimeSpan? Length { get; set; }
			public bool Tracked { get; set; }
			public int PreviousTrackedSeconds { get; set; }
			public bool Watched { get; set; }
		}

		static readonly string[] VideoExtensions = new[] {
			"avi"
			,"divx"
			,"dvr"
			,"mkv"
			,"mp4"
			,"mpeg"
			,"mpeg4"
			,"mpg"
			,"ogm"
			,"wmv"
		};

		List<MonitoredFile> monitored;
		Task checkTask;
		AutoResetEvent checkEvent = new AutoResetEvent(false);

		public ProcessMonitor(Logger logger) {
			ApplicationNames = new List<string>();
			ApplicationNames.Add("PotPlayerMini64");
			Logger = logger;
		}

		public List<string> ApplicationNames { get; private set; }
		public Logger Logger { get; private set; }
		public bool Running { get { return checkTask != null; } }

		public void Start() {
			monitored = new List<MonitoredFile>();
			checkTask = Task.Factory.StartNew(() => {
				var wait = TimeSpan.FromSeconds(30);

				do {
					try {
						Check();
					} catch(Exception e) {
						Logger.Error("Error while checking processes: " + e, e);
					}

				} while(!checkEvent.WaitOne(wait));
			});
		}

		public void Stop() {
			checkEvent.Set();
			checkTask.Wait();
			checkTask = null;
		}

		void Check() {
			Logger.Trace("Checking for open files");

			var files = GetMediaFiles();
			Logger.Trace("Files found: " + files.Count());

			foreach(var f in files) {
				Logger.Trace("Found file: " + f);
				var mon = monitored.SingleOrDefault(m => m.Filename.Equals(f, StringComparison.OrdinalIgnoreCase));
				if(mon == null) {
					Logger.Debug("File is not monitored: " + f);
					var match = new TvMatcher().Match(f);
					if(match != null) {
						Logger.Debug("Found episode info - name: " + match.Name + ", season: " + match.Season + ", episode: " + match.Episode);
					}

					mon = new MonitoredFile {
						Filename = f,
						Start = DateTime.Now,
						Match = match,
						Length = GetVideoLength(f)
					};
					monitored.Add(mon);

					// check if it's been tracked before
					using(var db = new EpisodeTrackerDBContext()) {
						var tracked = GetTrackedItem(db, mon);
						if(tracked != null) {
							Logger.Debug("This file has been tracked before");
							mon.Tracked = true;
							mon.PreviousTrackedSeconds = tracked.TrackedSeconds;
							mon.Watched = tracked.Watched;
						}
					}
					
				} else {
					Logger.Trace("File is monitored");

					if(!mon.Watched) {
						using(var db = new EpisodeTrackerDBContext()) {
							ITrackedItem tracked = null;
							Logger.Trace("Seconds since started monitoring: " + DateTime.Now.Subtract(mon.Start).TotalSeconds);

							// check if it's been monitored for a while before doing anything with file
							if(mon.Start <= DateTime.Now.AddMinutes(-1)) {				
								if(!mon.Tracked) {
									Logger.Debug("Recording file/episode as tracked: " + f);
									tracked = NewTrackedItem(db, mon);
									mon.Tracked = true;
								}


								if(tracked == null) tracked = GetTrackedItem(db, mon);
								tracked.TrackedSeconds = (int)DateTime.Now.Subtract(mon.Start).TotalSeconds + mon.PreviousTrackedSeconds;
								tracked.LastTracked = DateTime.Now;
								Logger.Trace("Total tracked seconds: " + tracked.TrackedSeconds);

								//if(mon.Length.HasValue && tracked.TrackedSeconds >= (mon.Length.Value.TotalSeconds * .75)) {
								//	Logger.Debug("Monitored file has probably been watched: " + mon.Filename);
								//	tracked.ProbablyWatched = true;
								//}
								db.SaveChanges();
							}
						}
					}
				}
			}

			for(var i = 0; i < monitored.Count; i++) {
				var mon = monitored[i];

				if(!files.Contains(mon.Filename)) {
					Logger.Debug("Monitored file is no longer open and will be removed: " + mon.Filename);

					// not open anymore
					if(mon.Tracked) {
						using(var db = new EpisodeTrackerDBContext()) {
							var tracked = GetTrackedItem(db, mon);
							if(mon.Length.HasValue && tracked.TrackedSeconds >= (mon.Length.Value.TotalSeconds * .75)) {
								Logger.Debug("Monitored file has probably been watched: " + mon.Filename);
								tracked.ProbablyWatched = true;
								db.SaveChanges();
							}
						}
					}

					monitored.RemoveAt(i);
					i--;
					continue;
				}
			}
		}

		ITrackedItem GetTrackedItem(EpisodeTrackerDBContext db, MonitoredFile mon) {
			if(mon.Match != null) {
				var match = mon.Match;
				// ignore multi episode files for now
				return db.TrackedEpisodes.FirstOrDefault(e => e.FileName == mon.Filename|| e.TrackedSeries.Name == match.Name && e.Number == match.Episode);
			} else {
				return db.TrackedOthers.SingleOrDefault(o => o.FileName == mon.Filename);
			}
		}

		ITrackedItem NewTrackedItem(EpisodeTrackerDBContext db, MonitoredFile mon) {
			ITrackedItem tracked = null;
			if(mon.Match != null) {
				var series = db.TrackedSeries.SingleOrDefault(s => s.Name == mon.Match.Name);
				if(series == null) {
					series = new TrackedSeries {
						Name = mon.Match.Name,
						Added = DateTime.Now
					};
					db.TrackedSeries.Add(series);
				}

				tracked = new TrackedEpisode {
					FileName = mon.Filename,
					Season = mon.Match.Season ?? 0,
					Number = mon.Match.Episode
				};
				series.TrackedEpisodes.Add((TrackedEpisode)tracked);
			} else {
				tracked = new TrackedOther {
					FileName = mon.Filename
				};
				db.TrackedOthers.Add((TrackedOther)tracked);
			}
			tracked.Added = DateTime.Now;
			tracked.LastTracked = tracked.Added;

			db.SaveChanges();
			return tracked;
		}

		List<string> GetMediaFiles() {
			var videoFiles = new List<string>();

			var processes = Process.GetProcesses();
			foreach(var process in processes) {
				if(process.Id <= 4) { continue; } // system processes
				if(!ApplicationNames.Contains(process.ProcessName)) continue;

				var files = GetFilesLockedBy(process);
				foreach(var f in files) {
					var ext = Path.GetExtension(f);
					if(ext.Length > 1) ext = ext.Substring(1);
					if(VideoExtensions.Contains(ext.ToLower()) && !videoFiles.Contains(f, StringComparer.OrdinalIgnoreCase)) {
						videoFiles.Add(f);
					}
				}
			}
			return videoFiles;
		}

		/// <summary>
		/// Return a list of file locks held by the process.
		/// </summary>
		List<string> GetFilesLockedBy(Process process) {
			var outp = new List<string>();

			var task = Task.Factory.StartNew(() => {
				try {
					outp = UnsafeGetFilesLockedBy(process);
				} catch(Exception e) {
					// ignore
					Logger.Error("Error getting locked files: " + e, e);
				}
			});

			task.Wait(250);
			return outp;
		}

		TimeSpan? GetVideoLength(string file) {
			var fi = new FileInfo(file);
			var shellAppType = Type.GetTypeFromProgID("Shell.Application");

			// hack to fix exception - Unable to cast COM object of type 'System.__ComObject' to interface type 'Shell32.Shell'
			dynamic shell = Activator.CreateInstance(shellAppType);
			var folder = (Shell32.Folder)shell.NameSpace(fi.DirectoryName);
			var item = folder.ParseName(fi.Name);

			for(int i = 0; i < short.MaxValue; i++) {
				var key = folder.GetDetailsOf(null, i);
				if(key != "Length") continue;
				var val = folder.GetDetailsOf(item, i);
				TimeSpan length;
				if(TimeSpan.TryParse(val, out length)) {
					return length;
				}
			}

			return null;
		}


		#region scary win32 stuff
		List<string> UnsafeGetFilesLockedBy(Process process) {
			var files = new List<string>();
			try {
				var handles = GetHandles(process);

				foreach(var handle in handles) {
					var file = GetFilePath(handle, process);
					if(file != null) files.Add(file);
				}
			} catch(Exception e) {
				Logger.Error("Error running unsafe get file handles: " + e, e);
			}
			return files;
		}

		const int CNST_SYSTEM_HANDLE_INFORMATION = 16;
		string GetFilePath(Win32API.SYSTEM_HANDLE_INFORMATION systemHandleInformation, Process process) {
			var ipProcessHwnd = Win32API.OpenProcess(Win32API.ProcessAccessFlags.All, false, process.Id);
			var objBasic = new Win32API.OBJECT_BASIC_INFORMATION();
			var objObjectType = new Win32API.OBJECT_TYPE_INFORMATION();
			var objObjectName = new Win32API.OBJECT_NAME_INFORMATION();
			var strObjectName = "";
			var nLength = 0;
			IntPtr ipTemp, ipHandle;

			if(!Win32API.DuplicateHandle(ipProcessHwnd, systemHandleInformation.Handle, Win32API.GetCurrentProcess(), out ipHandle, 0, false, Win32API.DUPLICATE_SAME_ACCESS))
				return null;

			IntPtr ipBasic = Marshal.AllocHGlobal(Marshal.SizeOf(objBasic));
			Win32API.NtQueryObject(ipHandle, (int)Win32API.ObjectInformationClass.ObjectBasicInformation, ipBasic, Marshal.SizeOf(objBasic), ref nLength);
			objBasic = (Win32API.OBJECT_BASIC_INFORMATION)Marshal.PtrToStructure(ipBasic, objBasic.GetType());
			Marshal.FreeHGlobal(ipBasic);

			IntPtr ipObjectType = Marshal.AllocHGlobal(objBasic.TypeInformationLength);
			nLength = objBasic.TypeInformationLength;
			// this one never locks...
			while((uint)(Win32API.NtQueryObject(ipHandle, (int)Win32API.ObjectInformationClass.ObjectTypeInformation, ipObjectType, nLength, ref nLength)) == Win32API.STATUS_INFO_LENGTH_MISMATCH) {
				if(nLength == 0) {
					Logger.Warn("nLength returned at zero!");
					return null;
				}
				Marshal.FreeHGlobal(ipObjectType);
				ipObjectType = Marshal.AllocHGlobal(nLength);
			}

			objObjectType = (Win32API.OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(ipObjectType, objObjectType.GetType());
			if(Is64Bits()) {
				ipTemp = new IntPtr(Convert.ToInt64(objObjectType.Name.Buffer.ToString(), 10) >> 32);
			} else {
				ipTemp = objObjectType.Name.Buffer;
			}


			var strObjectTypeName = Marshal.PtrToStringUni(ipTemp, objObjectType.Name.Length >> 1);
			Marshal.FreeHGlobal(ipObjectType);
			if(strObjectTypeName != "File")
				return null;

			nLength = objBasic.NameInformationLength;

			var ipObjectName = Marshal.AllocHGlobal(nLength);

			// ...this call sometimes hangs. Is a Windows error.
			while((uint)(Win32API.NtQueryObject(ipHandle, (int)Win32API.ObjectInformationClass.ObjectNameInformation, ipObjectName, nLength, ref nLength)) == Win32API.STATUS_INFO_LENGTH_MISMATCH) {
				Marshal.FreeHGlobal(ipObjectName);
				if(nLength == 0) {
					Console.WriteLine("nLength returned at zero! " + strObjectTypeName);
					return null;
				}
				ipObjectName = Marshal.AllocHGlobal(nLength);
			}
			objObjectName = (Win32API.OBJECT_NAME_INFORMATION)Marshal.PtrToStructure(ipObjectName, objObjectName.GetType());

			if(Is64Bits()) {
				ipTemp = new IntPtr(Convert.ToInt64(objObjectName.Name.Buffer.ToString(), 10) >> 32);
			} else {
				ipTemp = objObjectName.Name.Buffer;
			}

			if(ipTemp != IntPtr.Zero) {

				var baTemp = new byte[nLength];
				try {
					Marshal.Copy(ipTemp, baTemp, 0, nLength);

					strObjectName = Marshal.PtrToStringUni(Is64Bits() ? new IntPtr(ipTemp.ToInt64()) : new IntPtr(ipTemp.ToInt32()));
				} catch(AccessViolationException) {
					return null;
				} finally {
					Marshal.FreeHGlobal(ipObjectName);
					Win32API.CloseHandle(ipHandle);
				}
			}

			string path = GetRegularFileNameFromDevice(strObjectName);
			try {
				return path;
			} catch {
				return null;
			}
		}

		private static string GetRegularFileNameFromDevice(string strRawName) {
			string strFileName = strRawName;
			foreach(string strDrivePath in Environment.GetLogicalDrives()) {
				var sbTargetPath = new StringBuilder(Win32API.MAX_PATH);
				if(Win32API.QueryDosDevice(strDrivePath.Substring(0, 2), sbTargetPath, Win32API.MAX_PATH) == 0) {
					return strRawName;
				}
				string strTargetPath = sbTargetPath.ToString();
				if(strFileName.StartsWith(strTargetPath)) {
					strFileName = strFileName.Replace(strTargetPath, strDrivePath.Substring(0, 2));
					break;
				}
			}
			return strFileName;
		}

		private static IEnumerable<Win32API.SYSTEM_HANDLE_INFORMATION> GetHandles(Process process) {
			var nHandleInfoSize = 0x10000;
			var ipHandlePointer = Marshal.AllocHGlobal(nHandleInfoSize);
			var nLength = 0;
			IntPtr ipHandle;

			while(Win32API.NtQuerySystemInformation(CNST_SYSTEM_HANDLE_INFORMATION, ipHandlePointer, nHandleInfoSize, ref nLength) == Win32API.STATUS_INFO_LENGTH_MISMATCH) {
				nHandleInfoSize = nLength;
				Marshal.FreeHGlobal(ipHandlePointer);
				ipHandlePointer = Marshal.AllocHGlobal(nLength);
			}

			var baTemp = new byte[nLength];
			Marshal.Copy(ipHandlePointer, baTemp, 0, nLength);

			long lHandleCount;
			if(Is64Bits()) {
				lHandleCount = Marshal.ReadInt64(ipHandlePointer);
				ipHandle = new IntPtr(ipHandlePointer.ToInt64() + 8);
			} else {
				lHandleCount = Marshal.ReadInt32(ipHandlePointer);
				ipHandle = new IntPtr(ipHandlePointer.ToInt32() + 4);
			}

			var lstHandles = new List<Win32API.SYSTEM_HANDLE_INFORMATION>();

			for(long lIndex = 0; lIndex < lHandleCount; lIndex++) {
				var shHandle = new Win32API.SYSTEM_HANDLE_INFORMATION();
				if(Is64Bits()) {
					shHandle = (Win32API.SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(ipHandle, shHandle.GetType());
					ipHandle = new IntPtr(ipHandle.ToInt64() + Marshal.SizeOf(shHandle) + 8);
				} else {
					ipHandle = new IntPtr(ipHandle.ToInt64() + Marshal.SizeOf(shHandle));
					shHandle = (Win32API.SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(ipHandle, shHandle.GetType());
				}
				if(shHandle.ProcessID != process.Id) continue;
				lstHandles.Add(shHandle);
			}
			return lstHandles;

		}

		private static bool Is64Bits() {
			return Marshal.SizeOf(typeof(IntPtr)) == 8;
		}

		internal class Win32API {
			[DllImport("ntdll.dll")]
			public static extern int NtQueryObject(IntPtr ObjectHandle, int
				ObjectInformationClass, IntPtr ObjectInformation, int ObjectInformationLength,
				ref int returnLength);

			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

			[DllImport("ntdll.dll")]
			public static extern uint NtQuerySystemInformation(int
				SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength,
				ref int returnLength);

			[DllImport("kernel32.dll")]
			public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
			[DllImport("kernel32.dll")]
			public static extern int CloseHandle(IntPtr hObject);
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
			   ushort hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle,
			   uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);
			[DllImport("kernel32.dll")]
			public static extern IntPtr GetCurrentProcess();

			public enum ObjectInformationClass {
				ObjectBasicInformation = 0,
				ObjectNameInformation = 1,
				ObjectTypeInformation = 2,
				ObjectAllTypesInformation = 3,
				ObjectHandleInformation = 4
			}

			[Flags]
			public enum ProcessAccessFlags : uint {
				All = 0x001F0FFF,
				Terminate = 0x00000001,
				CreateThread = 0x00000002,
				VMOperation = 0x00000008,
				VMRead = 0x00000010,
				VMWrite = 0x00000020,
				DupHandle = 0x00000040,
				SetInformation = 0x00000200,
				QueryInformation = 0x00000400,
				Synchronize = 0x00100000
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct OBJECT_BASIC_INFORMATION { // Information Class 0
				public int Attributes;
				public int GrantedAccess;
				public int HandleCount;
				public int PointerCount;
				public int PagedPoolUsage;
				public int NonPagedPoolUsage;
				public int Reserved1;
				public int Reserved2;
				public int Reserved3;
				public int NameInformationLength;
				public int TypeInformationLength;
				public int SecurityDescriptorLength;
				public System.Runtime.InteropServices.ComTypes.FILETIME CreateTime;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct OBJECT_TYPE_INFORMATION { // Information Class 2
				public UNICODE_STRING Name;
				public int ObjectCount;
				public int HandleCount;
				public int Reserved1;
				public int Reserved2;
				public int Reserved3;
				public int Reserved4;
				public int PeakObjectCount;
				public int PeakHandleCount;
				public int Reserved5;
				public int Reserved6;
				public int Reserved7;
				public int Reserved8;
				public int InvalidAttributes;
				public GENERIC_MAPPING GenericMapping;
				public int ValidAccess;
				public byte Unknown;
				public byte MaintainHandleDatabase;
				public int PoolType;
				public int PagedPoolUsage;
				public int NonPagedPoolUsage;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct OBJECT_NAME_INFORMATION { // Information Class 1
				public UNICODE_STRING Name;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct UNICODE_STRING {
				public ushort Length;
				public ushort MaximumLength;
				public IntPtr Buffer;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct GENERIC_MAPPING {
				public int GenericRead;
				public int GenericWrite;
				public int GenericExecute;
				public int GenericAll;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct SYSTEM_HANDLE_INFORMATION { // Information Class 16
				public int ProcessID;
				public byte ObjectTypeNumber;
				public byte Flags; // 0x01 = PROTECT_FROM_CLOSE, 0x02 = INHERIT
				public ushort Handle;
				public int Object_Pointer;
				public UInt32 GrantedAccess;
			}

			public const int MAX_PATH = 260;
			public const uint STATUS_INFO_LENGTH_MISMATCH = 0xC0000004;
			public const int DUPLICATE_SAME_ACCESS = 0x2;
			public const uint FILE_SEQUENTIAL_ONLY = 0x00000004;
		}
		#endregion
	}
}
