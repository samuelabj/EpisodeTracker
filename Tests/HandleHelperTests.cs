using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Models;
using Xunit;

namespace EpisodeTracker.Tests {
	public class HandleHelperTests {

		[Fact]
		public void ParseReturnsPotPlayer() {
			var output = @"
Handle v3.5
Copyright (C) 1997-2012 Mark Russinovich
Sysinternals - www.sysinternals.com

------------------------------------------------------------------------------
PotPlayerMini64.exe pid: 7748 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   80: Section       \Windows\Theme577161652
   84: Section       \Sessions\1\Windows\Theme2445879050
   9C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   E4: File  (R-D)   C:\Windows\System32\en-US\msvfw32.dll.mui
  13C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  168: Section       \Sessions\1\BaseNamedObjects\_Daum_Street_Mem_TRANS_AUTH64_
  178: Section       \Sessions\1\BaseNamedObjects\_Daum_Street_Mem1_ID64_
  1FC: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
  200: File  (R-D)   C:\Windows\System32\en-US\wdmaud.drv.mui
  260: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  274: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  2A4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  35C: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  388: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  398: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  3BC: Section       \BaseNamedObjects\__ComCatalogCache__
  3D4: Section       \BaseNamedObjects\__ComCatalogCache__
  3D8: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  444: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  49C: File  (R-D)   C:\Windows\System32\en-US\devenum.dll.mui
  4A0: File  (R-D)   C:\Windows\System32\en-US\dsound.dll.mui
  664: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  680: Section       \Sessions\1\BaseNamedObjects\AMResourceMapping3-0x09-0x00064a
  6B0: File  (RWD)   D:\Downloads\Media\Archer (2009) - S03E07 - Crossing Over.avi
  6B4: File  (RWD)   D:\Downloads\Media\Archer (2009) - S03E07 - Crossing Over.avi
  760: Section       \Sessions\1\BaseNamedObjects\1e44
  854: Section       \Sessions\1\BaseNamedObjects\DirectSound Administrator shared thread array
  9B0: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
";
			var list = HandleHelper.ParseOutput(output);
			Assert.Equal(1, list.Count);
			var proc = list.First();
			Assert.Equal("PotPlayerMini64", proc.ProcessName);
			Assert.Equal(7748, proc.PID);
			Assert.Equal(2, proc.Files.Count(f => f == @"D:\Downloads\Media\Archer (2009) - S03E07 - Crossing Over.avi"));
		}

