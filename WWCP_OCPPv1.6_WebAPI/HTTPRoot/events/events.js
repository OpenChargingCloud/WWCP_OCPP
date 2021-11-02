///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartOCPPEventMessages() {
    var ConnectionColors = {};
    var StreamFilterPattern = document.getElementById('patternFilterInput');
    StreamFilterPattern.onchange = function () {
        var AllLogLines = document.getElementById('EventsDiv').getElementsByClassName('LogLine');
        for (var i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].innerHTML.indexOf(StreamFilterPattern.value) > -1)
                AllLogLines[i].style.display = 'table-row';
            else
                AllLogLines[i].style.display = 'none';
        }
    };
    function GetConnectionColors(ConnectionId) {
        var Colors = ConnectionColors[ConnectionId];
        if (Colors != undefined)
            return Colors;
        else {
            var red = Math.floor(Math.random() * 80 + 165).toString(16);
            var green = Math.floor(Math.random() * 80 + 165).toString(16);
            var blue = Math.floor(Math.random() * 80 + 165).toString(16);
            var ConnectionColor = red + green + blue;
            ConnectionColors[ConnectionId] = new Object();
            ConnectionColors[ConnectionId].textcolor = "000000";
            ConnectionColors[ConnectionId].background = ConnectionColor;
            return ConnectionColors[ConnectionId];
        }
    }
    function CreateLogEntry(Timestamp, Sender, Category, EventTrackingId, Request, ConnectionColorKey) {
        const connectionColor = GetConnectionColors(ConnectionColorKey);
        const request = Object.keys(Request).length !== 0 ? JSON.stringify(Request, null, 2) : "";
        var div = document.createElement('div');
        div.className = "LogLine";
        div.style.color = "#" + connectionColor.textcolor;
        div.style.background = "#" + connectionColor.background;
        div.innerHTML = "<div class=\"timestamp\">" + new Date(Timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
            "<div class=\"eventTrackingId\">" + EventTrackingId + "</div>" +
            "<div class=\"sender\">" + Sender + "</div>" +
            "<div class=\"category\">" + Category + "</div>" +
            "<div class=\"request\">" + request + "</div>" +
            "<div class=\"response\"></div>";
        if (div.innerHTML.indexOf(StreamFilterPattern.value) > -1)
            div.style.display = 'table-row';
        else
            div.style.display = 'none';
        document.getElementById('EventsDiv').insertBefore(div, document.getElementById('EventsDiv').firstChild);
    }
    function AppendLogEntry(Timestamp, Sender, EventTrackingId, Response, Runtime) {
        // &rArr;
        //const allLogLines  = ;
        for (const allLogLine of document.getElementById('EventsDiv').getElementsByClassName('LogLine')) {
            if (allLogLine.getElementsByClassName("eventTrackingId")[0].innerHTML == EventTrackingId) {
                allLogLine.getElementsByClassName("response")[0].innerHTML = JSON.stringify(Response, null, 2);
                break;
            }
        }
    }
    var LogEventsSource = null;
    if (window.EventSource != undefined) {
        LogEventsSource = new EventSource('events?take=1000'); // /inspect/daily/sse');
        //document.getElementById('EventSourceType').firstChild.innerHTML = 'Browser EventSource';
    } // else {
    // LogEventsSource = new EventSource2('/LogEvents');
    //document.getElementById('EventSourceType').firstChild.innerHTML = 'JavaScript EventSource';
    //}
    LogEventsSource.onmessage = function (event) {
        var LastEventId = event.lastEventId;
        var div = document.createElement('div');
        div.innerHTML = "Message: " + event.data + "<br>";
        document.getElementById('EventsDiv').insertBefore(div, document.getElementById('EventsDiv').firstChild);
    };
    LogEventsSource.addEventListener('error', function (event) {
        //if (event.readyState == EventSource.CLOSED) {
        //    // Connection was closed.
        //}
        if (event.data != undefined) {
            var div = document.createElement('div');
            div.innerHTML = "Error: " + event.data + "<br>";
            document.getElementById('EventsDiv').insertBefore(div, document.getElementById('EventsDiv').firstChild);
        }
    }, false);
    LogEventsSource.addEventListener('OnStarted', function (event) {
        try {
            var data = JSON.parse(event.data);
            var Timestamp = new Date(data.Timestamp).format('dd.mm.yyyy HH:MM:ss');
            var Message = data.Message;
            var LastEventId = event.lastEventId;
            var div = document.createElement('div');
            div.className = "LogLine";
            div.innerHTML = "<span class=\"Timestamp\">" + Timestamp + "</span>" +
                "<span class=\"OnStarted\">&nbsp;</span>" +
                "<span class=\"OnStartedMessage\">" + Message + "</span>";
            if (div.innerHTML.indexOf(StreamFilterPattern.value) > -1)
                div.style.display = 'block';
            else
                div.style.display = 'none';
            document.getElementById('EventsDiv').insertBefore(div, document.getElementById('EventsDiv').firstChild);
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('InvalidJSONRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            var Timestamp = new Date(data.Timestamp).format('dd.mm.yyyy HH:MM:ss');
            var RemoteSocket = data.RemoteSocket;
            var RoamingNetwork = data.RoamingNetwork;
            var EVSEId = data.EVSEId;
            var LastEventId = event.lastEventId;
            var ConnectionColor = GetConnectionColors(RemoteSocket);
            var div = document.createElement('div');
            div.className = "LogLine";
            div.style.color = "#" + ConnectionColor.textcolor;
            div.style.background = "#" + ConnectionColor.background;
            div.innerHTML = "<span class=\"Timestamp\">" + Timestamp + "</span>" +
                "<span class=\"OnNewConnection\">" + RemoteSocket + "</span>" +
                "<span class=\"OnNewConnectionMessage\">Invalid JSONRequest</span>";
            if (div.innerHTML.indexOf(StreamFilterPattern.value) > -1)
                div.style.display = 'block';
            else
                div.style.display = 'none';
            document.getElementById('EventsDiv').insertBefore(div, document.getElementById('EventsDiv').firstChild);
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnHTTPRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.remoteIPAddress, "daily", data.eventTrackingId, data.body, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnHTTPResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.remoteIPAddress, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnError', function (event) {
        try {
            var data = JSON.parse(event.data);
            var Timestamp = new Date(data.Timestamp).format('dd.mm.yyyy HH:MM:ss');
            var ConnectionId = data.ConnectionId;
            var Error = data.Error;
            var CurrentBuffer = data.CurrentBuffer;
            var LastEventId = event.lastEventId;
            var div = document.createElement('div');
            div.className = "LogLine";
            div.style.color = "#" + ConnectionColors[ConnectionId].textcolor;
            div.style.background = "#" + ConnectionColors[ConnectionId].background;
            div.innerHTML = "<span class=\"Timestamp\">" + Timestamp + "</span>" +
                "<span class=\"OnError\">" + ConnectionId + "</span>" +
                "<span class=\"OnErrorMessage\">" + Error + "<br>Current buffer: " + CurrentBuffer + "</span>";
            if (div.innerHTML.indexOf(StreamFilterPattern.value) > -1)
                div.style.display = 'block';
            else
                div.style.display = 'none';
            document.getElementById('EventsDiv').insertBefore(div, document.getElementById('EventsDiv').firstChild);
        }
        catch (ex) {
        }
    }, false);
    // OnBootNotificationRequest/-Response
    LogEventsSource.addEventListener('OnBootNotificationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "BootNotification", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnBootNotificationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnHeartbeatRequest/-Response
    LogEventsSource.addEventListener('OnHeartbeatRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "Heartbeat", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnHeartbeatResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnAuthorizeRequest/-Response
    LogEventsSource.addEventListener('OnAuthorizeRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "Authorize", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnAuthorizeResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnStartTransactionRequest/-Response
    LogEventsSource.addEventListener('OnStartTransactionRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "StartTransaction", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnStartTransactionResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnStatusNotificationRequest/-Response
    LogEventsSource.addEventListener('OnStatusNotificationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "SendStatusNotification", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnStatusNotificationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnMeterValuesRequest/-Response
    LogEventsSource.addEventListener('OnMeterValuesRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "SendMeterValues", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnMeterValuesResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnStopTransactionRequest/-Response
    LogEventsSource.addEventListener('OnStopTransactionRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "StopTransaction", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnStopTransactionResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnIncomingDataTransferRequest/-Response
    LogEventsSource.addEventListener('OnIncomingDataTransferRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "IncomingDataTransfer", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnIncomingDataTransferResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnDiagnosticsStatusNotificationRequest/-Response
    LogEventsSource.addEventListener('OnDiagnosticsStatusNotificationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "SendDiagnosticsStatusNotification", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnDiagnosticsStatusNotificationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnFirmwareStatusNotificationRequest/-Response
    LogEventsSource.addEventListener('OnFirmwareStatusNotificationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, data.chargeBoxId, "SendFirmwareStatusNotification", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnFirmwareStatusNotificationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnResetRequest/-Response
    LogEventsSource.addEventListener('OnResetRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "Reset", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnResetResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnChangeAvailabilityRequest/-Response
    LogEventsSource.addEventListener('OnChangeAvailabilityRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "ChangeAvailability", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnChangeAvailabilityResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnGetConfigurationRequest/-Response
    LogEventsSource.addEventListener('OnGetConfigurationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "GetConfiguration", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnGetConfigurationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnChangeConfigurationRequest/-Response
    LogEventsSource.addEventListener('OnChangeConfigurationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "ChangeConfiguration", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnChangeConfigurationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnDataTransferRequest/-Response
    LogEventsSource.addEventListener('OnDataTransferRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "DataTransfer", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnDataTransferResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnGetDiagnosticsRequest/-Response
    LogEventsSource.addEventListener('OnGetDiagnosticsRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "GetDiagnostics", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnGetDiagnosticsResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnTriggerMessageRequest/-Response
    LogEventsSource.addEventListener('OnTriggerMessageRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "TriggerMessage", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnTriggerMessageResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnUpdateFirmwareRequest/-Response
    LogEventsSource.addEventListener('OnUpdateFirmwareRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "UpdateFirmware", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnUpdateFirmwareResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnReserveNowRequest/-Response
    LogEventsSource.addEventListener('OnReserveNowRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "ReserveNow", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnReserveNowResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnCancelReservationRequest/-Response
    LogEventsSource.addEventListener('OnCancelReservationRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "CancelReservation", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnCancelReservationResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnRemoteStartTransactionRequest/-Response
    LogEventsSource.addEventListener('OnRemoteStartTransactionRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "RemoteStartTransaction", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnRemoteStartTransactionResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnRemoteStopTransactionRequest/-Response
    LogEventsSource.addEventListener('OnRemoteStopTransactionRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "RemoteStopTransaction", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnRemoteStopTransactionResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnSetChargingProfileRequest/-Response
    LogEventsSource.addEventListener('OnSetChargingProfileRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "SetChargingProfile", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnSetChargingProfileResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnClearChargingProfileRequest/-Response
    LogEventsSource.addEventListener('OnClearChargingProfileRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "ClearChargingProfile", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnClearChargingProfileResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnGetCompositeScheduleRequest/-Response
    LogEventsSource.addEventListener('OnGetCompositeScheduleRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "GetCompositeSchedule", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnGetCompositeScheduleResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnUnlockConnectorRequest/-Response
    LogEventsSource.addEventListener('OnUnlockConnectorRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "UnlockConnector", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnUnlockConnectorResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnGetLocalListVersionRequest/-Response
    LogEventsSource.addEventListener('OnGetLocalListVersionRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "GetLocalListVersion", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnGetLocalListVersionResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnSendLocalListRequest/-Response
    LogEventsSource.addEventListener('OnSendLocalListRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "SendLocalList", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnSendLocalListResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
    // OnClearCacheRequest/-Response
    LogEventsSource.addEventListener('OnClearCacheRequest', function (event) {
        try {
            var data = JSON.parse(event.data);
            CreateLogEntry(data.timestamp, "&rArr; " + data.chargeBoxId, "ClearCache", data.eventTrackingId, data.request, data.remoteIPAddress // ConnectionColorKey
            );
        }
        catch (ex) {
        }
    }, false);
    LogEventsSource.addEventListener('OnClearCacheResponse', function (event) {
        try {
            var data = JSON.parse(event.data);
            AppendLogEntry(data.timestamp, data.chargeBoxId, data.eventTrackingId, data.response, data.runtime);
        }
        catch (ex) {
        }
    }, false);
}
//# sourceMappingURL=events.js.map