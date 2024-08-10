# OCPP Binary Streams Extensions

The *Binary Streams Extensions* are enhancements that enable the use of binary data transfers
over the normal OCPP HTTP Web Sockets connection. This technology is particularly beneficial
in scenarios where there is a need to send continuous or periodic data streams. These streams
can include various types of data, such as periodic event notifications, firmware updates, or
logs and real-time debugging information.

By employing these extensions, you can streamline these data transfers and avoid separate
HTTP(S) or (S)FTP connections, which introduce undesirable complexity and potential severe
security vulnerabilities into your infrastructure. Reducing the number of used protocols,
open ports and firewall exceptions that need to be managed, monitored and secured the attack
surface of your charging infrastructure is reduced dramatically.





### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
