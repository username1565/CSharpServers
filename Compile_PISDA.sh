#	Build with Sytem.Data.SQLite.dll
#csc /out:PISDA.exe /reference:System.Data.SQLite.dll PISDA.cs

#	Build with Mono.Data.Sqlite.dll (to run this on windows, need file sqlite3.dll, and it's "SQLite.Interop.dll", or "System.Data.SQLite.dll" with Interop inside, but renamed to "sqlite3.dll")
csc /out:PISDA.exe /reference:Mono.Data.Sqlite.dll /platform:x86 PISDA.cs

#	Run compiled program
mono PISDA.exe