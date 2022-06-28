csc /main:HttpServer.HttpServer -reference:Mono.Data.Sqlite.dll /out:HTTPServer.exe *.cs
mono HTTPServer.exe 8082
