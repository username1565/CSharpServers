csc /out:UDPServersMulticast.exe *.cs
#	UDP server without multicast group
mono UDPServersMulticast.exe 0.0.0.0 8081

#	UDP server with mulcicast group IP
mono UDPServersMulticast.exe 0.0.0.0 8081 235.5.5.11