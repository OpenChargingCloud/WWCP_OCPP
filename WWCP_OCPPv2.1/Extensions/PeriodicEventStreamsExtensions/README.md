# OCPP Periodic Event Streams Extensions

The *OCPP Periodic Event Streams Extensions* allow you to send variable monitoring data more efficiently
as a stream of data.

## Steps

1. The CSMS creates a new variable monitoring by sending a `SetVariableMonitoringRequest` to the charging station.
2. The charging station response with a `SetVariableMonitoringResponse` that contains the `monitoringId` that is used to identify the monitoring stream.
3. The charging station sends a OpenPeriodicEventStreamRequest to CSMS with the constant data for each variable of the stream.
4. The CSMS replies.
5. Every maxTime seconds or when maxItems values are available charging station will send all periodic values of
   a monitor as one message via the NotifyPeriodicEventStream message.




## Further ideas

- ClosePeriodicEventStreamRequest should have e.g. an optional parameter to specify whether the stream
  should be close NOW, meaning that I do not want any more data to be sent out and even the queue should
  be cleared, OR whether I want the stream to stop accepting NEW data, thus all data within its queue is
  expected to be sent before shutting down. When the queue still has elements and they should still be
  sent towards the CSMS, there should an event notification sent to the CSMS when the queue finally
  became empty indicating, that the last stream event element had been sent and the stream is now closed.
  This helps the CSMS to understand the situation better.
- Alternative: The StreamDataElementType is extended by some well defined Meta data that indicate, that
  e.g. this is the last element of the stream.
- Perhaps the CSMS wants to pause one or multiple streams in case of debugging or over load situations.
  A bit later all streams are resumed. Intermediate elements are queued, but currently not send out.
  The CSMS might want to clear a stream queue because in error situation it was filled up with too many
  elements.



### Your participation

This software is Open Source under the Apache 2.0 license. We appreciate
your participation in this ongoing project, and your help to improve it
and the e-mobility ICT in general. If you find bugs, want to request a
feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
