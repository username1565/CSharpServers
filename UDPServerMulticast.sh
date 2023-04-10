csc /out:UDPServerMulticast.exe *.cs
#	UDP server without multicast group
mono UDPServerMulticast.exe 0.0.0.0 8081

#	UDP server with mulcicast group IP
mono UDPServerMulticast.exe 0.0.0.0 8081 235.5.5.11