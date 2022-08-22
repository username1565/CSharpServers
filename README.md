### Peer, written on CSharp (C#)

### Compile
Windows: `Peer.bat`
Linux (requires [mono](https://www.mono-project.com/docs/getting-started/install/linux/)): `./Peer.sh`

### Peer
[Decentralized networks](https://en.wikipedia.org/wiki/Peer-to-peer) constains peers.
Peer - it's server and client together.
# TCP-peer
TCP-peer - it's TCP-server and TCP-client together.
TCP-peer can connect to another known peers, from peers-list, so need to make and host the public peers, and hardcode the peers-list of known peers.
TCP-peers can supporting [Peer Exchange](https://en.wikipedia.org/wiki/Peer_exchange)

# UDP-peer
UDP-peer - it's UDP-server and UDP-client together.
UDP-peer can connect to another known peers, from peers-list, so need to make and host the public peers, and hardcode the peers-list of known peers.
UDP-peers can supporting [Peer Exchange](https://en.wikipedia.org/wiki/Peer_exchange) too.
Also, UDP-peer can supporting [Local Peer Discovery](https://en.wikipedia.org/wiki/Local_Peer_Discovery), because UDP-protocol supporting [UDP-multicast](https://metanit.com/sharp/net/5.2.php).


# TCPUDP-peer
TCP-peer and UDP-Peer (with multicast) - together.

## How peer works?
- Raise server-side - TCP/UDP-server with UDP-MultiCastGroupIP
- Raise client-side - TCP and UDP client (with MultiCastGroupIP)
  +  Try to connect the found TCP peers from peers list (addnode.txt),
     or/and try to make "Local Peer Discovery" (LDP) to find local peers, using UDP Multicast request,
     +   From client-side, send UDP-Multicast request to multicast-group IP, and wait responses.
         If somebody in LAN was responded, add his "IP:PORT" to peer list (Dictionary), and try to connect this peer
- Then process Peer Exchange (PEX), to synchronize peers-lists, and try to connect to known peers.
- Try to connect from client to server-side of another peers, and transfer some data (DHT) between peers, after connection.	
