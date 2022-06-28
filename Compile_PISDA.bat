set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set dir=%~dp0
cd %dir%

::	Copy libs in current folder
xcopy "%dir%..\bin\Debug\System.Data.SQLite.dll" "%dir%" /E /H /C /I /Y
xcopy "%dir%..\bin\Debug\Mono.Data.Sqlite.dll" "%dir%" /E /H /C /I /Y

RENAME %dir%System.Data.SQLite.dll sqlite3.dll

::	Build with Sytem.Data.SQLite.dll
::%csc% /out:PISDA.exe /reference:System.Data.SQLite.dll PISDA.cs

::	Build with Mono.Data.Sqlite.dll (to run this on windows, need file sqlite3.dll, and it's "SQLite.Interop.dll", or "System.Data.SQLite.dll" with Interop inside, but renamed to "sqlite3.dll")
%csc% /out:PISDA.exe /reference:Mono.Data.Sqlite.dll /platform:x86 PISDA.cs

::	Run compiled program
PISDA.exe

pause