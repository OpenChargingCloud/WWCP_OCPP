WWCP OCPP v1.6
==============

This software will allow the communication between World Wide Charging
Protocol (WWCP) entities and entities implementing the
_Open ChargePoint Protocol Version 1.6_, which is defined by the
Open Charge Alliance. The focus of this protocol are the communication
aspects between a e-mobility charging station and its operator backend.
For more details on this protocol please visit http://www.openchargealliance.org.

## Differences to the official protocol

The following desribes differences of this implementation to the official protocol specification.
Most changes are intended to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)*  and the *German Calibration Law (Eichrecht)*.

### Charge Box Identification

Within the JSON implementation of OCPP we allow for every request the additional JSON property "chargeBoxId".
This will allow you to multiplex multiple OCPP communication channels over a single web sockets connection.
In the official protocol specification this value is only taken from the web sockets context.


### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