		[Fact]
		public void ParseReturnsMultiple() {
			var output = @"
Handle v3.5
Copyright (C) 1997-2012 Mark Russinovich
Sysinternals - www.sysinternals.com

------------------------------------------------------------------------------
System pid: 4 \<unable to open process>
    8: File  (---)   C:\Windows\System32\config\SOFTWARE.LOG1
   1C: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b1-2165-11e2-be68-806e6f6e6963}.TMContainer00000000000000000001.regtrans-ms
   28: File  (---)   C:\Windows\System32\config\SOFTWARE
   50: File  (---)   C:\Windows\System32\config\SYSTEM
   54: File  (R-D)   C:\Windows\System32\wdi\LogFiles\WdiContextLog.etl.002
   60: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTEventLog-System.etl
   AC: File  (---)   C:\System Volume Information\{3808876b-c176-4e48-b7ae-04046e6cc752}
   B0: File  (---)   C:\System Volume Information\{6bab3d37-1fc2-11e2-be67-0090f5bf50ed}{3808876b-c176-4e48-b7ae-04046e6cc752}
   C8: File  (---)   C:\Windows\System32\config\DEFAULT
   DC: File  (---)   C:\Windows\System32\config\DEFAULT.LOG1
   E4: File  (---)   C:\Windows\System32\config\DEFAULT.LOG2
   F0: File  (---)   C:\System Volume Information\{22ff4aab-20eb-11e2-be67-0090f5bf50ed}{3808876b-c176-4e48-b7ae-04046e6cc752}
   F4: File  (---)   C:\Windows\System32\config\RegBack\DEFAULT
   F8: File  (---)   C:\hiberfil.sys
   FC: File  (---)   C:\Windows\System32\config\RegBack\SYSTEM
  104: File  (---)   C:\Windows\System32\config\SYSTEM.LOG1
  10C: File  (---)   \Device\Mup
  110: File  (---)   C:\Windows\System32\config\RegBack\SOFTWARE
  140: File  (R--)   D:\$Extend\$RmMetadata\$TxfLog\$TxfLog.blf
  144: File  (R--)   D:\$Extend\$RmMetadata\$TxfLog\$TxfLogContainer00000000000000000001
  148: File  (---)   C:\Windows\System32\config\SOFTWARE.LOG2
  150: File  (---)   C:\System Volume Information\{4fecc59b-22f2-11e2-be6a-0090f5bf50ed}{3808876b-c176-4e48-b7ae-04046e6cc752}
  160: File  (RW-)   \clfs
  164: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTUBPM.etl
  20C: File  (R--)   C:\$Extend\$RmMetadata\$TxfLog\$TxfLogContainer00000000000000000001
  210: File  (R--)   C:\$Extend\$RmMetadata\$TxfLog\$TxfLog.blf
  214: File  (R--)   \clfs
  21C: File  (RWD)   \clfs
  220: File  (R--)   C:\$Extend\$RmMetadata\$TxfLog\$TxfLogContainer00000000000000000002
  224: File  (RWD)   \clfs
  228: File  (RWD)   C:\$Extend\$RmMetadata\$Txf
  234: File  (RWD)   \clfs
  260: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTDiagLog.etl
  268: File  (R--)   D:\$Extend\$RmMetadata\$TxfLog\$TxfLogContainer00000000000000000002
  26C: File  (RWD)   \clfs
  270: File  (R--)   \clfs
  274: File  (RWD)   D:\$Extend\$RmMetadata\$Txf
  278: File  (RWD)   \clfs
  27C: File  (RWD)   \clfs
  280: File  (RW-)   \clfs
  29C: File  (RWD)   \clfs
  2A0: File  (R--)   \clfs
  2A8: File  (RWD)   \clfs
  2AC: File  (RWD)   \clfs
  2B0: File  (RW-)   \clfs
  2C0: File  (---)   C:\Windows\System32\config\SYSTEM.LOG2
  2C4: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTRAC_PS.etl
  2C8: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b1-2165-11e2-be68-806e6f6e6963}.TM.blf
  2CC: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b1-2165-11e2-be68-806e6f6e6963}.TMContainer00000000000000000002.regtrans-ms
  2D8: File  (RWD)   \clfs
  2DC: File  (RW-)   \clfs
  2FC: File  (R-D)   C:\Windows\System32\Drivers\en-US\USBXHCI.SYS.mui
  478: File  (-W-)   C:\pagefile.sys
  484: File  (---)   C:\Windows\bootstat.dat
  5B4: File  (RWD)   \clfs
  5B8: File  (RW-)   \clfs
  5CC: File  (---)   C:\Windows\ServiceProfiles\NetworkService\NTUSER.DAT.LOG2
  5D0: File  (R--)   C:\Windows\ServiceProfiles\NetworkService\NTUSER.DAT{92ee40c0-2165-11e2-be68-806e6f6e6963}.TMContainer00000000000000000002.regtrans-ms
  5EC: File  (---)   C:\Windows\System32\config\RegBack\SECURITY
  5F0: File  (---)   C:\Windows\System32\config\SECURITY.LOG2
  5F4: File  (---)   C:\Windows\System32\config\SECURITY.LOG1
  5F8: File  (---)   C:\Windows\System32\config\SECURITY
  5FC: File  (---)   C:\Windows\System32\config\RegBack\SAM
  630: File  (R-D)   C:\Windows\System32\en-GB\win32k.sys.mui
  6A0: File  (-W-)   C:\swapfile.sys
  6DC: File  (---)   C:\Windows\System32\config\SAM
  6EC: File  (---)   C:\Windows\System32\config\SAM.LOG1
  6F0: File  (---)   C:\Windows\System32\config\SAM.LOG2
  770: File  (R--)   C:\Windows\ServiceProfiles\NetworkService\NTUSER.DAT{92ee40c0-2165-11e2-be68-806e6f6e6963}.TMContainer00000000000000000001.regtrans-ms
  774: File  (---)   C:\Windows\ServiceProfiles\NetworkService\NTUSER.DAT
  778: File  (---)   C:\Windows\ServiceProfiles\NetworkService\NTUSER.DAT.LOG1
  77C: File  (R--)   C:\Windows\ServiceProfiles\NetworkService\NTUSER.DAT{92ee40c0-2165-11e2-be68-806e6f6e6963}.TM.blf
  B5C: File  (---)   C:\Windows\System32\config\BBI
  B64: File  (---)   C:\Windows\System32\config\BBI.LOG1
  B68: File  (---)   C:\Windows\System32\config\BBI.LOG2
  B6C: File  (R--)   C:\Windows\ServiceProfiles\LocalService\NTUSER.DAT{92ee40c8-2165-11e2-be68-88532e189d6d}.TM.blf
  B70: File  (---)   C:\Windows\ServiceProfiles\LocalService\NTUSER.DAT
  B74: File  (---)   C:\Windows\ServiceProfiles\LocalService\NTUSER.DAT.LOG1
  B78: File  (---)   C:\Windows\ServiceProfiles\LocalService\NTUSER.DAT.LOG2
  B7C: File  (R--)   C:\Windows\ServiceProfiles\LocalService\NTUSER.DAT{92ee40c8-2165-11e2-be68-88532e189d6d}.TMContainer00000000000000000001.regtrans-ms
  B80: File  (R--)   C:\Windows\ServiceProfiles\LocalService\NTUSER.DAT{92ee40c8-2165-11e2-be68-88532e189d6d}.TMContainer00000000000000000002.regtrans-ms
  B84: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTEventlog-Security.etl
  B8C: File  (RWD)   \clfs
  B90: File  (RW-)   \clfs
  CF8: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTEventLog-Application.etl
  F78: File  (---)   \Device\Mup
  F88: File  (---)   \Device\Mup
  F90: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTWFP-IPsec Diagnostics.etl
  F94: File  (R-D)   C:\Windows\System32\wfp\wfpdiag.etl
 14F8: File  (---)   C:\Users\Sam\ntuser.dat.LOG2
 1874: File  (R-D)   C:\Windows\ServiceProfiles\NetworkService\msmqlog.bin
 3464: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Support\MpWppTracing-11032012-114311-00000003-ffffffff.bin
 4168: File  (RW-)   \clfs
 41EC: File  (R-D)   C:\Windows\System32\LogFiles\SQM\SQMLogger.etl.008
 4230: File  (---)   C:\Users\Sam\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG1
 4248: File  (---)   C:\Windows\AppCompat\Programs\Amcache.hve.LOG1
 425C: File  (R-D)   C:\Windows\System32\LogFiles\AIT\AitEventLog.etl.002
 4264: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\UsrClass.dat{c29e704e-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000002.regtrans-ms
 4268: File  (---)   C:\Windows\AppCompat\Programs\Amcache.hve.LOG2
 427C: File  (RWD)   \clfs
 4280: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\UsrClass.dat{c29e704e-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000001.regtrans-ms
 4298: File  (---)   C:\Windows\AppCompat\Programs\Amcache.hve
 43C4: File  (---)   C:\Users\Sam\ntuser.dat.LOG1
 43D8: File  (R--)   C:\Users\Sam\NTUSER.DAT{c29e704a-2165-11e2-be69-0090f5bf50ed}.TM.blf
 43DC: File  (---)   C:\Users\Sam\NTUSER.DAT
 43E0: File  (R--)   C:\Users\Sam\NTUSER.DAT{c29e704a-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000001.regtrans-ms
 43E4: File  (R--)   C:\Users\Sam\NTUSER.DAT{c29e704a-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000002.regtrans-ms
 43EC: File  (RWD)   \clfs
 43F0: File  (RW-)   \clfs
 43F4: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\UsrClass.dat{c29e704e-2165-11e2-be69-0090f5bf50ed}.TM.blf
 43F8: File  (---)   C:\Users\Sam\AppData\Local\Microsoft\Windows\UsrClass.dat
 43FC: File  (---)   C:\Users\Sam\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG2
 4470: File  (R--)   C:\Users\UpdatusUser\AppData\Local\Microsoft\Windows\UsrClass.dat{c29e70ef-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000001.regtrans-ms
 447C: File  (RWD)   \clfs
 45B4: File  (RW-)   \clfs
 4E60: File  (---)   C:\Users\UpdatusUser\ntuser.dat.LOG1
 4EB0: File  (RWD)   C:?
 4EB8: File  (R--)   C:\Users\UpdatusUser\AppData\Local\Microsoft\Windows\UsrClass.dat{c29e70ef-2165-11e2-be69-0090f5bf50ed}.TM.blf
 4EBC: File  (---)   C:\Users\UpdatusUser\NTUSER.DAT
 4EC0: File  (RWD)   \clfs
 4EC8: File  (RWD)   C:\Windows\CSC\v2.0.6\pq
 4ECC: File  (---)   C:\Users\UpdatusUser\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG2
 4F14: File  (RWD)   C:\Windows\CSC\v2.0.6\temp
 4F18: File  (RW-)   C:\Windows\CSC\v2.0.6
 4F1C: File  (R--)   C:\Users\UpdatusUser\AppData\Local\Microsoft\Windows\UsrClass.dat{c29e70ef-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000002.regtrans-ms
 4F2C: File  (---)   C:\Users\UpdatusUser\AppData\Local\Microsoft\Windows\UsrClass.dat.LOG1
 4F84: File  (R--)   C:\Users\UpdatusUser\NTUSER.DAT{c29e70eb-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000002.regtrans-ms
 4FA0: File  (RW-)   \clfs
 4FA8: File  (---)   C:\Users\UpdatusUser\AppData\Local\Microsoft\Windows\UsrClass.dat
 4FAC: File  (RW-)   C:\Windows\CSC
 4FC8: File  (RW-)   C:\Windows\CSC\v2.0.6\namespace
 4FD0: File  (R--)   C:\Users\UpdatusUser\NTUSER.DAT{c29e70eb-2165-11e2-be69-0090f5bf50ed}.TMContainer00000000000000000001.regtrans-ms
 4FD4: File  (R--)   C:\Users\UpdatusUser\NTUSER.DAT{c29e70eb-2165-11e2-be69-0090f5bf50ed}.TM.blf
 4FDC: File  (---)   C:\Users\UpdatusUser\ntuser.dat.LOG2
 55FC: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b0-2165-11e2-be68-806e6f6e6963}.TxR.2.regtrans-ms
 5600: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b0-2165-11e2-be68-806e6f6e6963}.TxR.1.regtrans-ms
 5608: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b0-2165-11e2-be68-806e6f6e6963}.TxR.blf
 5A30: File  (R-D)   C:\Windows\System32\LogFiles\WMI\RtBackup\EtwRTSteam Event Tracing.etl
 5B90: File  (R--)   C:\Windows\System32\config\TxR\{92ee40b0-2165-11e2-be68-806e6f6e6963}.TxR.0.regtrans-ms
 5B94: File  (---)   \clfs
------------------------------------------------------------------------------
smss.exe pid: 360 NT AUTHORITY\SYSTEM
    4: File  (RW-)   C:\Windows
------------------------------------------------------------------------------
csrss.exe pid: 568 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   2C: Section       \Windows\SharedSection
   3C: File  (R-D)   C:\Windows\System32\en-US\csrss.exe.mui
------------------------------------------------------------------------------
wininit.exe pid: 660 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   74: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
------------------------------------------------------------------------------
csrss.exe pid: 676 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   2C: Section       \Sessions\1\Windows\SharedSection
------------------------------------------------------------------------------
winlogon.exe pid: 720 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   28: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   F0: Section       \Sessions\1\Windows\ThemeSection
  10C: Section       \Sessions\1\Windows\Theme2445879050
  120: Section       \Windows\Theme577161652
  1E8: Section       \...\ASqmManifestVersion
  1F0: Section       \...\ASqmManifest_27c6
------------------------------------------------------------------------------
services.exe pid: 760 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
------------------------------------------------------------------------------
lsass.exe pid: 768 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   44: Section       \BaseNamedObjects\Debug.Memory.v2.300
   7C: Section       \LsaPerformance
  1F8: Section       \BaseNamedObjects\Debug.Trace.Memory.300
  324: File  (RW-)   C:\Windows\debug\PASSWD.LOG
  734: File  (R-D)   C:\Windows\System32\en-GB\vaultsvc.dll.mui
  CD8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  DE4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Credentials
  DE8: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Credentials
  FB0: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\SystemCertificates\My
------------------------------------------------------------------------------
svchost.exe pid: 876 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
  1C0: Section       \BaseNamedObjects\__ComCatalogCache__
  214: Section       \BaseNamedObjects\RotHintTable
  21C: Section       \BaseNamedObjects\{A64C7F33-DA35-459b-96CA-63B51FB0CDB9}
  430: Section       \BaseNamedObjects\__ComCatalogCache__
  444: Section       \BaseNamedObjects\__ComCatalogCache__
  448: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  4A8: Section       \BaseNamedObjects\__ComCatalogCache__
  518: File  (R-D)   C:\Windows\System32\en-US\lsm.dll.mui
  530: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
  534: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
------------------------------------------------------------------------------
nvvsvc.exe pid: 908 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   30: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  184: Section       \BaseNamedObjects\__ComCatalogCache__
  244: Section       \BaseNamedObjects\__ComCatalogCache__
  248: File  (R--)   C:\Windows\Registration\R00000000000d.clb
------------------------------------------------------------------------------
nvSCPAPISvr.exe pid: 936 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   74: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
------------------------------------------------------------------------------
svchost.exe pid: 980 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
  178: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
  1B0: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
  26C: Section       \BaseNamedObjects\__ComCatalogCache__
  288: Section       \BaseNamedObjects\__ComCatalogCache__
  28C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  668: Section       \BaseNamedObjects\RotHintTable
------------------------------------------------------------------------------
svchost.exe pid: 348 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   64: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   6C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   D8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-WLAN-AutoConfig%4Operational.evtx
   E0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Windows Firewall With Advanced Security%4Firewall.evtx
  16C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Windows Firewall With Advanced Security%4ConnectionSecurity.evtx
  1B4: File  (R--)   C:\Windows\System32\winevt\Logs\System.evtx
  1B8: File  (R--)   C:\Windows\System32\winevt\Logs\Application.evtx
  1D8: File  (R--)   C:\Windows\System32\winevt\Logs\Red Gate Software.evtx
  1E0: File  (R--)   C:\Windows\System32\winevt\Logs\Security.evtx
  1FC: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Windows Defender%4Operational.evtx
  204: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Application-Experience%4Program-Compatibility-Assistant.evtx
  208: File  (R--)   C:\Windows\System32\winevt\Logs\Windows PowerShell.evtx
  20C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Windows Defender%4WHC.evtx
  214: File  (R--)   C:\Windows\System32\winevt\Logs\Key Management Service.evtx
  218: File  (R--)   C:\Windows\System32\winevt\Logs\PreEmptive.evtx
  21C: File  (R--)   C:\Windows\System32\winevt\Logs\Internet Explorer.evtx
  220: File  (R--)   C:\Windows\System32\winevt\Logs\PowerBiosServerLog.evtx
  224: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Ntfs%4Operational.evtx
  228: File  (R--)   C:\Windows\System32\winevt\Logs\OutLog.evtx
  22C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-WHEA%4Errors.evtx
  230: File  (R--)   C:\Windows\System32\winevt\Logs\OAlerts.evtx
  238: File  (R--)   C:\Windows\System32\winevt\Logs\Media Center.evtx
  23C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-NetworkProfile%4Operational.evtx
  240: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-Power%4Thermal-Operational.evtx
  244: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-EventTracing%4Admin.evtx
  248: File  (R--)   C:\Windows\System32\winevt\Logs\HardwareEvents.evtx
  24C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnosis-PCW%4Operational.evtx
  250: File  (R--)   C:\Windows\System32\winevt\Logs\Operational.evtx
  254: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Wcmsvc%4Operational.evtx
  260: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Ntfs%4WHC.evtx
  264: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Application-Experience%4Program-Compatibility-Troubleshooter.evtx
  268: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Application-Experience%4Steps-Recorder.evtx
  26C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Application-Experience%4Program-Telemetry.evtx
  270: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Application-Experience%4Program-Inventory.evtx
  274: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-WHEA%4Operational.evtx
  278: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-ShimEngine%4Operational.evtx
  284: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-DriverFrameworks-UserMode%4Operational.evtx
  294: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-PnP%4Configuration.evtx
  2D4: Section       \BaseNamedObjects\__ComCatalogCache__
  2EC: Section       \BaseNamedObjects\__ComCatalogCache__
  2F0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  30C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-PushNotification-Platform%4Operational.evtx
  388: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-LanguagePackSetup%4Operational.evtx
  3D0: Section       \BaseNamedObjects\mmGlobalPnpInfo
  4F8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-AppHost%4Admin.evtx
  530: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-DeviceSetupManager%4Admin.evtx
  59C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-WindowsBackup%4ActionCenter.evtx
  5A8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-User Profile Service%4Operational.evtx
  5D0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-TerminalServices-LocalSessionManager%4Operational.evtx
  5D8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-TerminalServices-LocalSessionManager%4Admin.evtx
  5E4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Authentication User Interface%4Operational.evtx
  688: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-DeviceSetupManager%4Operational.evtx
  690: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-GroupPolicy%4Operational.evtx
  694: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Dhcp-Client%4Admin.evtx
  698: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Dhcpv6-Client%4Admin.evtx
  6B4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-AppXDeploymentServer%4Operational.evtx
  744: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-StoreMgr%4Operational.evtx
  7B0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-UserPnp%4DeviceInstall.evtx
  82C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Audio%4Operational.evtx
  834: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Audio%4CaptureMonitor.evtx
  83C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Audio%4PlaybackManager.evtx
  88C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-NCSI%4Operational.evtx
  8B4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-HotspotAuth%4Operational.evtx
  8F4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Shell-ConnectedAccountState%4ActionCenter.evtx
  91C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-WMI-Activity%4Operational.evtx
  950: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-StorageSpaces-ManagementAgent%4WHC.evtx
  954: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-LiveId%4Operational.evtx
  99C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-TWinUI%4Operational.evtx
  9A0: File  (R-D)   C:\Windows\System32\en-US\provsvc.dll.mui
  9B4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Shell-Core%4ActionCenter.evtx
  9B8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Shell-Core%4Operational.evtx
  9C0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnosis-DPS%4Operational.evtx
  9D4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Resource-Exhaustion-Detector%4Operational.evtx
  9E0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Winlogon%4Operational.evtx
  9F0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-BackgroundTaskInfrastructure%4Operational.evtx
  9F4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-CertificateServicesClient-Lifecycle-User%4Operational.evtx
  9F8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-PushNotification-Platform%4Admin.evtx
  A00: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Bits-Client%4Operational.evtx
  A2C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-HomeGroup Control Panel%4Operational.evtx
  A30: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-HomeGroup Provider Service%4Operational.evtx
  A4C: Section       \BaseNamedObjects\windows_shell_global_counters
  AA8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-UserPnp%4ActionCenter.evtx
  ACC: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-SettingSync%4Operational.evtx
  B20: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-ReadyBoost%4Operational.evtx
  B5C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-SettingSync%4Debug.evtx
  B88: File  (RW-)   C:\Windows\ServiceProfiles\LocalService\AppData\Local\Microsoft\Windows\WindowsUpdate.log
  BCC: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-NetworkAccessProtection%4WHC.evtx
  BD4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnosis-Scheduled%4Operational.evtx
  BD8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-TaskScheduler%4Maintenance.evtx
  BE4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Security-SPP-UX-Notifications%4ActionCenter.evtx
  BE8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-FileHistory-Core%4WHC.evtx
  C70: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnosis-Scripted%4Admin.evtx
  C74: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnosis-Scripted%4Operational.evtx
  CCC: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Resource-Leak-Diagnostic%4Operational.evtx
  CE0: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnostics-Performance%4Operational.evtx
  CF8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-CodeIntegrity%4Operational.evtx
  D00: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Diagnostics-Networking%4Operational.evtx
  D38: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Kernel-PnPConfig%4Configuration.evtx
  D48: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-WindowsUpdateClient%4Operational.evtx
  D74: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Fault-Tolerant-Heap%4Operational.evtx
  D78: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-ReliabilityAnalysisComponent%4Operational.evtx
  D7C: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Resource-Exhaustion-Resolver%4Operational.evtx
  EA8: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-WER-Diag%4Operational.evtx
  F88: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-Known Folders API Service.evtx
  FF4: File  (R--)   C:\Windows\System32\winevt\Logs\Microsoft-Windows-RestartManager%4Operational.evtx
------------------------------------------------------------------------------
svchost.exe pid: 404 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   6C: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   70: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  1EC: Section       \BaseNamedObjects\__ComCatalogCache__
  28C: Section       \BaseNamedObjects\__ComCatalogCache__
  290: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  2AC: Section       \Windows\Theme577161652
  2B8: Section       \BaseNamedObjects\SENS Information Cache
  434: File  (RW-)   C:
  43C: File  (RW-)   C:\Windows\Tasks
  6DC: File  (RWD)   C:\Windows\System32\wbem\MOF
  744: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  758: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  75C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  764: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  AC0: Section       \BaseNamedObjects\SqmData_FwtSqmSession101457921_S-1-5-18
  D50: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
  EB4: File  (R-D)   C:\Windows\System32\en-US\activeds.dll.mui
  EE0: File  (R-D)   C:\Windows\System32\en-US\vsstrace.dll.mui
  F50: File  (R--)   C:\Windows\System32\wbem\Repository\MAPPING2.MAP
  F54: File  (R--)   C:\Windows\System32\wbem\Repository\MAPPING1.MAP
  F58: File  (R--)   C:\Windows\System32\wbem\Repository\OBJECTS.DATA
  F5C: File  (R--)   C:\Windows\System32\wbem\Repository\MAPPING3.MAP
  F60: File  (R--)   C:\Windows\System32\wbem\Repository\INDEX.BTR
  F78: Section       \BaseNamedObjects\Wmi Provider Sub System Counters
 1098: Section       \BaseNamedObjects\windows_shell_global_counters
 14F0: File  (R-D)   C:\Windows\System32\en-US\winhttp.dll.mui
 15AC: Section       \BaseNamedObjects\MMF_BITS_s
 16BC: File  (R--)   C:\ProgramData\Microsoft\Network\Downloader\qmgr0.dat
 1704: File  (R--)   C:\ProgramData\Microsoft\Network\Downloader\qmgr1.dat
------------------------------------------------------------------------------
svchost.exe pid: 424 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   6C: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   78: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   E0: File  (RWD)   C:\Windows\Fonts
  114: Section       \BaseNamedObjects\__ComCatalogCache__
  180: File  (RWD)   C:\Program Files (x86)\Sony\ReaderDesktop\deviceHelper\fonts
  1A4: File  (R-D)   C:\Windows\ServiceProfiles\LocalService\AppData\Local\~FontCache-FontFace.dat
  1B4: File  (R-D)   C:\Windows\ServiceProfiles\LocalService\AppData\Local\~FontCache-System.dat
  1BC: File  (RWD)   C:\Program Files (x86)\Common Files\Microsoft Shared\EQUATION
  1C8: File  (R-D)   C:\Windows\ServiceProfiles\LocalService\AppData\Local\~FontCache-S-1-5-18.dat
  254: File  (R-D)   C:\Windows\System32\es.dll
  27C: File  (R-D)   C:\Windows\System32\stdole2.tlb
  654: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  664: Section       \BaseNamedObjects\__ComCatalogCache__
  9F0: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
  A14: Section       \...\ASqmManifestVersion
  A1C: Section       \...\ASqmManifest_27c6
  B7C: File  (R-D)   C:\Windows\ServiceProfiles\LocalService\AppData\Local\~FontCache-S-1-5-21-2054895221-449969794-1663927727-1000.dat
  F0C: Section       \BaseNamedObjects\windows_shell_global_counters
 105C: File  (R-D)   C:\Windows\System32\en-US\dnsapi.dll.mui
------------------------------------------------------------------------------
dwm.exe pid: 808 Window Manager\DWM-1
    C: File  (RW-)   C:\Windows\System32
   48: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  228: Section       \BaseNamedObjects\__ComCatalogCache__
  23C: Section       \BaseNamedObjects\__ComCatalogCache__
  240: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  258: Section       \Windows\Theme577161652
  260: File  (R-D)   C:\Windows\System32\en-US\d2d1.dll.mui
  338: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  3A4: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
svchost.exe pid: 1040 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   64: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   68: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  114: Section       \BaseNamedObjects\__ComCatalogCache__
  128: Section       \BaseNamedObjects\__ComCatalogCache__
  134: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  378: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
  444: File  (R--)   D:\System Volume Information\tracking.log
  4E8: File  (---)   C:
  520: File  (---)   D:
  5C0: File  (RWD)   D:\$Extend\$ObjId
  65C: File  (R-D)   C:\Windows\System32\en-GB\wlansvc.dll.mui
  6D4: File  (RWD)   C:\$Extend\$ObjId
  6D8: File  (R--)   C:\System Volume Information\tracking.log
  99C: Section       \...\ASqmManifest_27c6
  9A4: Section       \...\ASqmManifestVersion
  A74: File  (R-D)   C:\Windows\System32\en-GB\rasdlg.dll.mui
  A9C: Section       \BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
NvXDSync.exe pid: 1096 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   38: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   90: Section       \BaseNamedObjects\__ComCatalogCache__
  14C: Section       \BaseNamedObjects\__ComCatalogCache__
  150: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  168: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
  170: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
  258: Section       \Windows\Theme577161652
  25C: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
nvvsvc.exe pid: 1116 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   1C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   38: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  13C: Section       \Windows\Theme577161652
  140: Section       \Sessions\1\Windows\Theme2445879050
  1C8: Section       \BaseNamedObjects\__ComCatalogCache__
  1E0: Section       \BaseNamedObjects\__ComCatalogCache__
  1E4: File  (R--)   C:\Windows\Registration\R00000000000d.clb
------------------------------------------------------------------------------
WUDFHost.exe pid: 1412 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   28: File  (R-D)   C:\Windows\System32\en-GB\WUDFHost.exe.mui
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
------------------------------------------------------------------------------
svchost.exe pid: 1504 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
   6C: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   70: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  150: File  (RWD)   C:\Windows\System32\Drivers\etc
  278: File  (---)   \Device\Mup
  294: File  (---)   \Device\Mup
  31C: File  (R-D)   C:\Windows\System32\en-US\dnsapi.dll.mui
  380: File  (R-D)   C:\Windows\System32\en-US\vsstrace.dll.mui
  3B4: Section       \BaseNamedObjects\__ComCatalogCache__
  3CC: Section       \BaseNamedObjects\__ComCatalogCache__
  3D0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  77C: File  (---)   C:\Windows\System32\catroot2\{F750E6C3-38EE-11D1-85E5-00C04FC295EE}\catdb
  7A8: File  (---)   C:\Windows\System32\catroot2\{127D0A1D-4EF2-11D1-8608-00C04FC295EE}\catdb
  7F8: File  (RWD)   C:\Users\Sam\AppData\LocalLow\Microsoft\CryptnetUrlCache\MetaData
  828: File  (RWD)   C:\Windows\System32\config\systemprofile\AppData\LocalLow\Microsoft\CryptnetUrlCache\MetaData
  8E4: File  (RWD)   C:\Windows\ServiceProfiles\LocalService\AppData\LocalLow\Microsoft\CryptnetUrlCache\MetaData
  8F0: File  (---)   C:\Windows\System32\catroot2\edb.log
  8FC: File  (R-D)   C:\Windows\System32\en-US\winhttp.dll.mui
  97C: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
------------------------------------------------------------------------------
spoolsv.exe pid: 1784 NT AUTHORITY\SYSTEM
   10: File  (RW-)   C:\Windows\System32
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   34: File  (R-D)   C:\Windows\System32\en-US\spoolsv.exe.mui
  240: File  (R-D)   C:\Windows\System32\en-GB\TCPMON.dll.mui
  3B0: Section       \BaseNamedObjects\__ComCatalogCache__
  3C8: Section       \BaseNamedObjects\__ComCatalogCache__
  3CC: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  4BC: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
  56C: File  (R-D)   C:\Windows\System32\en-US\inetpp.dll.mui
  784: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
------------------------------------------------------------------------------
svchost.exe pid: 1852 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   54: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   6C: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
  11C: File  (R-D)   C:\Windows\System32\en-US\bfe.dll.mui
  424: Section       \BaseNamedObjects\__ComCatalogCache__
  43C: Section       \BaseNamedObjects\__ComCatalogCache__
  440: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  6F0: Section       \...\ASqmManifestVersion
  704: Section       \...\ASqmManifest_27c6
  754: File  (R-D)   C:\Windows\System32\en-US\dnsapi.dll.mui
  8BC: File  (---)   \Device\Mup
  8D8: Section       \...\ASqmManifestVersion
  900: Section       \...\ASqmManifest_27c6
------------------------------------------------------------------------------
armsvc.exe pid: 1132 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   18: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   40: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
------------------------------------------------------------------------------
svchost.exe pid: 1388 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   D4: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   D8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  118: Section       \BaseNamedObjects\__ComCatalogCache__
  130: Section       \BaseNamedObjects\__ComCatalogCache__
  134: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  144: File  (RWD)   C:\Windows\System32\inetsrv\config
  14C: File  (R-D)   C:\Windows\System32\en-US\vsstrace.dll.mui
  150: File  (RWD)   C:\Windows\System32\inetsrv\config\schema
  168: File  (RWD)   C:\Windows\System32\inetsrv\config\schema
  170: File  (RWD)   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config
  178: File  (RWD)   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config
  17C: File  (RWD)   C:\Windows\System32\inetsrv\config
  180: File  (RWD)   C:\Windows\System32\inetsrv\config
------------------------------------------------------------------------------
dsNcService.exe pid: 1588 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   18: File  (RW-)   C:\Windows\SysWOW64
   64: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  114: Section       \BaseNamedObjects\Juniper.LogService.Settings.SharedMemory.v2
  17C: File  (R-D)   C:\Windows\SysWOW64\en-US\ws2_32.dll.mui
  194: File  (R-D)   C:\Windows\SysWOW64\en-GB\wininet.dll.mui
  198: File  (RWD)   C:\Users\Public\Juniper Networks\Logging\debuglog.log
  1D0: File  (R--)   C:\Users\Public\Juniper Networks\Logging\jsrvprt.log
  1D8: Section       \BaseNamedObjects\Juniper:NcIpc:SharedMemory
  1E8: Section       \BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
dasHost.exe pid: 2000 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
  3E8: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
  420: Section       \BaseNamedObjects\windows_shell_global_counters
  440: Section       \BaseNamedObjects\__ComCatalogCache__
  5B0: Section       \BaseNamedObjects\__ComCatalogCache__
  5B4: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  640: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
------------------------------------------------------------------------------
svchost.exe pid: 2124 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   64: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   68: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   FC: Section       \BaseNamedObjects\__ComCatalogCache__
  114: Section       \BaseNamedObjects\__ComCatalogCache__
  118: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  2A4: File  (R-D)   C:\Windows\System32\en-US\fdrespub.dll.mui
  2A8: File  (R-D)   C:\Windows\System32\en-GB\FunDisc.dll.mui
  35C: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
------------------------------------------------------------------------------
svchost.exe pid: 2152 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   AC: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   B0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  210: Section       \BaseNamedObjects\__ComCatalogCache__
  224: Section       \BaseNamedObjects\__ComCatalogCache__
  228: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  244: File  (RWD)   C:\Windows\System32\inetsrv\config\schema
  24C: File  (RWD)   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config
  250: File  (RWD)   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config
  254: File  (RWD)   C:\Windows\System32\inetsrv\config
  258: File  (RWD)   C:\Windows\System32\inetsrv\config
------------------------------------------------------------------------------
MsDepSvc.exe pid: 2340 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
   3C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   58: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   7C: Section       \BaseNamedObjects\Cor_Private_IPCBlock_2340
   80: Section       \BaseNamedObjects\Cor_Public_IPCBlock_2340
   D0: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\security.config.cch
   D4: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\enterprisesec.config.cch
  128: Section       \BaseNamedObjects\windows_shell_global_counters
  138: File  (R-D)   C:\Windows\ServiceProfiles\NetworkService\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\64bit\security.config.cch
  180: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_64\indexee.dat
  184: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
  188: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  454: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  45C: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  468: File  (R--)   C:\Windows\assembly\pubpol71.dat
  488: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  524: Section       \BaseNamedObjects\NLS_CodePage_1252_3_2_0_0
  52C: Section       \BaseNamedObjects\netfxcustomperfcounters.1.0.net clr networking
------------------------------------------------------------------------------
mqsvc.exe pid: 2412 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
   84: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  1E4: Section       \BaseNamedObjects\windows_shell_global_counters
  298: File  (R-D)   C:\Windows\System32\en-US\mqutil.dll.mui
  374: File  (R--)   C:\Windows\System32\msmq\storage\QMLog
  43C: File  (R-D)   C:\Windows\System32\en-US\vsstrace.dll.mui
  45C: Section       \BaseNamedObjects\__ComCatalogCache__
  474: Section       \BaseNamedObjects\__ComCatalogCache__
  478: File  (R--)   C:\Windows\Registration\R00000000000d.clb
------------------------------------------------------------------------------
sqlservr.exe pid: 2440 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
    C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   24: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   28: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   50: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  100: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  170: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\tempdb.mdf
  19C: Section       \BaseNamedObjects\SQL60_RUNNING
  1A4: File  (R--)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\Log\ERRORLOG
  280: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\model.mdf
  2C8: Section       \BaseNamedObjects\SQLCounters$MSSQLSERVER
  308: Section       \BaseNamedObjects\SQL_90_MEMOBJ_MSSQLSERVER_0
  440: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\mastlog.ldf
  454: Section       \BaseNamedObjects\SQL_90_MEMOBJ_MSSQLSERVER_1
  45C: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\master.mdf
  464: File  (R--)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\Log\log_120.trc
  470: File  (R--)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\Binn\mssqlsystemresource.ldf
  484: File  (R--)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\Binn\mssqlsystemresource.mdf
  4CC: Section       \BaseNamedObjects\windows_shell_global_counters
  52C: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\MSDBData.mdf
  53C: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\modellog.ldf
  544: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\MSDBLog.ldf
  548: File  (---)   C:\Program Files\Microsoft SQL Server\MSSQL10.MSSQLSERVER\MSSQL\DATA\templog.ldf
------------------------------------------------------------------------------
mysqld.exe pid: 2624 NT AUTHORITY\SYSTEM
    8: File  (RWD)   C:\ProgramData\MySQL\MySQL Server 5.5\data\Black-Beauty.err
   38: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   B8: File  (RW-)   C:\ProgramData\MySQL\MySQL Server 5.5\data
   BC: File  (RWD)   C:\Windows\Temp\ib7F71.tmp
  13C: File  (RWD)   C:\Windows\Temp\ib7F72.tmp
  148: File  (RWD)   C:\ProgramData\MySQL\MySQL Server 5.5\data\Black-Beauty.err
  15C: File  (RWD)   C:\Windows\Temp\ib7F73.tmp
  7E4: File  (RWD)   C:\Windows\Temp\ib7F83.tmp
  814: File  (R--)   C:\ProgramData\MySQL\MySQL Server 5.5\data\ibdata1
  818: File  (R--)   C:\ProgramData\MySQL\MySQL Server 5.5\data\ib_logfile0
  81C: File  (R--)   C:\ProgramData\MySQL\MySQL Server 5.5\data\ib_logfile1
  824: File  (RWD)   C:\Windows\Temp\ib7F94.tmp
------------------------------------------------------------------------------
sqlwriter.exe pid: 2972 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   5C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  108: File  (R-D)   C:\Windows\System32\en-US\vsstrace.dll.mui
  118: Section       \BaseNamedObjects\__ComCatalogCache__
  134: Section       \BaseNamedObjects\__ComCatalogCache__
  138: File  (R--)   C:\Windows\Registration\R00000000000d.clb
------------------------------------------------------------------------------
svchost.exe pid: 2992 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   94: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   98: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   C4: File  (RW-)   C:\Windows\debug\WIA\wiatrace.log
  13C: Section       \BaseNamedObjects\__ComCatalogCache__
  1AC: Section       \BaseNamedObjects\__ComCatalogCache__
  1B4: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  1E0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
------------------------------------------------------------------------------
TeamViewer_Service.exe pid: 3052 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   44: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
  1D8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
  A00: File  (RW-)   C:\Program Files (x86)\TeamViewer\Version7\TeamViewer7_Logfile.log
  C40: Section       \BaseNamedObjects\__ComCatalogCache__
  C50: Section       \BaseNamedObjects\__ComCatalogCache__
  C5C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  CBC: Section       \BaseNamedObjects\windows_shell_global_counters
  D0C: Section       \BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
svchost.exe pid: 2680 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   94: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
   98: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  16C: Section       \BaseNamedObjects\__ComCatalogCache__
  184: Section       \BaseNamedObjects\__ComCatalogCache__
  188: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  194: File  (RWD)   C:\Windows\System32\inetsrv\config\schema
  1A8: File  (RWD)   C:\Windows\System32\inetsrv\config
  1AC: File  (RWD)   C:\Windows\System32\inetsrv\config
  200: File  (R--)   C:\inetpub\temp\appPools\APC8618.tmp
  274: Section       \BaseNamedObjects\IISCounterControlBlock-46382a23-095e-4559-8d63-6fdeaf552c23
  27C: Section       \BaseNamedObjects\IISCacheCounters-fa95a2f9-d7d2-450f-8f5e-fdcaf62c867e
  280: Section       \BaseNamedObjects\IISCacheCounters-3225700a-33cf-430e-a1d7-3ddfc7f69099
  284: Section       \BaseNamedObjects\IISSitesCounters-21917d4d-3c79-40e9-a619-e1721f1bdfe3
  288: Section       \BaseNamedObjects\IISSitesCounters-ae69add7-ae78-48ff-9a89-2b6a931aa791
  294: Section       \BaseNamedObjects\ASP_PERFMON_MAIN_BLOCK
------------------------------------------------------------------------------
MsMpEng.exe pid: 1028 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   F4: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.7E
  154: File  (R--)   C:\ProgramData\Microsoft\Windows Defender\Support\MPLog-07262012-002211.log
  158: File  (R--)   C:\ProgramData\Microsoft\Windows Defender\Support\MPDetection-10272012-082853.log
  164: File  (---)   C:\ProgramData\Microsoft\Windows Defender\Scans\History\CacheManager\MpScanCache-0.bin
  16C: File  (RWD)   C:\ProgramData\Microsoft\Windows Defender\Definition Updates\Updates
  174: File  (RWD)   C:\Windows\System32\kernel32.dll
  17C: File  (RWD)   C:\Windows\System32\kernel32.dll
  1D8: File  (---)   C:\ProgramData\Microsoft\Windows Defender\Scans\MpDiag.bin
  224: Section       \BaseNamedObjects\MpSvcMemory-63107F42-5336-1DEF-B802-9FF15E0C0525
  22C: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.VE1
  230: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.80
  26C: File  (---)   \FileSystem\Filters\FltMgrMsg
  270: File  (---)   \FileSystem\Filters\FltMgrMsg
  280: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.87
  2AC: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.67
  2B0: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.A0
  2C4: File  (RWD)   C:
  2DC: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.VE0
  2E8: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.VE2
  2F0: File  (R-D)   C:\ProgramData\Microsoft\Windows Defender\Scans\mpcache-62E98F4A2C2F63209060B6EA87126AA5735F9023.bin.VF
  314: File  (---)   C:\ProgramData\Microsoft\Windows Defender\IMpService77BDAF73-B396-481F-9042-AD358843EC24.lock
  32C: File  (---)   C:\Program Files\Windows Defender\MsMpLics.dll
  330: File  (---)   C:\Program Files\Windows Defender\MpCmdRun.exe
  338: File  (---)   \FileSystem\Filters\FltMgrMsg
  33C: File  (---)   C:\Program Files\Windows Defender\MpCommu.dll
  344: File  (---)   \FileSystem\Filters\FltMgrMsg
  34C: File  (---)   \FileSystem\Filters\FltMgrMsg
  354: File  (---)   C:\Program Files\Windows Defender\MsMpCom.dll
  35C: File  (---)   C:\Program Files\Windows Defender\MpRtp.dll
  378: File  (---)   C:\Program Files\Windows Defender\MpOAV.dll
  380: File  (---)   C:\Program Files\Windows Defender\MpEvMsg.dll
  384: File  (---)   C:\Program Files\Windows Defender\MpAsDesc.dll
  3A0: File  (---)   C:\Windows\System32\Drivers\WdFilter.sys
  43C: Section       \BaseNamedObjects\UrlZonesSM_BLACK-BEAUTY$
  488: Section       \BaseNamedObjects\__ComCatalogCache__
  48C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  850: File  (RWD)   C:\Windows\System32\config\systemprofile\AppData\Roaming\Microsoft\SystemCertificates\My
------------------------------------------------------------------------------
SMSvcHost.exe pid: 2100 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   88: Section       \BaseNamedObjects\Cor_Private_IPCBlock_v4_2100
  1C8: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\SMSvcHost.exe
  1D0: File  (R--)   C:\Windows\assembly\pubpol71.dat
------------------------------------------------------------------------------
SMSvcHost.exe pid: 3424 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   84: Section       \BaseNamedObjects\Cor_Private_IPCBlock_v4_3424
  1C4: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v4.0.30319\SMSvcHost.exe
  1CC: File  (R--)   C:\Windows\assembly\pubpol71.dat
  294: Section       \BaseNamedObjects\net.pipe:EbmV0LnBpcGU6Ly8rL0IwNTgxNEM4LTg3REUtNDZGRS1BODBDLTQ4RDY1OTJENEQ3RC8=
  37C: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Diagnostics.ServiceModelSink\v4.0_4.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Diagnostics.ServiceModelSink.dll
  430: Section       \BaseNamedObjects\NetPipeActivator/endpoint
------------------------------------------------------------------------------
svchost.exe pid: 3896 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
  128: Section       \BaseNamedObjects\__ComCatalogCache__
  12C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  158: File  (R-D)   C:\Windows\System32\en-GB\svchost.exe.mui
------------------------------------------------------------------------------
taskhostex.exe pid: 4512 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
   7C: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
   C8: File  (R-D)   C:\Windows\System32\en-US\taskhostex.exe.mui
   D0: File  (R-D)   C:\Windows\System32\en-US\MsCtfMonitor.dll.mui
   D4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  100: Section       \BaseNamedObjects\__ComCatalogCache__
  118: Section       \BaseNamedObjects\__ComCatalogCache__
  11C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  184: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1A4: Section       \Windows\Theme577161652
  1A8: Section       \Sessions\1\Windows\Theme2445879050
  1BC: File  (R-D)   C:\Windows\System32\en-US\wdmaud.drv.mui
  240: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{99EA1984-A9DA-41E1-AECF-2D4FDC9B5F83}
  268: Section       \Sessions\1\BaseNamedObjects\CTF.AsmListCache.FMPDefault1
  270: File  (R-D)   C:\Windows\System32\en-US\winmm.dll.mui
  2FC: Section       \Sessions\1\BaseNamedObjects\SqmData_{A1FAB6B0-6508-4E6B-9203-37D566D9D7DE}_S-1-5-21-2054895221-449969794-1663927727-1000
  320: File  (---)   C:\Users\Sam\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat
  36C: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{925CA821-FAF5-4E07-A398-534AF8D47A3F}
  380: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
  434: File  (---)   C:\Users\Sam\AppData\Local\Microsoft\Windows\WebCache\V01.log
------------------------------------------------------------------------------
explorer.exe pid: 4632 BLACK-BEAUTY\Sam
   10: File  (RW-)   C:\Windows\System32
   14: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
   64: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  10C: Section       \BaseNamedObjects\__ComCatalogCache__
  110: Section       \Windows\Theme577161652
  114: Section       \Sessions\1\Windows\Theme2445879050
  158: Section       \Sessions\1\BaseNamedObjects\SessionImmersiveColorSet
  15C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  184: Section       \BaseNamedObjects\__ComCatalogCache__
  194: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  1EC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  22C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  234: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  264: Section       \BaseNamedObjects\windows_shell_global_counters
  270: File  (R-D)   C:\Windows\WinSxS\amd64_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_500eb830a3073ea7\comctl32.dll.mui
  290: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  298: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_500eb830a3073ea7
  2B0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  308: File  (R-D)   C:\Windows\System32\en-US\wpnprv.dll.mui
  334: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  350: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  368: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  36C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  370: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  3B4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  3B8: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Printer Shortcuts
  3D4: File  (R-D)   C:\Windows\System32\en-GB\srchadmin.dll.mui
  3F8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  408: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  44C: Section       \Sessions\1\BaseNamedObjects\C:*Users*Sam*AppData*Local*Microsoft*Windows*Caches*cversions.1.ro
  54C: File  (RWD)   C:\Windows\Fonts\segoeuisl.ttf
  55C: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Start Menu
  564: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Libraries
  580: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  5BC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  654: File  (R-D)   C:\Windows\System32\en-GB\duser.dll.mui
  658: File  (RWD)   C:\Windows\Fonts\segoeui.ttf
  66C: File  (RWD)   C:\Windows\Fonts\seguisym.ttf
  674: File  (R-D)   C:\Windows\System32\en-US\Windows.Globalization.dll.mui
  688: File  (RWD)   C:\Windows\Fonts\segoeuil.ttf
  68C: Section       \Sessions\1\BaseNamedObjects\HWNDInterface:1013a
  730: File  (RW-)   C:\Windows\System32
  738: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  780: File  (---)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Notifications\WPNPRMRY.tmp
  7A8: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Notifications\appdb.dat
  804: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\TileCacheTickle-1203968_100.dat
  830: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\TileCacheLogo-1204140_100.dat
  84C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_48.db
  858: File  (RWD)   D:\Downloads\Media
  890: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Network Shortcuts
  8E4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  92C: File  (RWD)   C:\Users\Public\Desktop
  95C: File  (R-D)   C:\Windows\System32\en-GB\batmeter.dll.mui
  964: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  9C8: Section       \BaseNamedObjects\SearchCrawlScopeVersion
  9CC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  A08: File  (R-D)   C:\Windows\System32\en-GB\tquery.dll.mui
  ABC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  C10: Section       \Sessions\1\BaseNamedObjects\C:*Users*Sam*AppData*Local*Microsoft*Windows*Caches*{3DA71D5A-20CC-432F-A115-DFE92379E91F}.1.ver0x0000000000000049.db
  CD8: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_256.db
  CF0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  D20: File  (R-D)   C:\Windows\System32\en-US\AltTab.dll.mui
  D44: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{7A2BA096-1DFB-4CB0-AC3E-3707D73D274C}.2.ver0x0000000000000003.db
  DC0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  DCC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  E14: File  (RWD)   C:\Users\Sam\Favorites
  FEC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1008: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\GoogleInc.GoogleSearch_yfg5n0ztvskxp\RoamingState
 100C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1020: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.BingNews_8wekyb3d8bbwe\RoamingState
 1024: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.BingSports_8wekyb3d8bbwe\RoamingState
 1028: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.BingFinance_8wekyb3d8bbwe\RoamingState
 1034: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.BingMaps_8wekyb3d8bbwe\RoamingState
 1054: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.BingTravel_8wekyb3d8bbwe\RoamingState
 105C: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.BingWeather_8wekyb3d8bbwe\RoamingState
 1064: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.Bing_8wekyb3d8bbwe\RoamingState
 106C: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.Camera_8wekyb3d8bbwe\RoamingState
 107C: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.Media.PlayReadyClient_8wekyb3d8bbwe\RoamingState
 1084: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\windows.immersivecontrolpanel_cw5n1h2txyewy\RoamingState
 1088: File  (R-D)   C:\Windows\System32\en-GB\VAN.dll.mui
 1094: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\microsoft.microsoftskydrive_8wekyb3d8bbwe\RoamingState
 109C: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.VCLibs.110.00_8wekyb3d8bbwe\RoamingState
 10A0: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.Reader_8wekyb3d8bbwe\RoamingState
 10AC: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.SkypeApp_kzf8qxf38zg5c\RoamingState
 10B8: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\microsoft.windowscommunicationsapps_8wekyb3d8bbwe\RoamingState
 10C0: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\microsoft.windowsphotos_8wekyb3d8bbwe\RoamingState
 10C8: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.WinJS.1.0_8wekyb3d8bbwe\RoamingState
 10D0: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\WinStore_cw5n1h2txyewy\RoamingState
 10D4: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.XboxLIVEGames_8wekyb3d8bbwe\RoamingState
 10E0: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.ZuneMusic_8wekyb3d8bbwe\RoamingState
 10EC: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\Microsoft.ZuneVideo_8wekyb3d8bbwe\RoamingState
 10F4: File  (RWD)   C:\Users\Sam\AppData\Local\Packages\mobilewares.AUWeatherPro_n1t8x4c14tycy\RoamingState
 1120: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 1178: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 11C4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 11C8: File  (RWD)   C:\ProgramData\Microsoft\Windows\WER\ReportArchive
 11EC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1210: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 12A0: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\TileCacheStartView-1203625_100.dat
 12AC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 12DC: File  (RWD)   C:\Users\Sam\Links
 1308: File  (R-D)   C:\Windows\System32\en-GB\hgcpl.dll.mui
 1318: Section       \Sessions\1\BaseNamedObjects\HWNDInterface:1013a
 133C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 136C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1374: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Spelling
 13C0: File  (RWD)   C:\Users\Sam\Links
 13D8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 13EC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
 13F8: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
 140C: File  (RWD)   C:\inetpub\wwwroot\atom\bin
 1418: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\IME\15.0\IMESC
 1438: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_48.db
 144C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 145C: File  (RWD)   C:\inetpub\wwwroot\atom\bin
 14CC: File  (R-D)   C:\Windows\System32\en-US\winmm.dll.mui
 14E8: File  (RWD)   D:\Downloads
 1558: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
 1568: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{924F06F1-BDEF-4238-B81A-778441D20340}.2.ver0x0000000000000002.db
 1578: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\WER\ReportArchive
 157C: File  (RWD)   C:\Users\Sam\Favorites
 159C: File  (RWD)   C:\Windows\Fonts\segoeuib.ttf
 1644: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\RoamingTiles
 169C: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
 16A4: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
 1744: File  (R-D)   C:\Windows\System32\en-US\wdmaud.drv.mui
 1748: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Start Menu
 1768: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Spelling
 181C: File  (R-D)   C:\Windows\System32\en-GB\ieframe.dll.mui
 1844: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Burn
 1854: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\WER\ERC
 1858: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
 186C: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Printer Shortcuts
 1870: File  (RWD)   C:\inetpub\wwwroot\atom\bin\Debug
 18D4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1908: File  (R-D)   C:\Windows\System32\en-US\chartv.dll.mui
 1910: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned
 1974: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{99EA1984-A9DA-41E1-AECF-2D4FDC9B5F83}
 1A10: File  (RWD)   C:\inetpub\wwwroot\atom\bin\Debug
 1A24: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
 1A28: File  (RWD)   C:\Users\Public\Desktop
 1A34: File  (R-D)   C:\Windows\System32\en-GB\Windows.UI.Immersive.dll.mui
 1A64: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Vault\UserProfileRoaming
 1A74: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\IME\15.0\IMESC
 1AC8: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Libraries
 1AD4: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Windows\Network Shortcuts
 1AF8: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\Internet Explorer\Quick Launch\User Pinned
 1AFC: File  (RWD)   C:\Users\Sam\Desktop
 1B08: File  (RWD)   C:\Users\Sam\Desktop
 1B10: File  (RWD)   C:\ProgramData\Microsoft\Windows\Start Menu
 1B18: File  (RWD)   C:\ProgramData\Microsoft\Windows\Start Menu
 1BBC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_32.db
 1BD0: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Application Shortcuts
 1C14: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_96.db
 1C1C: File  (RWD)   C:\Windows\Fonts\seguisb.ttf
 1C24: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
 1C78: File  (R-D)   C:\Windows\System32\en-GB\explorerframe.dll.mui
 1C7C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
 1CCC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1CDC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Burn
 1D18: File  (R-D)   C:\Windows\System32\en-GB\wlanmm.dll.mui
 1D44: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
 1D54: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
 1D74: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
 1D7C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Application Shortcuts
 1D8C: File  (R--)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\TileCacheDefault-1203828_100.dat
 1DE4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_idx.db
 1E00: File  (R-D)   C:\Windows\System32\en-GB\SyncCenter.dll.mui
 1E44: File  (RWD)   D:\Downloads\Media
 1F5C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Vault\UserProfileRoaming
 1F6C: File  (RWD)   D:\Downloads
 1F88: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
 1FAC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\RoamingTiles
 1FB0: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
 1FB4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
------------------------------------------------------------------------------
nvtray.exe pid: 5072 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
    C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
   38: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  11C: Section       \Windows\Theme577161652
  120: Section       \Sessions\1\Windows\Theme2445879050
  124: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  224: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  2F4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
------------------------------------------------------------------------------
SearchIndexer.exe pid: 308 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows\System32
   44: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   D0: Section       \BaseNamedObjects\windows_shell_global_counters
  1C4: Section       \BaseNamedObjects\UGATHERER
  1C8: Section       \BaseNamedObjects\UGathererObj
  1F8: Section       \BaseNamedObjects\UGTHRSVC
  1FC: Section       \BaseNamedObjects\UGthrSvcObj
  200: Section       \BaseNamedObjects\__ComCatalogCache__
  218: Section       \BaseNamedObjects\__ComCatalogCache__
  21C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  324: Section       \BaseNamedObjects\GDA:  ESENT Performance Data Schema Version 215
  328: File  (R-D)   C:\Windows\System32\en-US\ESENT.dll.mui
  330: Section       \BaseNamedObjects\IDA0:  ESENT Performance Data Schema Version 215
  354: File  (---)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\Windows.edb
  40C: File  (RW-)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\Projects\SystemIndex\PropMap\CiPT0000.000
  414: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  418: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  41C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  420: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  424: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  428: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  42C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{7A2BA096-1DFB-4CB0-AC3E-3707D73D274C}.2.ver0x0000000000000003.db
  434: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{924F06F1-BDEF-4238-B81A-778441D20340}.2.ver0x0000000000000002.db
  7AC: File  (R--)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\GatherLogs\SystemIndex\SystemIndex.8.gthr
  7B0: File  (R--)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\GatherLogs\SystemIndex\SystemIndex.8.Crwl
  7C8: File  (RW-)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\Projects\SystemIndex\SecStore\CiST0000.000
  7E0: Section       \BaseNamedObjects\WSearchIdxPi
  7E4: Section       \BaseNamedObjects\WseIdxPm
  83C: File  (R-D)   C:\Windows\System32\en-US\vsstrace.dll.mui
  8DC: File  (RW-)   C:
  8F0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  8F8: File  (RW-)   D:
  908: Section       \BaseNamedObjects\UsGthrCtrlFltPipeMssGthrPipe20
  97C: File  (R-D)   C:\Windows\System32\en-US\elscore.dll.mui
  B5C: Section       \BaseNamedObjects\UsGthrFltPipeMssGthrPipe20_1
  BA8: File  (---)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\edb.log
  BC0: File  (---)   C:\ProgramData\Microsoft\Search\Data\Applications\Windows\edbtmp.log
  BEC: Section       \BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
SynTPEnh.exe pid: 4644 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
   10: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
   3C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  104: Section       \Windows\Theme577161652
  108: Section       \Sessions\1\Windows\Theme2445879050
  10C: Section       \BaseNamedObjects\__ComCatalogCache__
  1B8: Section       \BaseNamedObjects\__ComCatalogCache__
  1BC: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  1CC: Section       \Sessions\1\BaseNamedObjects\SynAPIArena
  1D4: Section       \BaseNamedObjects\RotHintTable
  210: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  25C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  260: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  264: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  268: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  278: Section       \BaseNamedObjects\windows_shell_global_counters
  30C: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  330: Section       \Sessions\1\BaseNamedObjects\SynTPAPIMemMap
------------------------------------------------------------------------------
SynTPHelper.exe pid: 4880 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
   30: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   48: Section       \Windows\Theme577161652
   4C: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
RAVCpl64.exe pid: 4600 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
   10: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   40: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  12C: Section       \Windows\Theme577161652
  130: Section       \Sessions\1\Windows\Theme2445879050
  2AC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  36C: Section       \BaseNamedObjects\__ComCatalogCache__
  380: Section       \BaseNamedObjects\__ComCatalogCache__
  384: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  3FC: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
LCore.exe pid: 1192 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
    C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   10: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   1C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   20: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   24: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   28: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   2C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   30: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
   54: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  164: Section       \Windows\Theme577161652
  168: Section       \Sessions\1\Windows\Theme2445879050
  2C4: File  (RW-)   C:\Users\Sam\AppData\Roaming\NVIDIA\GLCache\26b968f82474d81ec3ffbec9f6a8934e\6542aa3c4149fe03\59f7eabc1bdabc21.toc
  2C8: File  (RW-)   C:\Users\Sam\AppData\Roaming\NVIDIA\GLCache\26b968f82474d81ec3ffbec9f6a8934e\6542aa3c4149fe03\59f7eabc1bdabc21.bin
  2E8: Section       \Sessions\1\BaseNamedObjects\qipc_sharedmemory_LCoreVrunning318f36b64e61b045c30421f3606f0b792039b17f
  388: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  398: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  39C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  3C4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  3C8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  3FC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  408: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  414: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  468: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  47C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  480: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  484: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  488: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  490: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  49C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  4AC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  4D0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  4FC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  510: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  520: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  538: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  548: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  558: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  56C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  580: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  58C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  594: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  59C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6CC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6D8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6DC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6E4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6E8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6EC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  6FC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  700: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  704: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  710: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  718: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  728: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  730: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  734: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  73C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  740: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  744: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  754: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  75C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  760: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  770: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  784: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  788: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  78C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  804: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  80C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  810: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
------------------------------------------------------------------------------
rundll32.exe pid: 5112 BLACK-BEAUTY\Sam
   10: File  (RW-)   C:\Windows\System32
   38: File  (R-D)   C:\Windows\System32\en-US\rundll32.exe.mui
   44: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   C0: Section       \Windows\Theme577161652
   C4: Section       \Sessions\1\Windows\Theme2445879050
   E0: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
   F8: Section       \BaseNamedObjects\__ComCatalogCache__
  118: Section       \BaseNamedObjects\__ComCatalogCache__
  11C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  210: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
------------------------------------------------------------------------------
SetPoint.exe pid: 4476 BLACK-BEAUTY\Sam
   10: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   1C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   20: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
   24: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   28: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   2C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
   30: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
   38: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   3C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
   40: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
   44: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   48: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
   4C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   74: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   BC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfcloc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_01c9581e60cbee58
  130: Section       \Sessions\1\BaseNamedObjects\CLogiInstanceCheck_MMF_SetPoint
  148: File  (RW-)   C:\Program Files\Logitech\SetPointP
  14C: Section       \Windows\Theme577161652
  150: Section       \Sessions\1\Windows\Theme2445879050
  188: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  18C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  190: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.atl_1fc8b3b9a1e18e3b_9.0.30729.6161_none_0a1fd3a3a768b895
  194: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  198: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  19C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  1A0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  1A4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.atl_1fc8b3b9a1e18e3b_9.0.30729.6161_none_0a1fd3a3a768b895
  1A8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  1AC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  1B0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.atl_1fc8b3b9a1e18e3b_9.0.30729.6161_none_0a1fd3a3a768b895
  1B4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  1B8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  1BC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.atl_1fc8b3b9a1e18e3b_9.0.30729.6161_none_0a1fd3a3a768b895
  1C0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  1C8: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  200: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  23C: Section       \Sessions\1\BaseNamedObjects\LD_KHAL_SharedGblMem
  2D4: Section       \BaseNamedObjects\__ComCatalogCache__
  2D8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  2F4: Section       \BaseNamedObjects\__ComCatalogCache__
  2F8: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  304: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  30C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  310: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  314: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  320: Section       \BaseNamedObjects\windows_shell_global_counters
  398: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  40C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  434: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.mfc_1fc8b3b9a1e18e3b_9.0.30729.6161_none_044aad0bab1eb146
  438: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  45C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
------------------------------------------------------------------------------
KHALMNPR.exe pid: 3980 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Program Files\Logitech\SetPointP
   30: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   3C: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
   F4: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  10C: Section       \Sessions\1\BaseNamedObjects\LD_KHAL_SharedGblMem
  238: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  24C: File  (RWD)   C:\ProgramData
  258: Section       \Windows\Theme577161652
  25C: Section       \Sessions\1\Windows\Theme2445879050
  26C: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  278: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  298: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  29C: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  2A4: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
  2AC: Section       \Sessions\1\BaseNamedObjects\LogiBugShow
------------------------------------------------------------------------------
TortoiseHgOverlayServer.exe pid: 4840 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
    C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   10: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   3C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   D8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
  138: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  15C: Section       \Windows\Theme577161652
  160: Section       \Sessions\1\Windows\Theme2445879050
  164: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  16C: File  (RWD)   C:\Users\Sam\AppData\Roaming\TortoiseHg\OverlayServerLog.txt
------------------------------------------------------------------------------
chrome.exe pid: 5180 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   48: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  104: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  130: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1B0: Section       \Windows\Theme577161652
  1B4: Section       \Sessions\1\Windows\Theme2445879050
  224: File  (RWD)   C:\Windows\System32\Drivers\etc
  25C: File  (R--)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\lockfile
  274: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  27C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  284: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  28C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\resources.pak
  39C: Section       \BaseNamedObjects\__ComCatalogCache__
  3BC: Section       \BaseNamedObjects\__ComCatalogCache__
  3C4: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  3CC: Section       \BaseNamedObjects\windows_shell_global_counters
  3E8: File  (---)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Extension State\LOCK
  40C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Web Data
  42C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Extension State\000132.log
  438: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Extension State\MANIFEST-000130
  450: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Index
  468: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Extension State\000133.sst
  4DC: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\User StyleSheets
  4F8: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  93C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Visited Links
  EA4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Extension Cookies-journal
 1254: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Network Action Predictor-journal
 12E8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Top Sites-journal
 13C4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Favicons
 14B4: File  (RW-)   C:\Users\Sam\AppData\Local\Temp\etilqs_3UNqc0OhLBRRH3R
 16A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cookies
 1740: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\https_docs.google.com_0.indexeddb.leveldb\000586.log
 1790: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
 17A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_algjnflpgoopkdijmkalfcifomdhmcbe_0.localstorage
 1C08: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Index-journal
 1C10: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_1
 1C14: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_0
 1C18: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_kkjmcfdcdbbkdacicmpokoddagejpknh_0.localstorage
 1C20: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\https_docs.google.com_0.indexeddb.leveldb\000582.sst
 1C30: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\QuotaManager-journal
 1C38: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\https_docs.google.com_0.indexeddb.leveldb\000585.sst
 1C3C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_maps.google.com_0.localstorage-journal
 1C54: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_3
 1C60: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_1
 1C64: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_1
 1C70: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\index
 1C74: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\index
 1C7C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_bhlhnicpbhignbdhedgjhgdocnmhomnp_0.localstorage
 1C80: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_2
 1C88: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_0
 1C8C: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_0
 1C94: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_hcjdanpjacpeeppdjkppebobilhaglfo_0.localstorage
 1C98: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_okanipcmceoeemlbjnmnbdibhgpbllgc_0.localstorage
 1C9C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_ddkmiidlgnkhnfhigdpadkaamogngkin_0.localstorage
 1CA0: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_2
 1CA8: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cache\data_3
 1CAC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_hipbfijinpcgfogaopmgehiegacbhmob_0.localstorage
 1CB0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 1CB8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_bkiabklhofojmagogdjdmhmbhngajopa_0.localstorage-journal
 1CBC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_bkiabklhofojmagogdjdmhmbhngajopa_0.localstorage
 1CC0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_cfhdojbkjhnklbpkdaibdccddilifddb_0.localstorage
 1CD4: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_0
 1CD8: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\index
 1CDC: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\index
 1CE8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_nmkbaaijgpppbokgnhhoakihofedkgcc_0.localstorage
 1CF4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\QuotaManager
 1D08: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_1
 1D10: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_2
 1D14: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_2
 1D1C: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_3
 1D20: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Application Cache\Cache\data_3
 1D28: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
 1D2C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_docs.google.com_0.localstorage
 1D34: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_oadboiipflhobonjjffjbfekfjcgkhco_0.localstorage
 1D38: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\History
 1D58: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_mmgagnmbebdebebbcleklifnobamjonh_0.localstorage
 1D70: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_algjnflpgoopkdijmkalfcifomdhmcbe_0.localstorage-journal
 1D74: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Extension Cookies
 1D80: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 1D8C: File  (R-D)   C:\Windows\SysWOW64\en-GB\explorerframe.dll.mui
 1D9C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Archived History
 1DA0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\databases\Databases.db
 1DA8: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 1DAC: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
 1DB0: File  (---)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\https_docs.google.com_0.indexeddb.leveldb\LOCK
 1DC4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_mmgagnmbebdebebbcleklifnobamjonh_0.localstorage-journal
 1DD4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\https_docs.google.com_0.indexeddb.leveldb\MANIFEST-000584
 1DE0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Network Action Predictor
 1E20: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_hipbfijinpcgfogaopmgehiegacbhmob_0.localstorage-journal
 1E2C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Sync Data\SyncData.sqlite3
 1ED0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\https_docs.google.com_0.indexeddb.leveldb\000583.sst
 1F38: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 1F70: File  (---)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Current Session
 1FCC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Login Data
 1FDC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Sync Data\SyncData.sqlite3-journal
 201C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Web Data-journal
 2028: File  (RW-)   C:\Users\Sam\AppData\Local\Temp\etilqs_lS4AXa0Rsuur03s
 2034: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 204C: File  (RW-)   C:\Users\Sam\AppData\Local\Temp\etilqs_oOVKeU5kE8335oC
 20E0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_hdokiejnpimakedhajhdlcegeplioahd_0.localstorage
 20F0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.feedly.com_0.localstorage
 2154: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\History Index 2012-10
 21A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\History-journal
 21F8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Safe Browsing Cookies-journal
 2240: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\chrome-extension_hdokiejnpimakedhajhdlcegeplioahd_0.localstorage-journal
 22B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Safe Browsing Cookies
 230C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Cookies-journal
 2310: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Shortcuts
 2364: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 2374: File  (---)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Current Tabs
 2404: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Top Sites
 2430: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 2568: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_stackoverflow.com_0.localstorage
 2774: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_mail.google.com_0.localstorage
 2784: File  (R-D)   C:\Windows\SysWOW64\en-US\wdmaud.drv.mui
 27D0: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
 2830: File  (RW-)   C:\Users\Sam\AppData\Local\Temp\etilqs_B0hD7irPdjhjqLm
 2848: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\History Index 2012-11-journal
 28A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\History Index 2012-10-journal
 28BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 28EC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Shortcuts-journal
 2900: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{924F06F1-BDEF-4238-B81A-778441D20340}.2.ver0x0000000000000002.db
 2A74: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
 2B10: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 2B14: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_github.com_0.localstorage-journal
 2B2C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
 2B6C: File  (RW-)   C:\Users\Sam\AppData\Local\Temp\etilqs_kIVfrIKcL5nKxkU
 2BA4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Favicons-journal
 2C48: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.fizzy.com_0.localstorage-journal
 2D5C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 2D9C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 2DE4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.google.com.au_0.localstorage
 2E10: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_plus.google.com_0.localstorage-journal
 2E74: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_plus.google.com_0.localstorage
 2E84: File  (R-D)   C:\Windows\SysWOW64\en-US\MMDevAPI.dll.mui
 2E98: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\History Index 2012-11
 2ED8: File  (---)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\002\p\Paths\LOCK
 2EE0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_disqus.com_0.localstorage
 2F08: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 2F90: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 2FD4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
 2FF4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\Origins\000936.log
 3064: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
 3074: File  (R-D)   C:\Windows\SysWOW64\en-GB\comdlg32.dll.mui
 30D8: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 3134: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{7A2BA096-1DFB-4CB0-AC3E-3707D73D274C}.2.ver0x0000000000000003.db
 31B8: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
 31C8: Section       \Sessions\1\BaseNamedObjects\SessionImmersiveColorSet
 3320: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
 332C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 350C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_github.com_0.localstorage
 3554: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.facebook.com_0.localstorage
 35D8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_accounts.google.com_0.localstorage
 35FC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_mediacdn.disqus.com_0.localstorage
 36BC: File  (---)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\Origins\LOCK
 36EC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\Origins\000768.sst
 375C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.feedly.com_0.localstorage-journal
 37AC: File  (R-D)   C:\Windows\WinSxS\x86_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_97bbef07b78367ad\comctl32.dll.mui
 37DC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_maps.google.com_0.localstorage
 3830: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_97bbef07b78367ad
 3934: File  (RW-)   C:\Users\Sam\AppData\Local\Temp\etilqs_FDBlyYIp0OlsQ4F
 3950: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_disqus.com_0.localstorage-journal
 39E8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
 3A30: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\Origins\MANIFEST-000935
 3A38: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_ausdroid.net_0.localstorage
 3A64: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\https_careers.bristol.ac.uk_0.localstorage
 3A74: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\002\p\Paths\MANIFEST-000155
 3B04: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.lifehacker.com.au_0.localstorage-journal
 3B24: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.lifehacker.com.au_0.localstorage
 3D18: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\002\p\Paths\000005.sst
 3FD8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\File System\002\p\Paths\000156.log
 4058: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.fupa.com_0.localstorage-journal
 4078: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.fupa.com_0.localstorage
 4350: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\Local Storage\http_www.fizzy.com_0.localstorage
------------------------------------------------------------------------------
chrome.exe pid: 5296 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  134: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  174: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  224: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5348 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  144: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  14C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  16C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1D0: Section       \Windows\Theme577161652
  1D4: Section       \Sessions\1\Windows\Theme2445879050
  1DC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
------------------------------------------------------------------------------
chrome.exe pid: 5436 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   90: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  224: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  230: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5448 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  140: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  16C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  218: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5472 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   68: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  144: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  1E0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  220: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5512 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  20C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  280: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 5556 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  228: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 5592 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1DC: Section       \Windows\Theme577161652
  1E0: Section       \Sessions\1\Windows\Theme2445879050
  22C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  260: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  278: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\databases\chrome-extension_edacconmaakjimmfgnblocblbcdcpbko_0\1
------------------------------------------------------------------------------
chrome.exe pid: 5632 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   64: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  184: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1DC: Section       \Windows\Theme577161652
  1E0: Section       \Sessions\1\Windows\Theme2445879050
  218: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5740 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   7C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  198: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  224: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  2A4: File  (RWD)   C:\Users\Sam\AppData\Local\Google\Chrome\User Data\Default\databases\chrome-extension_hdokiejnpimakedhajhdlcegeplioahd_0\2
------------------------------------------------------------------------------
chrome.exe pid: 5768 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1DC: Section       \Windows\Theme577161652
  1E0: Section       \Sessions\1\Windows\Theme2445879050
  218: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  288: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 5796 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  21C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  3E0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 5836 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  210: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  238: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 5876 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   88: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  228: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  248: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 5924 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  170: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  218: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5960 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   90: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  180: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D0: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  22C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5996 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  214: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  240: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 6016 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  220: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  22C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 6068 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   88: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  138: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  238: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  260: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 6132 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   44: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   F4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
   F8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
  128: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  158: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  160: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  168: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  180: Section       \Windows\Theme577161652
  184: Section       \Sessions\1\Windows\Theme2445879050
  1CC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5128 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  144: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 5188 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   48: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   FC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  12C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  15C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  164: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  16C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  184: Section       \Windows\Theme577161652
  188: Section       \Sessions\1\Windows\Theme2445879050
  1D8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  1DC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  278: Section       \Sessions\1\BaseNamedObjects\LASTPASS_ASO_SHARED_DATA
------------------------------------------------------------------------------
chrome.exe pid: 6004 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   78: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
   A0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  144: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  150: Section       \Windows\Theme577161652
  190: Section       \Sessions\1\Windows\Theme2445879050
  1B8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1C0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1C8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1D8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  224: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
googledrivesync.exe pid: 5600 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   18: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
   1C: File  (RW-)   C:\Program Files (x86)\Google\Drive\googledrivesync.exe
   54: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
------------------------------------------------------------------------------
googledrivesync.exe pid: 6064 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   18: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
   1C: File  (RW-)   C:\Program Files (x86)\Google\Drive\googledrivesync.exe
   48: File  (RW-)   C:\Program Files (x86)\Google\Drive\googledrivesync.exe
   50: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   54: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   68: File  (RW-)   C:\Program Files (x86)\Google\Drive\googledrivesync.exe
   EC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   F0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  108: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  120: Section       \Windows\Theme577161652
  124: Section       \Sessions\1\Windows\Theme2445879050
  140: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  210: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  214: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  24C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  250: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  254: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  258: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  25C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  260: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  264: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  26C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  270: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  290: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  298: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  29C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  2A0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  2AC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  368: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  378: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  380: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  388: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  3A4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  3BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  3F8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Drive\lockfile
  734: File  (RW-)   C:\Users\Sam\Google Drive
------------------------------------------------------------------------------
Skype.exe pid: 5408 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   9C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  194: Section       \Windows\Theme577161652
  198: Section       \Sessions\1\Windows\Theme2445879050
  1D0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
  21C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  248: File  (R-D)   C:\Program Files (x86)\Skype\Phone\Skype.exe
  278: Section       \BaseNamedObjects\__ComCatalogCache__
  290: Section       \BaseNamedObjects\__ComCatalogCache__
  294: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  2A8: Section       \BaseNamedObjects\windows_shell_global_counters
  2AC: File  (RW-)   C:\Program Files (x86)\Skype\Phone
  384: File  (---)   C:\Users\Sam\AppData\Roaming\Skype\shared_httpfe\queue.lock
  3A0: File  (RWD)   C:\Users\Sam\AppData\Roaming\Skype
  440: File  (---)   C:\Users\Sam\AppData\Roaming\Skype\shared_dynco\dc.lock
  444: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\shared_dynco\dc.db
  448: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\shared_dynco\dc.db
  464: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\shared_httpfe\queue.db
  46C: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\shared_httpfe\queue.db
  494: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  4A0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  53C: Section       \BaseNamedObjects\RotHintTable
  5CC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  5D0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_97bbef07b78367ad
  5D4: File  (R-D)   C:\Windows\WinSxS\x86_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_97bbef07b78367ad\comctl32.dll.mui
  5D8: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  650: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\main.db
  654: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\DbTemp\temp-5r15HpXa3FNfOm0UrW5Yh7ux
  65C: File  (RWD)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj
  660: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  664: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  668: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  66C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  6A0: File  (---)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\main.lock
  6A8: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\DbTemp\temp-5r15HpXa3FNfOm0UrW5Yh7ux
  6AC: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\main.db
  6C4: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\DbTemp\temp-rvz7H208RhriZxz5O0NqPnFR
  6C8: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\DbTemp\temp-rvz7H208RhriZxz5O0NqPnFR
  6D0: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
  75C: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\keyval.db
  760: File  (---)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\keyval.lock
  768: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\keyval.db
  778: File  (---)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\msn.lock
  784: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\msn.db
  78C: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\msn.db
  7B8: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  7C8: File  (R-D)   C:\Windows\SysWOW64\en-US\MMDevAPI.dll.mui
  864: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  870: File  (R-D)   C:\Windows\SysWOW64\en-US\mswsock.dll.mui
  880: File  (R-D)   C:\Windows\SysWOW64\ieframe.dll
  894: File  (R-D)   C:\Windows\SysWOW64\en-GB\ieframe.dll.mui
  8A4: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\dc.db
  8AC: File  (R-D)   C:\Windows\SysWOW64\en-US\jscript9.dll.mui
  8B4: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\LAR5LC0K\AdLoader[1].htm
  8C8: File  (R-D)   C:\Windows\SysWOW64\en-GB\urlmon.dll.mui
  920: Section       \Sessions\1\BaseNamedObjects\MSIMGSIZECacheMap
  9AC: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{99EA1984-A9DA-41E1-AECF-2D4FDC9B5F83}
  A00: File  (R-D)   C:\Windows\SysWOW64\stdole2.tlb
  A0C: File  (RWD)   C:\Windows\Fonts\arialbd.ttf
  A14: File  (RWD)   C:\Windows\Fonts\arial.ttf
  A30: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\YC4QOL4I\xml[2].xml
  A38: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\dc.db
  A74: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{925CA821-FAF5-4E07-A398-534AF8D47A3F}
  AB8: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\bistats.db
  B48: File  (R-D)   C:\Windows\SysWOW64\en-GB\MFC42.dll.mui
  C40: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\SystemCertificates\My
  D38: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\GPJANQA3\home[1].htm
  D5C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  F2C: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\YC4QOL4I\xd_arbiter[2].htm
  F60: File  (R-D)   C:\Windows\SysWOW64\iepeers.dll
  F68: File  (R-D)   C:\Windows\SysWOW64\mshtml.tlb
  F78: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\Content.IE5\LAR5LC0K\ads-in-client[1].js
 10D0: File  (---)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\bistats.lock
 1330: File  (RW-)   C:\Users\Sam\AppData\Roaming\Skype\samuel.abj\bistats.db
 1354: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
------------------------------------------------------------------------------
SpotifyWebHelper.exe pid: 6524 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   4C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   D0: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  138: Section       \Windows\Theme577161652
  13C: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
Dropbox.exe pid: 6636 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   18: File  (RW-)   C:\Windows\SysWOW64
   4C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  1FC: Section       \Windows\Theme577161652
  200: Section       \Sessions\1\Windows\Theme2445879050
  238: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  284: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  2AC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
  374: File  (RW-)   C:\Users\Sam\AppData\Roaming\Dropbox\config.dbx
  38C: Section       \BaseNamedObjects\__ComCatalogCache__
  39C: Section       \BaseNamedObjects\__ComCatalogCache__
  3A0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  3D8: File  (R-D)   C:\Windows\SysWOW64\FirewallAPI.dll
  3E0: File  (R-D)   C:\Windows\SysWOW64\stdole2.tlb
  428: File  (RW-)   C:\Users\Sam\AppData\Roaming\Dropbox\filecache.dbx
  42C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  490: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  5CC: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  7D0: File  (RW-)   C:\Users\Sam\AppData\Roaming\Dropbox\sigstore.dbx
  890: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  8B8: File  (RW-)   C:\Users\Sam\AppData\Roaming\Dropbox\photo.dbx
  8C0: Section       \BaseNamedObjects\windows_shell_global_counters
  8E4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  958: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  98C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{924F06F1-BDEF-4238-B81A-778441D20340}.2.ver0x0000000000000002.db
  990: File  (R-D)   C:\Windows\SysWOW64\en-GB\NetworkExplorer.dll.mui
  9C0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  9C4: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  9D4: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{7A2BA096-1DFB-4CB0-AC3E-3707D73D274C}.2.ver0x0000000000000003.db
  A40: File  (R-D)   C:\Windows\SysWOW64\en-US\imageres.dll.mui
  B04: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  B6C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  B74: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  B7C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  BDC: File  (R-D)   C:\Windows\SysWOW64\en-GB\explorerframe.dll.mui
  C60: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  CC4: Section       \Sessions\1\BaseNamedObjects\SessionImmersiveColorSet
  D30: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  D34: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  D4C: File  (R-D)   C:\Windows\SysWOW64\en-US\chartv.dll.mui
  D50: File  (R-D)   C:\Windows\SysWOW64\en-GB\ntshrui.dll.mui
  E40: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  E64: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  E6C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  E70: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
  EA4: File  (RW-)   D:\Dropbox
  F20: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  F28: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
------------------------------------------------------------------------------
Everything-1.2.1.451a.exe pid: 6668 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   18: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
   4C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   F8: Section       \Windows\Theme577161652
   FC: Section       \Sessions\1\Windows\Theme2445879050
  100: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  144: File  (RWD)   C:
  158: File  (RWD)   C:
  17C: File  (RWD)   C:
  184: File  (RWD)   D:
  194: File  (RWD)   C:
  19C: File  (RWD)   D:
  1BC: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  1C4: Section       \BaseNamedObjects\__ComCatalogCache__
  1DC: Section       \BaseNamedObjects\__ComCatalogCache__
  1E0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  1F4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  254: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  264: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  270: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  278: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
  298: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
  2AC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  2F0: Section       \BaseNamedObjects\windows_shell_global_counters
  2FC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
------------------------------------------------------------------------------
IAStorIcon.exe pid: 7088 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   44: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   7C: File  (R-D)   C:\Users\Sam\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\security.config.cch
   88: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
   A8: Section       \BaseNamedObjects\Cor_Private_IPCBlock_7088
   AC: Section       \BaseNamedObjects\Cor_Public_IPCBlock_7088
  100: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\security.config.cch
  104: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\enterprisesec.config.cch
  15C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1A0: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_32\index1e1.dat
  1A4: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  1BC: Section       \Windows\Theme577161652
  1C0: Section       \Sessions\1\Windows\Theme2445879050
  1D0: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  1E8: File  (R--)   C:\Windows\assembly\pubpol71.dat
  1EC: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorCommon.dll
  1F8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  200: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorUtil.dll
  298: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
  2B8: Section       \BaseNamedObjects\NLS_CodePage_1252_3_2_0_0
  2C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
  2D0: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IntelVisualDesign.dll
  304: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  3B4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  564: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorUIHelper.dll
------------------------------------------------------------------------------
nusb3mon.exe pid: 6244 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   1C: File  (RW-)   C:\Windows\SysWOW64
   24: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
   58: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  114: Section       \Windows\Theme577161652
  118: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
ReaderAppHelper.exe pid: 6340 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   28: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   34: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
  140: Section       \Windows\Theme577161652
  144: Section       \Sessions\1\Windows\Theme2445879050
  1A4: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  3A0: Section       \BaseNamedObjects\__ComCatalogCache__
  3B8: Section       \BaseNamedObjects\__ComCatalogCache__
  3BC: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  3C8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
------------------------------------------------------------------------------
jusched.exe pid: 3400 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   34: Section       \Windows\Theme577161652
   54: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   58: Section       \Sessions\1\Windows\Theme2445879050
  140: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  150: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  164: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  278: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
  29C: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  324: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\SystemCertificates\My
  570: Section       \BaseNamedObjects\__ComCatalogCache__
  588: File  (R-D)   C:\Windows\SysWOW64\en-US\mswsock.dll.mui
  59C: Section       \BaseNamedObjects\__ComCatalogCache__
  5A0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  5A8: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  5B0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  5B4: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  5B8: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  5C4: Section       \BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
THXAudio.exe pid: 3372 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   3C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   78: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
   98: Section       \BaseNamedObjects\Cor_Public_IPCBlock_3372
   A4: Section       \BaseNamedObjects\Cor_Private_IPCBlock_3372
   F4: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\security.config.cch
   F8: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\enterprisesec.config.cch
  14C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  160: File  (R-D)   C:\Users\Sam\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\security.config.cch
  19C: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_32\index1e1.dat
  1BC: Section       \Windows\Theme577161652
  1C0: Section       \Sessions\1\Windows\Theme2445879050
  1D0: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  1D4: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  1E4: File  (R--)   C:\Windows\assembly\pubpol71.dat
  1F0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  298: Section       \BaseNamedObjects\netfxcustomperfcounters.1.0.net clr networking
  2DC: Section       \Sessions\1\BaseNamedObjects\308883d2-d39a-3214-affb-0612fbc2ce051.3Map
  2E0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  2F0: File  (R-D)   C:\Program Files (x86)\Creative\THX TruStudio Pro\THXAudioCP\en-US\THXAudio.resources.dll
  610: File  (R-D)   C:\Windows\SysWOW64\en-GB\msctfui.dll.mui
  620: Section       \BaseNamedObjects\__ComCatalogCache__
  66C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  6A0: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  6A8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  6E0: Section       \BaseNamedObjects\__ComCatalogCache__
  724: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  738: Section       \BaseNamedObjects\Font Cache Mapping 87a8974c-f1c0-4af7-951a-43f67798fc5c
  75C: File  (R-D)   C:\Windows\SysWOW64\en-US\MMDevAPI.dll.mui
------------------------------------------------------------------------------
WmiPrvSE.exe pid: 4684 NT AUTHORITY\NETWORK SERVICE
    8: File  (RW-)   C:\Windows\System32
   80: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   DC: Section       \BaseNamedObjects\Wmi Provider Sub System Counters
   F8: Section       \BaseNamedObjects\__ComCatalogCache__
  110: Section       \BaseNamedObjects\__ComCatalogCache__
  114: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  1A8: File  (R-D)   C:\Windows\System32\en-US\advapi32.dll.mui
  200: File  (R-D)   C:\Windows\System32\wbem\en-US\cimwin32.dll.mui
------------------------------------------------------------------------------
explorer.exe pid: 6296 BLACK-BEAUTY\Sam
   10: File  (RW-)   C:\Windows\System32
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
   6C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  114: Section       \Windows\Theme577161652
  118: Section       \Sessions\1\Windows\Theme2445879050
  128: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  160: Section       \BaseNamedObjects\__ComCatalogCache__
  178: Section       \BaseNamedObjects\__ComCatalogCache__
  17C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  1E8: Section       \Sessions\1\BaseNamedObjects\C:*Users*Sam*AppData*Local*Microsoft*Windows*Caches*cversions.1.ro
  214: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  240: Section       \BaseNamedObjects\windows_shell_global_counters
  27C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  28C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  2A0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  2A4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  2AC: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  2B0: File  (R-D)   C:\Windows\System32\en-GB\explorerframe.dll.mui
  2B8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  2F0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  304: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  338: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  340: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  344: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{7A2BA096-1DFB-4CB0-AC3E-3707D73D274C}.2.ver0x0000000000000003.db
  348: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  34C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{924F06F1-BDEF-4238-B81A-778441D20340}.2.ver0x0000000000000002.db
  3AC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
  40C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  414: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
  420: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  424: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
  47C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_32.db
  52C: Section       \Sessions\1\BaseNamedObjects\SessionImmersiveColorSet
  554: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
  5CC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
  60C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  6C0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  734: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_48.db
  820: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_1024.db
  8B0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  96C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  9A0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  9CC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_256.db
  A0C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  A20: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
  A40: File  (R-D)   C:\Windows\System32\en-US\ole32.dll.mui
  A90: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  ABC: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  B34: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
  BD4: Section       \BaseNamedObjects\SearchCrawlScopeVersion
  BE0: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_idx.db
  C04: Section       \BaseNamedObjects\RotHintTable
  D04: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
  D30: File  (R-D)   C:\Windows\System32\en-US\winmm.dll.mui
  D60: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
  D6C: File  (R-D)   C:\Windows\System32\en-US\wdmaud.drv.mui
  D84: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  E08: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  E0C: File  (R-D)   C:\Windows\System32\en-GB\duser.dll.mui
  E10: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_500eb830a3073ea7
  E14: File  (R-D)   C:\Windows\WinSxS\amd64_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_500eb830a3073ea7\comctl32.dll.mui
  E6C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_idx.db
  E90: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_48.db
 10A4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
 1378: File  (R-D)   C:\Windows\System32\en-GB\dlnashext.dll.mui
 144C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
 1480: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_256.db
 15B0: File  (R-D)   C:\Windows\System32\en-US\imageres.dll.mui
 15D4: Section       \Sessions\1\BaseNamedObjects\C:*Users*Sam*AppData*Local*Microsoft*Windows*Caches*{3DA71D5A-20CC-432F-A115-DFE92379E91F}.1.ver0x0000000000000049.db
 1608: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
 1690: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_96.db
 16A4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
 16D8: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_idx.db
------------------------------------------------------------------------------
IAStorDataMgrSvc.exe pid: 1832 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   54: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   74: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
   88: Section       \BaseNamedObjects\Cor_Private_IPCBlock_1832
   94: Section       \BaseNamedObjects\Cor_Public_IPCBlock_1832
   F0: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\security.config.cch
   F4: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\enterprisesec.config.cch
  148: Section       \BaseNamedObjects\windows_shell_global_counters
  158: File  (R-D)   C:\Windows\SysWOW64\config\systemprofile\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\security.config.cch
  1A0: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_32\index1e1.dat
  1B4: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  1BC: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  1D8: File  (R--)   C:\Windows\assembly\pubpol71.dat
  1E4: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorUtil.dll
  1F0: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorDataMgr.dll
  1FC: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorDataMgrSvc.exe
  298: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IAStorCommon.dll
  2A4: File  (R-D)   C:\Program Files (x86)\Intel\Intel(R) Rapid Storage Technology\IsdiInterop.dll
  364: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  3B0: File  (R-D)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5\msvcm90.dll
  3C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
------------------------------------------------------------------------------
LMS.exe pid: 3120 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  14C: File  (---)   C:\Windows\SysWOW64\log.txt
------------------------------------------------------------------------------
daemonu.exe pid: 2760 BLACK-BEAUTY\UpdatusUser
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   58: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  1B8: Section       \BaseNamedObjects\windows_shell_global_counters
  1DC: File  (RW-)   C:\ProgramData\NVIDIA\Updatus\updtclient.log
  214: File  (RW-)   C:\ProgramData\NVIDIA\Updatus\journalBS.jour.dat
  338: Section       \BaseNamedObjects\__ComCatalogCache__
  33C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  3D8: File  (R-D)   C:\Windows\SysWOW64\en-US\mswsock.dll.mui
  5F4: File  (R-D)   C:\Windows\SysWOW64\en-US\dnsapi.dll.mui
------------------------------------------------------------------------------
UNS.exe pid: 6552 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  25C: Section       \BaseNamedObjects\windows_shell_global_counters
  2B4: Section       \BaseNamedObjects\__ComCatalogCache__
------------------------------------------------------------------------------
devenv.exe pid: 1528 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   1C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_ba245425e0986353
   58: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   5C: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\TestDriven.dll
  134: Section       \Windows\Theme577161652
  138: Section       \Sessions\1\Windows\Theme2445879050
  14C: Section       \Sessions\1\BaseNamedObjects\MSSCCIShareData1528
  170: Section       \BaseNamedObjects\__ComCatalogCache__
  174: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1B4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  218: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  220: Section       \BaseNamedObjects\__ComCatalogCache__
  22C: Section       \BaseNamedObjects\Cor_Private_IPCBlock_v4_1528
  264: Section       \...\Cor_SxSPublic_IPCBlock
  330: File  (R--)   C:\Windows\assembly\pubpol71.dat
  338: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.Interop.10.0\10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.Interop.10.0.dll
  340: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.Interop.10.0\10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.Interop.10.0.dll
  35C: Section       \Sessions\1\BaseNamedObjects\SqmData_VisualStudioMainSqmSession_000005f8_S-1-5-21-2054895221-449969794-1663927727-1000
  36C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.dll
  374: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.UI.Internal.resources\v4.0_11.0.0.0_en_b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.UI.Internal.resources.dll
  3C0: File  (RW-)   C:\Windows\SysWOW64
  3E8: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Platform.WindowManagement.resources\v4.0_11.0.0.0_en_b03f5f7f11d50a3a\Microsoft.VisualStudio.Platform.WindowManagement.resources.dll
  420: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  464: Section       \BaseNamedObjects\RotHintTable
  478: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\dte80a.olb
  49C: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\u2c1sywi.tti\NotifyPropertyWeaverVsPackage.dll
  4A4: Section       \Sessions\1\BaseNamedObjects\MSSCCIShareData1528
  4DC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.DataDesign.DataSetDesigner\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.DataDesign.DataSetDesigner.dll
  4E0: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  4E4: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Design\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Design.dll
  4F0: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll
  500: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.WizardFrameworkVS.dll
  504: File  (RW-)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE
  514: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\dte80.olb
  538: File  (RW-)   C:\Windows\SysWOW64
  53C: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\tmsqrwzu.pvx\tangible.Modelling.UseCases.DslPackage.dll
  544: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Web.CSS.dll
  550: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\SilverlightExecute.TestRunner.exe
  564: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\MutantDesign.ManagedAddIns.dll
  568: File  (R-D)   C:\Windows\assembly\GAC\Extensibility\7.0.3300.0__b03f5f7f11d50a3a\Extensibility.dll
  5C4: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\VisualStudio\11.0\AutoRecoverDat
  5CC: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.WizardFramework.dll
  5D8: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.WizardFrameworkVS.dll
  5E0: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  5F0: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\MutantDesign.ManagedAddIns.XmlSerializers.dll
  600: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\TestDriven.Services.dll
  610: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\TestDriven.TestRunner.Server.dll
  62C: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Launch.bmp
  630: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Launch.bmp
  634: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Abort.bmp
  638: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Debug.bmp
  63C: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Coverage.bmp
  640: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Performance.bmp
  644: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Runtime.bmp
  648: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Runtime.bmp
  64C: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Silverlight.bmp
  650: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  654: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  65C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  660: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  674: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  680: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  684: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
  6C4: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_32.db
  6C8: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\VisualStudio\11.0\AutoRecoverDat
  6CC: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
  6D4: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Reflector.bmp
  6D8: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_96.db
  6DC: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\DotTrace.bmp
  6E0: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\NCover.bmp
  6E4: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Build.bmp
  6E8: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\NUnit.bmp
  6EC: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\MbUnit.bmp
  6F0: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Zanebug.bmp
  6F4: File  (R--)   C:\Program Files (x86)\TestDriven.NET 3\icons\Gallio_Icarus.bmp
  6F8: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\TestDriven.TestRunner.dll
  704: File  (R-D)   C:\Windows\assembly\GAC\Microsoft.VisualStudio.CommandBars\8.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.CommandBars.dll
  70C: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Design\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Design.dll
  710: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\msenv.dll
  714: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.WizardFramework.dll
  718: File  (R-D)   C:\Program Files (x86)\TestDriven.NET 3\JetBrains.dotTrace.Integration.dll
  720: File  (R-D)   C:\Windows\SysWOW64\stdole2.tlb
  738: File  (RWD)   C:\Users\Sam
  754: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Interop.dll
  758: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\4kd02omt.yer\tangible.Modelling.Components.DslPackage.dll
  770: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Debugger.Engine.dll
  788: File  (R-D)   C:\Windows\WinSxS\x86_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_97bbef07b78367ad\comctl32.dll.mui
  794: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC#\VCSPackages\csproj.dll
  7C0: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\vslangproj100.olb
  7C8: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\vslangproj90.olb
  7D0: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\vslangproj80.olb
  7D8: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\vslangproj2.olb
  7E4: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\vslangproj.olb
  7E8: File  (R-D)   C:\Windows\SysWOW64\en-GB\duser.dll.mui
  7EC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.c..-controls.resources_6595b64144ccf1df_6.0.9200.16384_en-gb_97bbef07b78367ad
  7F4: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.CSharp.Services.Language.Interop.dll
  7F8: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.CSharp.Services.Language.Interop.dll
  7FC: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.CallHierarchy.Package.Definitions.dll
  804: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.CallHierarchy.Package.Definitions.dll
  818: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.Platform.VSEditor.Interop.dll
  81C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.Platform.VSEditor.Interop.dll
  828: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\ComponentModelCache\Microsoft.VisualStudio.Default.cache
  854: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TemplateProviders\Microsoft.VisualStudio.TemplateProviders.Implementation.dll
  870: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Host.dll
  878: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Data.Framework\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Data.Framework.dll
  890: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\s3grgubb.05n\tangible.SyntaxEditor.Wpf.dll
  89C: File  (RWD)   C:\Program Files (x86)
  8A4: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\s3grgubb.05n\tangible.Text.Wpf.dll
  8B0: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.TextManager.Interop.11.0\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.TextManager.Interop.11.0.dll
  8C8: File  (RW-)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE
  8D0: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\hiumy31i.vtr\tangible.Modelling.StateDiagrams.DslPackage.dll
  8D8: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Progression.CodeSchema\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Progression.CodeSchema.dll
  8EC: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.Interop.11.0\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.Interop.11.0.dll
  8FC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll
  900: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll
  908: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Debugger.Interop.Internal\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Debugger.Interop.Internal.dll
  90C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Debugger.Engine.dll
  950: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.DataDesign.Common\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.DataDesign.Common.dll
  958: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\Microsoft\VsGraphics\VsGraphicsDebuggerPkg.dll
  988: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll
  9D0: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\extensionSdks.en-US.cache
  9EC: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\s0eqyz3y.5z5\Microsoft.Web.Languages.dll
  9F4: File  (R-D)   C:\Windows\assembly\GAC\Microsoft.VisualStudio.Debugger.InteropA\9.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Debugger.InteropA.dll
  A04: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\s0eqyz3y.5z5\WebEssentials2012.dll
  A08: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\s3grgubb.05n\tangible.T4Editor2010WPF.dll
  A10: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\s3grgubb.05n\tangible.DebugHelper.dll
  A24: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Progression.CodeSchema\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Progression.CodeSchema.dll
  A28: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_32\System.EnterpriseServices\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.EnterpriseServices.Wrapper.dll
  A2C: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.Interop.11.0\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.Interop.11.0.dll
  A30: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_32\System.Transactions\v4.0_4.0.0.0__b77a5c561934e089\System.Transactions.dll
  A58: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Progression.LanguageService.CSharp\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Progression.LanguageService.CSharp.dll
  A64: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\dte100.olb
  A6C: File  (R-D)   C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\dte90.olb
  A7C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Microsoft.VisualStudio.TestWindow.Interfaces.dll
  A84: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\DataDesign\Microsoft.VSDesigner.BootstrapPackage.dll
  A88: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.Data.Entity.Design.BootstrapPackage.dll
  A8C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Microsoft.VisualStudio.TestWindow.dll
  AA4: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.IDE.ToolboxControlsInstaller.dll
  AAC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.XmlEditor\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.XmlEditor.dll
  AB8: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.IDE.ToolboxControlsInstaller.dll
  AC0: File  (RWD)   C:\Windows\Fonts\segoeui.ttf
  ADC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Toolbox.resources\v4.0_11.0.0.0_en_b03f5f7f11d50a3a\Microsoft.VisualStudio.Toolbox.resources.dll
  AEC: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\o04lnr10.ro1\tangible.Modelling.ClassDiagrams.DslPackage.dll
  AF8: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Windows.Forms.VisualStudio.11.0\v4.0_4.0.0.0__b77a5c561934e089\System.Windows.Forms.VisualStudio.11.0.dll
  B08: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  B28: File  (RWD)   C:\Windows\Fonts\segoeuib.ttf
  B7C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Package.dll
  B90: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_idx.db
  BA4: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.tlb
  BA8: File  (R-D)   C:\Users\Sam\AppData\Local\Temp\mnjxjbc2.zqh
  BAC: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.tlb
  BB0: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Windows.Forms.tlb
  BBC: Section       \Sessions\1\BaseNamedObjects\Microsoft_VS80_JIT_Debugger-5f8
  C18: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Debugger.Interop.10.0\10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Debugger.Interop.10.0.dll
  C34: Section       \Sessions\1\BaseNamedObjects\Microsoft_VS90_causality_sharedmemory-4248
  DA8: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.dll
  DB4: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Security.dll
  DB8: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.dll
  DC0: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.XML.dll
  DD4: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Runtime.Serialization.dll
  DDC: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VSDesigner.Management.dll
  DEC: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.Entity.dll
  DF4: File  (R-D)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Core.dll
  E00: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.ServerExplorer.dll
  E08: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.DataDesign.Interfaces\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.DataDesign.Interfaces.dll
  E20: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.DataTools.Interop.dll
  E48: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.Data.ConnectionUI.dll
  E50: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Data.Providers.SqlCompact\v4.0_4.0.1.0__89845dcd8080cc91\Microsoft.VisualStudio.Data.Providers.SqlCompact.dll
  E58: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.Data.ConnectionUI.Dialog.dll
  E74: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.DataTools.dll
  E7C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Compatibility.dll
  E84: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Providers.SqlServer.dll
  EA8: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.DataDesign.Common\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.DataDesign.Common.dll
  EAC: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\PrivateAssemblies\Microsoft.Expression.DesignHost.dll
  EB0: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\PrivateAssemblies\Microsoft.Expression.DesignHost.dll
  EB4: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_32\System.Data\v4.0_4.0.0.0__b77a5c561934e089\System.Data.dll
  EBC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.DataDesign.Interfaces\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.DataDesign.Interfaces.dll
  EC0: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.DataTools.Interop.dll
  EC4: File  (R-D)   C:\Windows\assembly\GAC\Microsoft.VisualStudio.Debugger.InteropA\9.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Debugger.InteropA.dll
  EC8: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Interop.dll
  ECC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Data.Framework\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Data.Framework.dll
  ED0: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Debugger.Interop.Internal\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Debugger.Interop.Internal.dll
  ED4: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.DataTools.dll
  EE0: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.SqlServerCe.Client\4.0.0.0__89845dcd8080cc91\Microsoft.SqlServerCe.Client.dll
  EE8: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Providers.Oracle.dll
  EFC: File  (R-D)   C:\Windows\assembly\GAC_MSIL\System.Data.SqlServerCe\4.0.0.0__89845dcd8080cc91\System.Data.SqlServerCe.dll
  F00: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Data.Providers.Common.dll
  F24: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.ServiceModel\v4.0_4.0.0.0__b77a5c561934e089\System.ServiceModel.dll
  F78: Section       \BaseNamedObjects\windows_shell_global_counters
  F7C: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.vspManagementUI\v4.0_4.0.1.0__89845dcd8080cc91\Microsoft.VisualStudio.vspManagementUI.dll
  FA0: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.ManagedInterfaces.WCF\9.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.ManagedInterfaces.WCF.dll
  FA4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  FA8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  FC4: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\rfrrsnos.5wu\tangible.Modelling.ActivityDiagram.DslPackage.dll
  FD0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  FF4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 1030: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 1038: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{7A2BA096-1DFB-4CB0-AC3E-3707D73D274C}.2.ver0x0000000000000003.db
 103C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
 1040: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{924F06F1-BDEF-4238-B81A-778441D20340}.2.ver0x0000000000000002.db
 1068: Section       \Sessions\1\BaseNamedObjects\SessionImmersiveColorSet
 1094: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.Web.CSS.dll
 1108: File  (R-D)   C:\Windows\SysWOW64\en-GB\msctfui.dll.mui
 1124: File  (R-D)   C:\Users\Sam\AppData\Local\Microsoft\VisualStudio\11.0\Extensions\ne3bbuqn.u2l\tasl.Dsl.DslPackage.dll
 1140: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_256.db
 114C: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_256.db
 119C: File  (R-D)   C:\Windows\SysWOW64\en-US\imageres.dll.mui
 11B0: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.IdentityModel\v4.0_4.0.0.0__b77a5c561934e089\System.IdentityModel.dll
 1234: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_idx.db
 1240: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\thumbcache_idx.db
 127C: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.WCFReference.Interop\9.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.WCFReference.Interop.dll
 12B8: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.XmlEditor\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.XmlEditor.dll
 12E0: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_48.db
 1354: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.ExtensionManager.Implementation\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.ExtensionManager.Implementation.dll
 13AC: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VSDesigner.WCF\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VSDesigner.WCF.dll
 1410: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.TemplateWizardInterface\8.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.TemplateWizardInterface.dll
 145C: File  (R-D)   C:\Windows\assembly\GAC_MSIL\VSLangProj110\11.0.0.0__b03f5f7f11d50a3a\VSLangProj110.dll
 1490: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_48.db
 14A4: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Microsoft.VisualStudio.ServerExplorer.dll
 14B8: File  (R-D)   C:\Windows\SysWOW64\en-US\mswsock.dll.mui
 15E8: File  (RWD)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Explorer\iconcache_16.db
 1604: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_32\System.Data.OracleClient\v4.0_4.0.0.0__b77a5c561934e089\System.Data.OracleClient.dll
 170C: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Package.LanguageService.11.0\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Package.LanguageService.11.0.dll
 1728: File  (RWD)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.xml
 1748: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\WorkflowDesigner\Microsoft.VisualStudio.Activities.CSharp.dll
 175C: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TemplateProviders\Microsoft.VisualStudio.TemplateProviders.Implementation.dll
 178C: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Diagnostics.ServiceModelSink\v4.0_4.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Diagnostics.ServiceModelSink.dll
 17A8: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.ServiceModel.Internals\v4.0_4.0.0.0__31bf3856ad364e35\System.ServiceModel.Internals.dll
 181C: File  (RWD)   C:\Windows\Fonts\segoeuiz.ttf
 1820: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Zip.11.0\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Zip.11.0.dll
 1944: Section       \BaseNamedObjects\NLS_CodePage_1252_3_2_0_0
 19C0: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
 19D0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
 1A7C: File  (RWD)   C:\Windows\Fonts\segoeuii.ttf
 1B30: File  (RWD)   C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.xml
 1B48: File  (R-D)   C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Editors.WCF\v4.0_11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Editors.WCF.dll
------------------------------------------------------------------------------
Microsoft.VisualStudio.PerfWatson.exe pid: 2964 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   6C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   C0: Section       \...\Cor_SxSPublic_IPCBlock
   C4: Section       \BaseNamedObjects\Cor_Private_IPCBlock_v4_2964
  1F8: File  (R--)   C:\Windows\assembly\pubpol71.dat
  240: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  2C8: File  (R-D)   C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\mfhpjf5y.dxe\WindowsErrorReporting.Interop.dll
  344: Section       \Windows\Theme577161652
  348: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
conhost.exe pid: 4428 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   28: File  (R-D)   C:\Windows\System32\en-US\conhost.exe.mui
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   80: Section       \Windows\Theme577161652
   84: Section       \Sessions\1\Windows\Theme2445879050
------------------------------------------------------------------------------
EpisodeTracker.vshost.exe pid: 4248 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Users\Sam\Documents\Projects\Personal\EpisodeTracker\MediaReign.EpisodeTracker\bin\Debug
   60: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   B0: Section       \...\Cor_SxSPublic_IPCBlock
   B4: Section       \BaseNamedObjects\Cor_Private_IPCBlock_v4_4248
  1B0: File  (R--)   C:\Windows\assembly\pubpol71.dat
  1B4: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.HostingProcess.Utilities\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.HostingProcess.Utilities.dll
  1F4: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.HostingProcess.Utilities.Sync\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.HostingProcess.Utilities.Sync.dll
  214: File  (R-D)   C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.Debugger.Runtime\11.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Debugger.Runtime.dll
  220: Section       \Sessions\1\BaseNamedObjects\__FEQA_IPCINFO__1098
  23C: Section       \BaseNamedObjects\__ComCatalogCache__
------------------------------------------------------------------------------
jucheck.exe pid: 2852 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Windows\SysWOW64
   1C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
   5C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  100: Section       \Windows\Theme577161652
  104: Section       \Sessions\1\Windows\Theme2445879050
  184: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  2D4: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
  2E4: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  364: File  (RWD)   C:\Users\Sam\AppData\Roaming\Microsoft\SystemCertificates\My
------------------------------------------------------------------------------
chrome.exe pid: 5400 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  134: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Windows\Theme577161652
  1DC: Section       \Sessions\1\Windows\Theme2445879050
  20C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  258: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 2284 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  220: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  27C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 812 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C8: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1E0: Section       \Windows\Theme577161652
  1E4: Section       \Sessions\1\Windows\Theme2445879050
  228: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  308: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 6476 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  134: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  140: Section       \Windows\Theme577161652
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  220: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  244: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 4468 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   44: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   F4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  128: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  15C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  164: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  16C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  180: Section       \Windows\Theme577161652
  184: Section       \Sessions\1\Windows\Theme2445879050
  1E8: Section       \BaseNamedObjects\__ComCatalogCache__
  1F0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Google Talk Plugin\gtbplugin.log
  20C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 2028 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  174: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  20C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
googletalkplugin.exe pid: 4208 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
   30: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
   58: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  12C: Section       \Windows\Theme577161652
  130: Section       \Sessions\1\Windows\Theme2445879050
  17C: Section       \BaseNamedObjects\__ComCatalogCache__
  194: Section       \BaseNamedObjects\__ComCatalogCache__
  1AC: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  328: Section       \Sessions\1\BaseNamedObjects\1070
  348: File  (R--)   C:\Users\Sam\AppData\Local\Google\Google Talk Plugin\googletalkplugin_port
  364: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  378: File  (R--)   C:\Users\Sam\AppData\Local\Google\Google Talk Plugin\googletalkplugin_ws_port
  3AC: File  (R-D)   C:\Windows\SysWOW64\en-US\MMDevAPI.dll.mui
  3BC: File  (R-D)   C:\Windows\SysWOW64\en-US\advapi32.dll.mui
------------------------------------------------------------------------------
taskhost.exe pid: 4576 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   6C: File  (R-D)   C:\Windows\System32\en-US\taskhost.exe.mui
   74: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   D4: Section       \BaseNamedObjects\__ComCatalogCache__
   EC: Section       \BaseNamedObjects\__ComCatalogCache__
   F0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  124: File  (---)   C:\ProgramData\Microsoft\RAC\StateData\RacMetaData.dat
  17C: File  (R--)   C:\ProgramData\Microsoft\RAC\StateData\RacDatabase.sdf
  198: Section       \...\{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22795:sqlce_se_ver
  19C: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22795:sqlce_se_mem
  1A0: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22795:sqlce_se_mem_ext
  1A8: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22795:sqlce_se_lks
  1B4: File  (RW-)   C:\ProgramData\Microsoft\RAC\Temp\sql2B9F.tmp
  1C0: Section       \...\{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_658629:sqlce_se_ver
  1C4: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_658629:sqlce_se_mem
  1C8: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_658629:sqlce_se_mem_ext
  1D0: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_658629:sqlce_se_lks
  428: File  (---)   C:\ProgramData\Microsoft\RAC\StateData\RacEtwData.dat
  42C: File  (---)   C:\ProgramData\Microsoft\RAC\StateData\RacWmiEventData.dat
  464: File  (RW-)   C:\ProgramData\Microsoft\RAC\PublishedData\RacWmiDatabase.sdf
  47C: File  (---)   C:\ProgramData\Microsoft\RAC\StateData\RacEventData.dat
  48C: Section       \...\{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22809:sqlce_se_ver
  490: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22809:sqlce_se_mem
  494: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22809:sqlce_se_mem_ext
  49C: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_196608_22809:sqlce_se_lks
  4A8: File  (RW-)   C:\ProgramData\Microsoft\RAC\Temp\sql2DD3.tmp
  4B4: Section       \...\{c1febab0-d9dc-11e0-8755-806e6f6e6963}_65536_658630:sqlce_se_ver
  4B8: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_65536_658630:sqlce_se_mem
  4BC: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_65536_658630:sqlce_se_mem_ext
  4C4: Section       \...\0:{c1febab0-d9dc-11e0-8755-806e6f6e6963}_65536_658630:sqlce_se_lks
  4F0: Section       \BaseNamedObjects\windows_shell_global_counters
  4F4: File  (---)   C:\ProgramData\Microsoft\RAC\StateData\RacDataBookmarks.dat
  4F8: File  (R-D)   C:\Windows\System32\en-US\advapi32.dll.mui
  510: File  (---)   C:\ProgramData\Microsoft\RAC\StateData\RacWmiDataBookmarks.dat
  534: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
  71C: File  (R-D)   C:\Windows\System32\en-US\mswsock.dll.mui
------------------------------------------------------------------------------
LCDClock.exe pid: 7860 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Program Files\Logitech Gaming Software\plugins\LCDAppletsMono-8.12.072\Applets\x64
   1C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
   24: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   6C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  104: Section       \Windows\Theme577161652
  108: Section       \Sessions\1\Windows\Theme2445879050
  128: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
------------------------------------------------------------------------------
PresentationFontCache.exe pid: 4472 NT AUTHORITY\LOCAL SERVICE
    8: File  (RW-)   C:\Windows\System32
   3C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   50: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   5C: Section       \BaseNamedObjects\Cor_Private_IPCBlock_4472
   74: Section       \BaseNamedObjects\Cor_Public_IPCBlock_4472
   CC: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\security.config.cch
   D0: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\enterprisesec.config.cch
  124: Section       \BaseNamedObjects\windows_shell_global_counters
  134: File  (R-D)   C:\Windows\ServiceProfiles\LocalService\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\64bit\security.config.cch
  17C: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_64\indexee.dat
  1A4: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  1A8: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  1B8: File  (R--)   C:\Windows\assembly\pubpol71.dat
  1C0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  1D8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  270: Section       \BaseNamedObjects\Font Cache Mapping 87a8974c-f1c0-4af7-951a-43f67798fc5c
------------------------------------------------------------------------------
Steam.exe pid: 8624 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   38: Section       \Windows\Theme577161652
   58: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   5C: Section       \Sessions\1\Windows\Theme2445879050
  124: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
  190: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  1B8: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_3
  24C: File  (RW-)   C:\Program Files (x86)\Steam\steam.log
  388: File  (RW-)   C:\Program Files (x86)\Steam\debug.log
  3C0: Section       \Sessions\1\BaseNamedObjects\libcef_1173556931566878164
  3C4: File  (RW-)   C:\Program Files (x86)\Steam
  3EC: Section       \Sessions\1\BaseNamedObjects\Steam_{E9FD3C51-9B58-4DA0-962C-734882B19273}_Pid:000021B0
  490: Section       \Sessions\1\BaseNamedObjects\Steam3Master_SharedMemFile
  4F0: File  (RW-)   D:\Program Files\Steam\steamapps\left 4 dead 2 client.ncf
  50C: File  (R--)   C:\Program Files (x86)\Steam\logs\connection_log.txt
  518: Section       \Sessions\1\BaseNamedObjects\STEAM_DRM_IPC
  550: File  (R-D)   C:\Windows\SysWOW64\en-US\wdmaud.drv.mui
  574: File  (R-D)   C:\Windows\SysWOW64\en-US\MMDevAPI.dll.mui
  5A4: Section       \BaseNamedObjects\__ComCatalogCache__
  5BC: Section       \BaseNamedObjects\__ComCatalogCache__
  5C0: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  5D0: File  (RWD)   C:\Windows\Fonts\arial.ttf
  618: Section       \Sessions\1\BaseNamedObjects\GameOverlayRender_PIDStream_mem-IPCWrapper
  61C: File  (R--)   C:\Program Files (x86)\Steam\logs\appinfo_log.txt
  620: File  (R--)   C:\Program Files (x86)\Steam\logs\cloud_log.txt
  69C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  710: File  (RW-)   C:\Program Files (x86)\Steam\config\htmlcache\Cookies
  71C: File  (R--)   C:\Program Files (x86)\Steam\logs\content_log.txt
  738: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  744: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  75C: Section       \BaseNamedObjects\windows_shell_global_counters
  78C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  790: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  7B4: File  (RW-)   D:\Program Files\Steam\steamapps\winui.gcf
  7D0: File  (R--)   C:\Program Files (x86)\Steam\logs\download_log.txt
  7E4: File  (RW-)   D:\Program Files\Steam\steamapps\sourceinit.gcf
  814: File  (RWD)   C:\Windows\Fonts\ariali.ttf
  86C: File  (RWD)   C:\Windows\Fonts\arialbd.ttf
  890: Section       \Sessions\1\BaseNamedObjects\21b0
  8C0: File  (R-D)   C:\Windows\SysWOW64\en-US\fwpuclnt.dll.mui
  8C4: File  (R-D)   C:\Windows\SysWOW64\en-US\advapi32.dll.mui
  C08: File  (RW-)   D:\Program Files\Steam\steamapps\left 4 dead 2 common.ncf
  CF8: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_1
  D10: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  D50: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_0
  DFC: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_2
  E08: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_2
  E30: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\index
  E84: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\index
  EB4: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  F0C: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_1
  F50: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_3
  FA8: File  (RWD)   C:\Program Files (x86)\Steam\config\htmlcache\data_0
------------------------------------------------------------------------------
DisplayFusion.exe pid: 6828 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Program Files (x86)\DisplayFusion
   38: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   54: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   74: Section       \BaseNamedObjects\Cor_Public_IPCBlock_6828
   80: Section       \BaseNamedObjects\Cor_Private_IPCBlock_6828
   D8: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\security.config.cch
   DC: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\enterprisesec.config.cch
  130: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  138: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
  148: File  (R-D)   C:\Users\Sam\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\64bit\security.config.cch
  184: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_64\indexee.dat
  188: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  44C: File  (R--)   C:\Windows\assembly\pubpol71.dat
  454: Section       \BaseNamedObjects\NLS_CodePage_1252_3_2_0_0
  48C: Section       \Windows\Theme577161652
  490: Section       \Sessions\1\Windows\Theme2445879050
  49C: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  4A0: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  4A4: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  4C8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
  53C: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  6F4: Section       \BaseNamedObjects\__ComCatalogCache__
  70C: Section       \BaseNamedObjects\__ComCatalogCache__
  710: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  71C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  724: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  728: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  72C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  738: Section       \BaseNamedObjects\windows_shell_global_counters
  798: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  81C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  824: Section       \Sessions\1\BaseNamedObjects\BFS_DF_NOTIFYFILE_4e4e5468-8c6d-466a-93c3-79888b6eac84
  87C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  974: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  AC8: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  B58: Section       \BaseNamedObjects\netfxcustomperfcounters.1.0.net clr networking
------------------------------------------------------------------------------
DisplayFusionAppHook.exe pid: 2248 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   14: File  (RW-)   C:\Program Files (x86)\DisplayFusion
   58: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   74: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
   8C: Section       \BaseNamedObjects\Cor_Public_IPCBlock_2248
   98: Section       \BaseNamedObjects\Cor_Private_IPCBlock_2248
   F4: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\security.config.cch
   F8: File  (R-D)   C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\enterprisesec.config.cch
  148: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  15C: File  (R-D)   C:\Users\Sam\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\security.config.cch
  198: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_32\index1e1.dat
  1A0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  1A4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
  45C: Section       \Windows\Theme577161652
  478: Section       \Sessions\1\Windows\Theme2445879050
  484: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_d089c358442de345
  488: File  (R--)   C:\Windows\assembly\pubpol71.dat
  498: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  4A0: File  (R-D)   C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
------------------------------------------------------------------------------
SteamService.exe pid: 4800 NT AUTHORITY\SYSTEM
    C: File  (RW-)   C:\Windows
   40: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Program Files (x86)\Steam
  16C: Section       \BaseNamedObjects\Steam_{E9FD3C51-9B58-4DA0-962C-734882B19273}_Pid:000012C0
  18C: Section       \BaseNamedObjects\SteamClientService_SharedMemFile
------------------------------------------------------------------------------
chrome.exe pid: 7796 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1DC: Section       \Windows\Theme577161652
  1E0: Section       \Sessions\1\Windows\Theme2445879050
  23C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  314: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 9168 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1DC: Section       \Windows\Theme577161652
  1E0: Section       \Sessions\1\Windows\Theme2445879050
  1E4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  22C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 1560 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  144: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1C4: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1DC: Section       \Windows\Theme577161652
  1E0: Section       \Sessions\1\Windows\Theme2445879050
  238: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  2D0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
cmd.exe pid: 3140 BLACK-BEAUTY\Sam
   44: File  (RW-)   C:\Users\Sam\Desktop
   48: File  (R-D)   C:\Windows\System32\en-US\cmd.exe.mui
   68: File  (R--)   C:\Users\Sam\Desktop\out2.txt
------------------------------------------------------------------------------
conhost.exe pid: 6312 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows
   28: File  (R-D)   C:\Windows\System32\en-US\conhost.exe.mui
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   78: Section       \Windows\Theme577161652
   7C: Section       \Sessions\1\Windows\Theme2445879050
   90: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
   A8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   C8: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  134: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
------------------------------------------------------------------------------
audiodg.exe pid: 8632 \<unable to open process>
    4: File  (RW-)   C:\Windows
   E4: File  (R-D)   C:\Windows\System32\en-US\audiodg.exe.mui
   EC: Section       \BaseNamedObjects\__ComCatalogCache__
  150: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  154: Section       \BaseNamedObjects\__ComCatalogCache__
  228: Section       \BaseNamedObjects\windows_shell_global_counters
------------------------------------------------------------------------------
PotPlayerMini64.exe pid: 7748 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Windows\System32
   34: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   80: Section       \Windows\Theme577161652
   84: Section       \Sessions\1\Windows\Theme2445879050
   9C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
   E4: File  (R-D)   C:\Windows\System32\en-US\msvfw32.dll.mui
  13C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  168: Section       \Sessions\1\BaseNamedObjects\_Daum_Street_Mem_TRANS_AUTH64_
  178: Section       \Sessions\1\BaseNamedObjects\_Daum_Street_Mem1_ID64_
  1FC: File  (R-D)   C:\Windows\System32\en-US\MMDevAPI.dll.mui
  200: File  (R-D)   C:\Windows\System32\en-US\wdmaud.drv.mui
  260: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  274: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  2A4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  35C: Section       \Sessions\1\BaseNamedObjects\UrlZonesSM_Sam
  388: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  398: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  3BC: Section       \BaseNamedObjects\__ComCatalogCache__
  3D4: Section       \BaseNamedObjects\__ComCatalogCache__
  3D8: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  444: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  49C: File  (R-D)   C:\Windows\System32\en-US\devenum.dll.mui
  4A0: File  (R-D)   C:\Windows\System32\en-US\dsound.dll.mui
  664: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  680: Section       \Sessions\1\BaseNamedObjects\AMResourceMapping3-0x09-0x00064a
  6B0: File  (RWD)   D:\Downloads\Media\Archer (2009) - S03E07 - Crossing Over.avi
  6B4: File  (RWD)   D:\Downloads\Media\Archer (2009) - S03E07 - Crossing Over.avi
  760: Section       \Sessions\1\BaseNamedObjects\1e44
  854: Section       \Sessions\1\BaseNamedObjects\DirectSound Administrator shared thread array
  9B0: Section       \Sessions\1\BaseNamedObjects\windows_webcache_bloom_section_{6BB76D48-3D33-4858-BF7E-A3FA74E1EE4E}
------------------------------------------------------------------------------
ssh-agent.exe pid: 7536 BLACK-BEAUTY\Sam
   18: File  (RW-)   C:\Windows
   58: Section       \Sessions\1\BaseNamedObjects\7756.msys-1.0S3.shared.02011-07-20 17:52
   5C: Section       \Sessions\1\BaseNamedObjects\cygpid.1D70
   90: Section       \Sessions\1\BaseNamedObjects\7756.msys-1.0S3.Sam.02011-07-20 17:52
   94: Section       \Sessions\1\BaseNamedObjects\7756.msys-1.0S3.Sam.02011-07-20 17:52
   DC: File  (RW-)   C:\Users\Sam\AppData\Local\GitHub\PortableGit_8810fd5c2c79c73adcc73fd0825f3b32fdb816e7
  6C4: Section       \BaseNamedObjects\NLS_CodePage_1252_3_2_0_0
  CD8: File  (R-D)   C:\Windows\SysWOW64\en-US\mswsock.dll.mui
------------------------------------------------------------------------------
chrome.exe pid: 9160 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  238: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  244: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
chrome.exe pid: 88 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   84: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  13C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  140: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
  148: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1B0: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1B8: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1D0: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1E0: Section       \Windows\Theme577161652
  1E4: Section       \Sessions\1\Windows\Theme2445879050
  234: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
------------------------------------------------------------------------------
chrome.exe pid: 8816 BLACK-BEAUTY\Sam
   4C: File  (RW-)   C:\Windows
   8C: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
  134: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  19C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\theme_resources_100_percent.pak
  1A4: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\ui_resources_100_percent.pak
  1AC: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94\Locales\en-GB.pak
  1BC: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_893961408605e985
  1D4: Section       \Windows\Theme577161652
  1D8: Section       \Sessions\1\Windows\Theme2445879050
  22C: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\22.0.1229.94
  254: File  (RW-)   C:\Users\Sam\AppData\Local\Google\Chrome\Application\Dictionaries\en-GB-2-3.bdic
------------------------------------------------------------------------------
SearchProtocolHost.exe pid: 9852 NT AUTHORITY\SYSTEM
   10: File  (RW-)   C:\Windows\System32
   3C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
  300: Section       \BaseNamedObjects\UsGthrCtrlFltPipeMssGthrPipe20
  308: Section       \BaseNamedObjects\__ComCatalogCache__
  318: Section       \BaseNamedObjects\__ComCatalogCache__
  31C: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  37C: Section       \BaseNamedObjects\UsGthrFltPipeMssGthrPipe20_1
  470: Section       \BaseNamedObjects\windows_shell_global_counters
  48C: Section       \BaseNamedObjects\windows_shell_global_counters
  4C0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  4C4: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  4D0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  4D4: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
------------------------------------------------------------------------------
SearchFilterHost.exe pid: 7816 NT AUTHORITY\SYSTEM
    8: File  (RW-)   C:\Windows\System32
   50: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   E0: Section       \BaseNamedObjects\__ComCatalogCache__
   F8: Section       \BaseNamedObjects\__ComCatalogCache__
   FC: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  414: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  418: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{6AF0698E-D558-4F6E-9B3C-3716689AF493}.2.ver0x0000000000000006.db
  41C: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  420: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*{DDF571F2-BE98-426D-8288-1A9A39C3FDA2}.2.ver0x0000000000000002.db
  424: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  428: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
  8F0: Section       \BaseNamedObjects\C:*ProgramData*Microsoft*Windows*Caches*cversions.2.ro
------------------------------------------------------------------------------
sublime_text.exe pid: 9932 BLACK-BEAUTY\Sam
    C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   18: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
   1C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   40: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   C8: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
   E0: Section       \Windows\Theme577161652
   E4: Section       \Sessions\1\Windows\Theme2445879050
  10C: File  (RWD)   C:\Users\Sam\AppData\Roaming\Sublime Text 2\Packages
  164: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  18C: File  (RW-)   C:\Users\Sam\Desktop
  278: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  2E4: Section       \BaseNamedObjects\__ComCatalogCache__
  2FC: Section       \BaseNamedObjects\__ComCatalogCache__
  300: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  350: File  (RW-)   C:\Users\Sam\AppData\Local\Microsoft\Windows\Temporary Internet Files\counters.dat
  364: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
------------------------------------------------------------------------------
Expresso.exe pid: 7496 BLACK-BEAUTY\Sam
    8: File  (RW-)   C:\Program Files (x86)\Ultrapico\Expresso
   40: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
   54: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
   78: Section       \BaseNamedObjects\Cor_Public_IPCBlock_7496
   84: Section       \BaseNamedObjects\Cor_Private_IPCBlock_7496
   D4: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\security.config.cch
   D8: File  (R-D)   C:\Windows\Microsoft.NET\Framework64\v2.0.50727\CONFIG\enterprisesec.config.cch
  12C: Section       \Sessions\1\BaseNamedObjects\windows_shell_global_counters
  140: File  (R-D)   C:\Users\Sam\AppData\Roaming\Microsoft\CLR Security Config\v2.0.50727.312\64bit\security.config.cch
  17C: File  (R--)   C:\Windows\assembly\NativeImages_v2.0.50727_64\indexee.dat
  1A4: Section       \Windows\Theme577161652
  1A8: Section       \Sessions\1\Windows\Theme2445879050
  1B8: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.6910_none_88dc8c812fb1ba3f
  1CC: File  (R--)   C:\Windows\assembly\pubpol71.dat
  1D0: Section       \BaseNamedObjects\NLS_CodePage_1252_3_2_0_0
  1D8: File  (R-D)   C:\Program Files (x86)\Ultrapico\Expresso\Builder.dll
  244: Section       \BaseNamedObjects\__ComCatalogCache__
  25C: Section       \BaseNamedObjects\__ComCatalogCache__
  260: File  (R--)   C:\Windows\Registration\R00000000000d.clb
  27C: File  (R-D)   C:\Program Files (x86)\Ultrapico\Expresso\ToolStripExDll.dll
  290: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sorttbls.nlp
  298: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.gdiplus_6595b64144ccf1df_1.1.9200.16384_none_72771d4ecc1c3a4d
  2A8: File  (R-D)   C:\Windows\assembly\GAC_64\mscorlib\2.0.0.0__b77a5c561934e089\sortkey.nlp
  2B8: File  (R-D)   C:\Program Files (x86)\Ultrapico\Expresso\RegDecoder.dll
  2CC: File  (R-D)   C:\Windows\Fonts\StaticCache.dat
  2E4: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.9200.16384_none_418c2a697189c07f
  310: File  (RWD)   C:\Windows\Fonts\segoeui.ttf
  314: File  (RWD)   C:\Windows\Fonts\tahomabd.ttf
  344: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
------------------------------------------------------------------------------
handle.exe pid: 9180 BLACK-BEAUTY\Sam
   10: File  (RW-)   C:\Windows
   18: File  (RW-)   C:\Users\Sam\Desktop
   28: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_bf100cd445f4d954
   60: File  (RW-)   C:\Windows\WinSxS\x86_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_50944e7cbcb706e5
   68: File  (R--)   C:\Users\Sam\Desktop\out2.txt
------------------------------------------------------------------------------
handle64.exe pid: 9336 BLACK-BEAUTY\Sam
    C: File  (R--)   C:\Users\Sam\Desktop\out2.txt
   18: File  (RW-)   C:\Users\Sam\Desktop
   28: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.windows.common-controls_6595b64144ccf1df_5.82.9200.16384_none_7762d5fd3178b04e
   4C: File  (RW-)   C:\Windows\WinSxS\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.6871_none_08e717a5a83adddf
";
			var list = HandleHelper.ParseOutput(output);
			Assert.True(list.Count > 1);
			Assert.Equal(36, list.Count(p => p.ProcessName == "chrome"));
		}
	}
}
