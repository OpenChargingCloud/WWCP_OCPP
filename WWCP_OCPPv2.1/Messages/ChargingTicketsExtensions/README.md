# OCPP Charging Tickets Extensions

The *Charging Tickets Extensions* allow you to defined a *ticket* with which the EV driver is able
to authenticate and authorize at a charging station and immediately start a charging session - even
when this station is offline.

This might sound much like ISO 15118, but has many advantages over it:

1. First of all, it is much simpler, as it is just a digitally signed JSON object.
2. It is not limited to cable based communication beteen the car and the charging station. It can also be sent by a smart phone or smart watch via WLAN, Bluetooth or NFC.
3. ISO 15118 is just *authentication*. All it can give you is an *ev contract identification (EVCOId or eMAId)*. Afterwards you still need to do classical *authorization* via EV roaming to check if the EV driver is still allowed to charge at this charging station.
4. ISO 15118 contract certificates have a very long life-time by design. Revoking a certificate or an entire contract is a very expensive process, mostly done via *certification revocation list* and the already mentioned additional authorization step via EV roaming. This introduces a long delay until a charging session really starts. In contrast to this *charging tickets* are very short lifed certificates of just days or even hours. Therefore the need for revocation lists is minimal.
5. *Charging tickets* can store additional data like *charging tariffs* and a list of charging pools, -stations and EVSEs this ticket is valid (or invalid) for.
6. *Charging tickets* are anonymous. They do not contain any personal data of the EV driver like his/her *ev contract identification*. This is a big advantage over ISO 15118, as it allows you to use *charging tickets* for *ad hoc charging* at public charging stations without being tracked.


### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
