# OCPP - Computer Science Edition

The *OCPP Computer Science Edition/Extensions* are Open Source vendor extensions defined by *GraphDefined GmbH* to modernize and improve the *Open Charge Point Protocol* in various ways that are currently not in scope of the *Open Charge Alliance*, but might become part of future versions of OCPP.

At present, the [Open Charge Alliance](https://www.openchargealliance.org) exercises a cautious approach towards introducing refinement and enhancements, as well as managing
backward and forward compatibility. This occasionally inhibits or delays key advancements in the fast-paced field of electric mobility.
This structured list of proposals seeks to address these issues in a way that everyone, can appreciate and contribute to.

## JSON-LD Context Support
- JSON-LD context information for all OCPP v2.1+ requests, respones and main data structures *(implemented)*.
- Full support of JSON-LD is currently out-of-scope.


## Binary Data Streams Extensions
The *HTTP Web Sockets* standard supports an efficient way to transport binary data like *Firmware Updates* or *Logging/Debugging data* and *Binary Event Streams*. The *Binary Data Streams Extensions* provide multiple extensions to make use of binary data transport:
- *GetFile*, *SendFile*, *DeleteFile*, *ListDirectory* are new requests/responses providing simple file transfer mechanisms just like the well known *File Transfer Protocol (FTP)*.
- More efficient transport of measurement data via *Binary Event Streams* (normal event streams are already part of the latest OCPP v2.1 draft).
- Currently OCPP uses out-of-band HTTP requests for *Firmware Updates* or to transport *Logging/Debugging data*. This is a high network security risc. These extensions will solve these security issues by moving those operations into the OCPP protocol, allowing network operators to DROP all non-OCPP communication within their networks.
- Setup and tear-down of binary channels and topologies in combination with the *Overlay Networking Extensions*.
- *Modbus/TCP* transport to allow a direct communication with e.g. smart energy meters located in the remote network of a charging location.
- *RS232/RS485* and *Modbus/RTU* transport to allow a direct communication with e.g. smart energy meters located in the remote network of a charging location.
- Generic *TCP/TLS-Proxy* to allow a direct communication with legacy devices located in the remote network of a charging location, e.g Germany *Smart Meter Gateways (SMGWs)*.
- As binary data is often very large compared to normal OCPP requests/responses an additional priority scheduling of message transmission is required to prioritize *"normal"* OCPP requests/responses over binary background transmissions.


## Overlay Networking Extensions
- Loosely related to the OCA internal *"OCPP Local CSMS"* and *"Routing Node"* concept, but not based on *Source Routing*, as this is known to not scale well.
- Every charging station, networking node, CSMS backend has an unique networking node identification.
- Charging Stations can still connect via Web Socket connections using the "traditional" RPC framework. The "Networking Nodes" care about the details of the Overlay Networking.
- *Networking Nodes* accept local HTTP Web Socket connections and aggregate them into a single HTTP Web Socket connection towards e.g. the CSMS.
- Connected charging stations, (local) networking nodes and CSMS backends exchange routing information about the reachability of devices based on the networking node identifications.
- Networking nodes can act as a **Security Gateway** or **OCPP Firewall** and make use of a [Signature policy](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtensions/README.md) e.g. to add additional signatures to requests/responses.
- **Anycast** and **Multicast** support allows sending information like e.g. EVSE status information or meter values to multiple destinations/backends.
- A charging station is no longer just a HTTP Web Socket client and a CSMS is no longer just a HTTP Web Socket server. Both can use both HTTP Web Socket roles.
  - After a network outtage e.g. a local networking node could initiate a HTTP Web Socket connection to a local charging station, instead of waiting for a reconnect.
  - OCPP local initial setup and maintenance is simplified as a charging station could also expose an OCPP endpoint via its WLAN access point and service people could use their notebook or a smartphone app for OCPP configuration and maintenance.
  - Exposing a limited public OCPP endpoint for EV (driver) communication via e.g. WLAN and accepting **Charging Tickets** from EVs and EV driver apps for secure offline charging.
  - The EV driver could "deliver" firmware or configuration updates to a charging station or fetch charging session data and deliver it to a charging station operator (*EV Driver Networking*).
- High availability for network configurations *upcoming*
  - Like DNS Service (SRV) Resource Records, https://de.wikipedia.org/wiki/SRV_Resource_Record
  - Better strategies and fall backs when remote endpoints are changed or login, passwords or certificates are changed.
  

## End-to-End Security Extensions
In addition to the use of *Transport Layer Security (TLS)* as defined within the official [OCPP Security Whitepaper](https://www.openchargealliance.org/about-us/info-en-whitepapers/) the [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtensions/README.md):
  - **Digital signatures** on *every OCPP v2.1+ request/response* and for some data structures like [end-to-end charging tariffs](../WWCP_OCPPv2.1/Extensions/E2EChargingTariffsExtensions/README.md), [charging tickets](../WWCP_OCPPv2.1/Extensions/ChargingTicketsExtension/README.md) and signature policies.
  - **Signature policies** define which request/response/data structure is signed or verified by which cryptographic keys. Multiple signatures per data structure are allowed, e.g. for *Overlay Networking* or secure grid load control. *(implemented)*.
  - **User roles** overcome the current *"everything is done by the same user"* approach of OCPP. Users are identified by the digital signature of their requests/responses and a **signature policy** defines which commands having which parameters can be used by which user role.
  - **Encapsulated Security Payload (ESP)** OCPP requests and responses can be encrypted, transported via the overlay network and be decrypted. ESP requests/responses are send via binary HTTP Web Socket streams.
  - **Public key infrastructure (PKI)** using specialized certificates defining what can be done with a certificate (*key usage*) within the large e-mobility and energy domain. This improves the security and flexibility of the end-to-end security extensions and overcomes the main weaknesses and too narrow focus of ISO 15118 based PKIs.


## Charging Tickets Extensions
The [Charging Tickets Extensions](../WWCP_OCPPv2.1/Extensions/ChargingTicketsExtensions/README.md) incorporate the [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtensions/README.md) to facilitate a more robust, privacy-aware certificate-based authorization mechanism for charging sessions at (offline) charging stations. Unlike traditional systems based on ISO 15118 Plug&Charge, this approach offers enhanced security features based on end-to-end encryption and short-lived anonymous certificates.

Charging tariffs are an officially planned feature of OCPP v2.1 Draft 3++ to support regulatory requirements especially for public charging under the **German Calibration Law (Eichrecht)** and will most likely be aligned to OCPI v2.2.1 tariffs. The [E2E Charging Tariffs Extensions](../WWCP_OCPPv2.1/Extensions/E2EChargingTicketsExtensions/README.md) are a prototype implementation of this approach incorporating the [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtensions/README.md). Unlike tariffs based on OCPI, this approach offers enhanced regulatory and security features based on end-to-end digital signatures and immutable data structures *(partial implemented)*.


## Controllable Grid Loads Extensions
**§ 14a EnWG** of the [German Energiewirtschaftsgesetz (*Energy Industry Act*)](https://www.gesetze-im-internet.de/enwg_2005/) focuses on utilizing renewable energy sources more effectively and to avoid grid congestion. The main idea is to allow energy suppliers and grid operators to control (indirectly) the consumption at end users in order to reduce peak loads and/or to improve the overall grid stability. In the context of e-mobility, this law provides a regulatory framework for the "**smart (de)charging**" of electric vehicles by delaying or accelerating the charging sessions based on real-time grid conditions. Again the [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtensions/README.md) are used to defined signed e.g. *grid-to-charging station* messages in order to control and to inform the EV driver (and a transparency platform) about interventions in a transparent and secure way. Also a feedback mechanism is provided to support the optimization of the grid interventions. Another design goal is to prevent the misuse of this technology by internal attackers for energy market manipulation.


