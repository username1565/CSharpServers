set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=HTTPServer

::%csc% /main:HttpServer.HttpServer /out:HTTPServer.exe *.cs
::%csc% /main:HttpServer.HttpServer -reference:Mono.Data.Sqlite.dll /out:%program%.exe *.cs
%csc% /main:HttpServer.HttpServer -reference:Mono.Data.Sqlite.dll -reference:Chaos.NaCl.dll /out:%program%.exe *.cs

HTTPServer.exe 8082

pause