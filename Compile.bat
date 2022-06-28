set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

%csc% /main:HttpServer.HttpServer /out:HTTPServer.exe *.cs

HTTPServer.exe 8082

pause