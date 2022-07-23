csc /main:HttpServer.HttpServer -reference:Mono.Data.Sqlite.dll -reference:Chaos.NaCl.dll /out:HTTPServer.exe *.cs

#	Without captcha
mono HTTPServer.exe 8082

#	With captcha
#mono HTTPServer.exe 8082 captcha
