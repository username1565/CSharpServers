set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

xcopy "..\bin\Debug\Mono.Data.Sqlite.dll" "." /Y
ECHO F|xcopy "..\bin\Debug\System.Data.SQLite.dll" ".\sqlite3.dll" /Y

%csc% /out:SQLite3Methods.exe /main:SQLite3.SQLite3Methods /reference:Mono.Data.Sqlite.dll /platform:x86 PISDA.cs SQLite3Methods.cs
SQLite3Methods.exe

pause