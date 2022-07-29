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

## How peer works?
1. Raise server.
2. Raise client.
3. Try to connect the found peers, from peers list, or/and try to make "Local Peer Discovery", to find local peers using UDP-multicast.
4. Try to connect from client to server-side of another peers, and transfer some data between peers, after connection.
