set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=ConsoleClient

%csc% /out:%program%.exe %program%.cs

%program%.exe 8082

pause