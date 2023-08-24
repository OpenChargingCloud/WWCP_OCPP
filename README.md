# WWCP OCPP

This software will allow the communication between World Wide Charging
Protocol (WWCP) entities and entities implementing the
_Open ChargePoint Protocol Version 1.6/2.0.1/2.1_, which is defined by the
Open Charge Alliance. The focus of this protocol are the communication
aspects between a e-mobility charging station and its operator backend.
For more details on this protocol please visit http://www.openchargealliance.org.

## Versions

- **OCPP v2.1** is a based on an internal OCA specification and currently under development.
- **OCPP v2.0.1** is fully implemented and at least one tests exists for
every charging station or CSMS message.
- **OCPP v1.6** and the **Security Whitepaper** extensions are fully implemented
and at least one tests exists for every charging station or CSMS message. This
version was also tested on multiple *Open Charge Alliance Plugfests*.
- **OCPP v1.5** is no longer maintained. If you still need this version please
send us an e-mail.

## Content

- **Implementation Details and Differences** for [OCPP v1.6](WWCP_OCPPv1.6/README.md), [OCPP v2.0.1](WWCP_OCPPv2.0.1/README.md)  and [OCPP v2.0.1](WWCP_OCPPv2.1/README.md) to the official protocol specification. The OCPP specification has unfortunatelly many flaws and security issues. This implementation provides extentions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)*  and the *German Calibration Law (Eichrecht)*.


### Your participation

This software is Open Source under the **Apache 2.0 license** and in some parts
**Affero GPL 3.0 license**. We appreciate your participation in this
ongoing project, and your help to improve it and the e-mobility ICT in
general. If you find bugs, want to request a feature or send us a pull
request, feel free to use the normal GitHub features to do so. For this
please read the Contributor License Agreement carefully and send us a signed
copy or use a similar free and open license.
