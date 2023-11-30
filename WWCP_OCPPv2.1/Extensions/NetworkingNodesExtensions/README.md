# OCPP Networking Nodes Extensions

The *OCPP Networking Nodes Extensions* allow you to to have networking nodes between charging stations
and charging station management systems to optimize communication flows within larger charge parks and
improve the overall network security.


## Steps

1. The networking node connects to the charging station management system and registers itself as a networking node.

2. The charging station connects to the networking node and registers itself as a charging station.

3. The networking node sends an UpdateNetworkTopologyRequest including a network topology information
to the charging station management system to inform the charging station management system about
the newly connected charging station.

4. The network topology information might be a list of directly connected charging stations and/or a (recursive)
list of other network topology information.

5. The charging station management system now is aware, that the charging station is reachable via the networking node.



## High Availablity Networking

1. Charging stations might maintain multiple networking connections to multiple networking nodes and/or charging station management systems.
In this case the charging station should include a *priority* to each network connection (setup) to indicate its preferred connection.

2. The charging station should send Heartbeats via all network connections to indicate its availability.
The heartbeat should also include the *priority* of the network connection.



### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
