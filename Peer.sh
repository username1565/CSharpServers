csc /out:Peer.exe *.cs /reference:Mono.Data.Sqlite.dll /reference:sqlite3.dll /platform:x86

#Peer.exe IP port MulticastGroupIP PeerDiscoveryInterval DBFilePath TableName KeyName ValueName

#SQLite3 storage
%program%.exe 0.0.0.0 8081 235.5.5.11 15 Hashtable.db3 KeyValue key value

#Simple TXT storage
%program%.exe 0.0.0.0 8081 235.5.5.11 15 Hashtable.txt
