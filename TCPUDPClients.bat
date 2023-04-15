set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=UdpClients

%csc% /out:%program%.exe *.cs

::UDP
::%program%.exe 127.0.0.1 8081

::UDP Multicast - response not receiving...
%program%.exe 0.0.0.0 8081 235.5.5.11

pause
