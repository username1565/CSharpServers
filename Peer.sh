csc /out:Peer.exe *.cs /reference:Mono.Data.Sqlite.dll /reference:sqlite3.dll /platform:x86
mono Peer.exe 0.0.0.0 8081 235.5.5.11
