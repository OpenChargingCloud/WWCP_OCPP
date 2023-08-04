# OCPP - Computer Science Edition

The following list of proposed updates, suggestions, and modifications are intended for upcoming iterations of the *Open Charge Point Protocol*.
At present, the *Open Charge Alliance* exercises a cautious approach towards introducing refinement and enhancements, as well as managing
backward and forward compatibility. This occasionally inhibits or delays key advancements in the fast-paced field of electric mobility.
This structured list of proposals seeks to address these issues in a way that everyone, can appreciate and contribute to.

- JSON-LD context information for JSON data structures *upcoming*
- Digital signatures for every OCPP command/response, allowing multi-signatures *upcoming*
- OCPP user role model to overcome the current "everything is done by the root user" approach of OCPP. Users are identified by the digital signature of their commands/responses *upcoming*
- High availability for network configurations *upcoming*
  - Like DNS Service (SRV) Resource Records, https://de.wikipedia.org/wiki/SRV_Resource_Record
- Steuerbaren Verbrauchseinrichtungen nach 14a EnWG. *upcoming*
  - Netzbetreiber Warnung an Transparenzplattform
  - Transparenzplattform > Ladestationsbetreiber
  - Bestätigung der Ladestationen an Transparenzplattform
- Efficient transport of binary data within OCPP websockets *upcoming*
  - Firmware updates *upcoming*
  - More efficient measurement data transport *upcoming*
  - RS232/RS485 transport *upcoming*
  - Modbus/TCP transport to allow a direct communication with e.g. smart energy meters located in the remote network of a charging location *upcoming*
- OCPP Overlay Networking *upcoming*
  - OCPP Local Node *upcoming*
- OCPP Security Gateway *upcoming*
- OCPP HTTP/WebSocket Server on each charging station *upcoming*
  - Alternative local communication between charging stations and local nodes *upcoming*
  - EV Driver/EV Communication via WLAN + App (special user rolls required) *upcoming*
