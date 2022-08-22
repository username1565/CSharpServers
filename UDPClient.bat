set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=UdpClient

%csc% /out:%program%.exe *.cs

::Test connection to UDP server
%program%.exe 127.0.0.1 8081

pause