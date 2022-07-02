csc /main:HttpServer.HttpServer -reference:Mono.Data.Sqlite.dll -reference:Chaos.NaCl.dll /out:HTTPServer.exe *.cs
mono HTTPServer.exe 8082 captcha
