set fdir=%WINDIR%\Microsoft.NET\Framework
set csc=%fdir%\v4.0.30319\csc.exe
set msbuild=%fdir%\v4.0.30319\msbuild.exe

set program=Peer

%csc% /out:%program%.exe *.cs /reference:Mono.Data.Sqlite.dll /reference:sqlite3.dll /platform:x86

::Peer.exe IP port MulticastGroupIP PeerDiscoveryInterval SyncDHTInterval DBFilePath TableName KeyName ValueName

::SQLite3 storage
%program%.exe 0.0.0.0 8081 235.5.5.11 15 30 Hashtable.db3 KeyValue key value

::Simple TXT storage
::%program%.exe 0.0.0.0 8081 235.5.5.11 15 30 Hashtable.txt

pause