set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=TcpServer

%csc% /out:%program%.exe %program%.cs

:: show current encoding:
chcp

start telnet 127.0.0.1 1234

%program%.exe

pause