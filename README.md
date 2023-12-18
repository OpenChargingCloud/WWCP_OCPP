# WWCP OCPP

This software libraries will allow the communication between World Wide Charging Protocol (WWCP) entities and entities implementing the _Open Charge Point Protocol Version 1.6/2.0.1/2.1_, which is defined by the [_Open Charge Alliance (OCA)_](http://www.openchargealliance.org). The focus of the *Open Charge Point Protocol* are all the communication aspects between e-mobility *charging stations*, *local nodes* and *Charging Station Operator Systems/Backends (CSMS)*.

This software also allows you to build standalone OCPP micro services and use case specific gateways between OCPP and your internal microservice architecture.

For more details on the *Open Charge Point Protocol* please visit http://www.openchargealliance.org.


## Versions

- **OCPP v2.1** is a based on an internal OCA specification (Draft 2 v0.44) and currently
under development. This version also comes with our [OCPP CSE](OCPP_CSE) extensions for
cryptographically signed messages, security polices, user roles, end-to-end digital
signed charging tariffs and charging tickets. This version was also tested on multiple
Open Charge Alliance Plugfests.
- **OCPP v2.0.1** is fully implemented and at least one tests exists for every charging
station or CSMS message. This versions is not longer actively maintained and justs remains
to reflect data structure changes within different OCPP version. Please use version v2.1.
instead.
- **OCPP v1.6** and the **Security Whitepaper** extensions are fully implemented
and at least one tests exists for every charging station or CSMS message. This
version was also tested on multiple *Open Charge Alliance Plugfests*.
- **OCPP v1.5** is no longer maintained. If you still need this version please
send us an e-mail.

## Implementation Details and Differences

The official protocol specifications of [OCPP v1.6](WWCP_OCPPv1.6/README.md), [OCPP v2.0.1](WWCP_OCPPv2.0.1/README.md) and [OCPP v2.1](WWCP_OCPPv2.1/README.md) have unfortunatelly still many protocol design flaws and security issues which are still not addressed properly by the *Open Charge Alliance Technical Working Group (TWG)*. Therefore this implementation provides a rich set of vendor extentions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods.

#### End-to-End Cyber Security

OCCP currently on ly offers *hop-by-hop* security via *Transport-Layer-Security (TLS)*. This only provides communication privacy and security against external attackers, but can not secure more complex networking scenarios e.g. with local controllers between the charging stations and the backend(s).

This project provides fills these gaps via *Digital Signatures* on all requests, responses and main data structures. Additional *Signature Policies* and *User Roles* will further limit the attack surface of your critical charging infrastructure and allow you to implement fine-grained access controls.


#### German Calibration Law (Eichrecht)

The software aspects of the German Calibration Law (Eichrecht) demand a secure communication of meter values, errors (and charging tariff information for ad hoc charging sessions) and a Charging Transparency Software for end users and regulators. Currently OCPP only supports minimal support for this regulatory requirements and thus the expected security benefit does not reach the end users.

This project provides additional data structures and commands to ensure a secure exchange of cryptographic and metrological data and digital calibration certificates.


#### Controllable Loads (German §14a EnWG)

...


#### European General Data Protection Regulation (GDPR)

Neither RFID cards, nor AutoCharge, smartphone apps and even not ISO 15118 Plug & Charge currently provide a GDPR compliant protection of end users and their data.

This project provides new data structures for encrypting privacy sensitive data and new EV driver authorization methods to avoid user tracking and data abuse.


#### Overlay Networking

While in the past a single charging station was connected directly to an operator backend, todays charging infrastructure is far more complex. Today we have to support efficient and secure operations for a cluster of colocated charging stations connected to the same grid connection point. Those charging stations oftern come from different vendors and some sort of local load management should reduce energy consumption peaks.

This project provides additional data structures and commands to enable overlay networking between charging stations, local controllers and operator backends. It also provides enhanced network configuration options for High-Available network setups to simplify the *NEVI requirements* in the US.


### Security Policy

We take security very seriously. Therefore we not only created the *End-to-End-Security* extensions but also the *Calibration Law* extensions, but also continuously review security vulnerabilities within the code, and provide fixes in a timely manner.

Please report security vulnerabilities in the Issues section of this repository. By keeping security issues visible, we leverage our community in helping fix them promptly.

If you are concerned sharing a vulnerability with the rest of the community, you can also send your report to: github@graphdefined.com


### Your participation

This software is Open Source under the **Apache 2.0 license** and in some parts **Affero GPL 3.0 license**. We appreciate your participation in this ongoing project, and your help to improve it and the e-mobility ICT in general. If you find bugs, want to request a feature or send us a pull request, feel free to use the normal GitHub features to do so. For this please read the Contributor License Agreement carefully and send us a signed copy or use a similar free and open license.
