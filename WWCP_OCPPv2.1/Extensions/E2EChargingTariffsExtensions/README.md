# OCPP E2E Charging Tariffs Extensions

The *E2E Charging Tariffs Extensions* allow you to have machine readable and digital signed read-only charging tariffs.
It is a legal requirement in some countries to verify that a tariff is shown before, during and after the charging session
and to make sure, that the tariff was not manipulated during that time. By this the EV driver can be sure that (s)he is charged the correct price.
This is especially important for *dynamic tariffs* or the famous *German Calibration Law* for secure adhoc charging at public charging stations.

Charging Tariffs might be part of OCPP v2.1 Draft 3++, but the current approach of the Open Charge Alliance does not adopt end-to-end use cases, it stayes an OCPP-internal solution. Therefore we have created this experimental extension.

## Differences to OCPI and OCPP

The charging tariff data structure is based on OCPI v2.2.1 and OCPP v2.1 draft 2 v0.44, but has many advantages over it:

1. It is digitally signed, so that the EV driver can be sure that the tariff was not manipulated.
2. The charging tariff has a **well-defined read-only globally unique** and **unique over time** **identification**, so that it can be referenced by other data structures like *charging tickets* easily.
3. Added a **created** timestamp to track the creation timestamp of a charging tariff.
4. The charging tariff has a **"providerId"** to identify the provider of the tariff instead of the clunky *"country code"* and *"party id"* of OCPI.
5. The charging tariff has a **"providerName"** for a user-friendly name of the provider.
6. The charging tariff has a **"replaces"** property to indicate, that this tariff replaces another tariff, which might still be valid for some time.
7. The charging tariff has a **"references"** property to indicate, that this tariff information is based on another tariff, e.g. because some local adaption of the tariff was required.
8. Removed the OCPI **"last updated"** property, as charging tickets are immutable by definition.
9. Renamed the OCPI property **"Start"** to a _mandatory_ **"NotBefore"**, to be more aligned with certificates.
10. Renamed the OCPI property **"End"** to **NotAfter**, to be more aligned with certificates.
11. Renamed the OCPI property **"TariffAltText"** to **"Description"** and its data type from **IEnumerable&lt;DisplayText&gt;** to **DisplayTexts** to simplify its usage.
12. Renamed the OCPI property **"TariffAltURL"** to **"URL"**
13. Added the optional property **"EVSEIds"** as a list of EVSEs, this charging tariff is valid for.
14. Added the optional property **"NetworkPaths"** as a list of charging stations (including all EVSEs), this charging tariff is valid for.
15. Added the optional property **"ChargingPoolIds"** as a list of charging pools (including all charging stations and EVSEs), this charging tariff is valid for.


### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
