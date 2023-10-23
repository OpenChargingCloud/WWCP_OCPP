# OCPP - Computer Science Edition

The following list of proposed updates, suggestions, and modifications are intended for upcoming iterations of the *Open Charge Point Protocol*.
At present, the *Open Charge Alliance* exercises a cautious approach towards introducing refinement and enhancements, as well as managing
backward and forward compatibility. This occasionally inhibits or delays key advancements in the fast-paced field of electric mobility.
This structured list of proposals seeks to address these issues in a way that everyone, can appreciate and contribute to.

- JSON-LD context information for JSON data structures *upcoming*
- Real End-to-End Security in addition to the *Transport Layer Security (TLS)* of the official **OCPP Security Whitepaper**
  - The [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtension/README.md) add digital signatures to every OCPP v2.1+ request/response, to some data structures like [charging tariffs](../WWCP_OCPPv2.1/Extensions/ChargingTariffsExtension/README.md) or [charging tickets](../WWCP_OCPPv2.1/Extensions/ChargingTicketsExtension/README.md) and allow multiple signatures per data structure.
  - The [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtension/README.md) also provide an user role model to overcome the current *"everything is done by the same user"* approach of OCPP. Users are identified by the digital signature of their requests/responses and a **signature policy** will decide which commands can be used by which user role.
- The [Charging Tickets Extensions](../WWCP_OCPPv2.1/Extensions/ChargingTicketsExtension/README.md) incorporate the **End-to-End Security Extensions** to facilitate a more robust, privacy-aware certificate-based authorization mechanism for charging sessions at (offline) charging stations. Unlike traditional systems based on ISO 15118 Plug&Charge, this approach offers enhanced security features based on end-to-end encryption and short-lived anonymous certificates.
- Steuerbaren Verbrauchseinrichtungen nach 14a EnWG. *upcoming*
  - Netzbetreiber Warnung an Transparenzplattform
  - Transparenzplattform > Ladestationsbetreiber
  - Bestätigung der Ladestationen an Transparenzplattform
- OCPP Overlay Networking *upcoming*
  - Based on the OCA internal *"OCPP Local CSMS"* concept
  - Connected charging stations, (local) nodes and backends exchange routing information about reachable nodes *upcoming*
  - OCPP **Security Gateway** makes use of the **signature policies** of the [End-to-End Security Extensions](../WWCP_OCPPv2.1/Extensions/E2ESecurityExtension/README.md) acting as "OCPP Firewall"
  - Some information, e.g. EVSE status information or meter values might be duplicated and send to multiple backends *upcoming*  
- OCPP HTTP/WebSocket Server on each charging station *upcoming*
  - Alternative local communication between charging stations and local nodes *upcoming*
  - EV Driver/EV Communication via WLAN + App (special user rolls required) *upcoming*
    - Charging Tickets: EMP signed anonymous authorization data for offline charging
    - EV Driver Networking: EV driver can deliver firmware or configuration updates to a charging station or fetch charging session data and deliver it to a charging station operator
- Efficient transport of binary data within OCPP websockets *upcoming*
  - Setup and tear-down of binary channels and topologies *upcoming*
  - Firmware updates *upcoming*
  - More efficient measurement data transport *upcoming*
  - RS232/RS485 transport *upcoming*
  - Modbus/TCP transport to allow a direct communication with e.g. smart energy meters located in the remote network of a charging location *upcoming*
- High availability for network configurations *upcoming*
  - Like DNS Service (SRV) Resource Records, https://de.wikipedia.org/wiki/SRV_Resource_Record
