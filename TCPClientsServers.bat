set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=TCPClientsServers

%csc% /out:%program%.exe *.cs

%program%.exe 0.0.0.0 8001

pause