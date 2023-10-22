# OCPP End-2-End Security Extensions

The *OCPP End-2-End Security Extensions* allow you to use digital signatures on every
OCPP request and response. This allows you to verify the origin of a message and to
verify that the message has not been tampered with.

Some data structures like *Charging Tariffs* (see: Charging Tariffs Extensions) can also
be digitally signed. It is a legal requirement in some countries to verify that a tariff
was not tampered with, so that the customer can be sure that he is charged the correct
amount. This is especially important for *dynamic tariffs* or the famous *German Calibration Law*
for adhoc charging at public charging stations.

## Signature Policies

The signing and verification of OCPP messages is defined by a so called *Signature Policy*.
This policies defines which OCPP message are signed and which private key is used for the signing,
and which requirements the signature must fulfill when it is verified.


## User Roles

The *Signature Policies* can also be used to define user roles. By this we can separate the concerns
around mangaging charging stations. While an *admin role* can manage all configuration settings of a charging
station and perform software updates, a *service role* might only read log files or start and stop charging
sessions in case of an emergency. The *service role* might also be used by an E-Mobility Provider or
EV Roaming Operator to perform its tasks around reservations and remote starts.


### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
