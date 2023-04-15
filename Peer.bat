set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=TCPUDPMulticastClientsServers

%csc% /out:%program%.exe *.cs

::or use LAN-address, instead of 127.0.0.1
%program%.exe 127.0.0.1 8081 235.5.5.11

pause
