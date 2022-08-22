csc /out:TCPUDPMulticastClientsServers.exe *.cs

#or use LAN address, instead of 127.0.0.1
mono TCPUDPMulticastClientsServers.exe 127.0.0.1 8081 235.5.5.11
