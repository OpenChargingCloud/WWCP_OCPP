///<reference path="../../../../../../../libs/UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartEventsSSE() {
    //MenuHighlight('Events');
    var connectionColors = {};
    var eventsDiv = document.getElementById('eventsDiv');
    var streamFilterInput = document.getElementById('eventsFilterDiv').getElementsByTagName('input')[0];
    streamFilterInput.onchange = function () {
        var AllLogLines = eventsDiv.getElementsByClassName('logLine');
        for (var i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].innerHTML.indexOf(streamFilterInput.value) > -1)
                AllLogLines[i].style.display = 'table-row';
            else
                AllLogLines[i].style.display = 'none';
        }
    };
    function GetConnectionColors(connectionId) {
        var colors = connectionColors[connectionId];
        if (colors !== undefined)
            return colors;
        else {
            var red = Math.floor(Math.random() * 80 + 165).toString(16);
            var green = Math.floor(Math.random() * 80 + 165).toString(16);
            var blue = Math.floor(Math.random() * 80 + 165).toString(16);
            var connectionColor = red + green + blue;
            connectionColors[connectionId] = new Object();
            connectionColors[connectionId].textcolor = "000000";
            connectionColors[connectionId].background = connectionColor;
            return connectionColors[connectionId];
        }
    }
    function CreateLogEntry(timestamp, roamingNetwork, eventTrackingId, command, message, connectionColorKey) {
        var connectionColor = GetConnectionColors(connectionColorKey);
        if (typeof message === 'string') {
            message = [message];
        }
        var div = document.createElement('div');
        div.className = "logLine";
        div.style.color = "#" + connectionColor.textcolor;
        div.style.background = "#" + connectionColor.background;
        div.innerHTML = "<div class=\"timestamp\">" + timestamp + "</div>" + //  new Date(timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
            "<div class=\"roamingNetwork\">" + roamingNetwork + "</div>" +
            "<div class=\"eventTrackingId\">" + eventTrackingId + "</div>" +
            "<div class=\"command\">" + command + "</div>" +
            "<div class=\"message\">" + message.reduce(function (a, b) { return a + "<br />" + b; });
        +"</div>";
        if (div.innerHTML.indexOf(streamFilterInput.value) > -1)
            div.style.display = 'table-row';
        else
            div.style.display = 'none';
        eventsDiv.insertBefore(div, eventsDiv.firstChild);
    }
    function AppendLogEntry(timestamp, roamingNetwork, command, searchPattern, message) {
        var AllLogLines = eventsDiv.getElementsByClassName('logLine');
        for (var i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].getElementsByClassName("command")[0].innerHTML == command) {
                if (AllLogLines[i].innerHTML.indexOf(searchPattern) > -1) {
                    AllLogLines[i].getElementsByClassName("message")[0].innerHTML += message;
                    break;
                }
            }
        }
    }
    var eventsSource = window.EventSource !== undefined
        ? new EventSource('events')
        : null;
    if (eventsSource !== null) {
        eventsSource.onmessage = function (event) {
            console.debug(event);
        };
        eventsSource.onerror = function (event) {
            console.debug(event);
        };
        // {"timestamp":"2024-02-26T21:53:54.019Z","connection":{"localSocket":"127.0.0.1:9920","remoteSocket":"127.0.0.1:64675","customData":{"networkingNodeId":"GD001"}},"message":[2,"100000","BootNotification",{"chargingStation":{"model":"mm","vendorName":"vv"},"reason":"ApplicationReset"}]}
        eventsSource.addEventListener('OnNewTCPConnection', function (event) {
            var _a, _b;
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, (_b = (_a = request.connection.customData) === null || _a === void 0 ? void 0 : _a.chargeBoxId) !== null && _b !== void 0 ? _b : "-", request.eventTrackingId, "OnNewTCPConnection", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNewWebSocketConnection', function (event) {
            var _a, _b;
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, (_b = (_a = request.connection.customData) === null || _a === void 0 ? void 0 : _a.chargeBoxId) !== null && _b !== void 0 ? _b : "-", request.eventTrackingId, "OnNewWebSocketConnection", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnJSONMessageRequestReceived2', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnJSONMessageRequestReceived", request.message, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnJSONMessageResponseSent2', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnJSONMessageResponseSent", request.message, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnJSONErrorResponseSent', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnJSONErrorResponseSent", JSON.stringify(request), request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnClosesMessageReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnClosesMessageReceived", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTCPConnectionClosed', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnTCPConnectionClosed", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        // Certificates
        eventsSource.addEventListener('OnGet15118EVCertificateRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnGet15118EVCertificate", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnGet15118EVCertificateResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnGet15118EVCertificate", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnGetCertificateStatusRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnGetCertificateStatus", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnGetCertificateStatusResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnGetCertificateStatus", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnGetCRLRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnGetCRL", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnGetCRLResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnGetCRL", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnSignCertificateRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnSignCertificate", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnSignCertificateResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnSignCertificate", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        // Charging
        eventsSource.addEventListener('OnAuthorizeRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnAuthorize", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnAuthorizeResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnAuthorize", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnClearedChargingLimitRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnClearedChargingLimit", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnClearedChargingLimitResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnClearedChargingLimit", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnMeterValuesRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnMeterValues", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnMeterValuesResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnMeterValues", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyChargingLimitRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyChargingLimit", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyChargingLimitResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyChargingLimit", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyEVChargingNeedsRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyEVChargingNeeds", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyEVChargingNeedsResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyEVChargingNeeds", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyEVChargingScheduleRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyEVChargingSchedule", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyEVChargingScheduleResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyEVChargingSchedule", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyPriorityChargingRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyPriorityCharging", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyPriorityChargingResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyPriorityCharging", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifySettlementRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifySettlement", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifySettlementResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifySettlement", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnPullDynamicScheduleUpdateRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnPullDynamicScheduleUpdate", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnPullDynamicScheduleUpdateResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnPullDynamicScheduleUpdate", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnReportChargingProfilesRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnReportChargingProfiles", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnReportChargingProfilesResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnReportChargingProfiles", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnReservationStatusUpdateRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnReservationStatusUpdate", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnReservationStatusUpdateResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnReservationStatusUpdate", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnStatusNotificationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnStatusNotification", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnStatusNotificationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnStatusNotification", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTransactionEventRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnTransactionEvent", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTransactionEventResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnTransactionEvent", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        // Customer
        eventsSource.addEventListener('OnNotifyCustomerInformationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyCustomerInformation", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyCustomerInformationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyCustomerInformation", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyDisplayMessagesRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyDisplayMessages", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyDisplayMessagesResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyDisplayMessages", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        // Device Model
        eventsSource.addEventListener('OnLogStatusNotificationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnLogStatusNotification", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnLogStatusNotificationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnLogStatusNotification", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyEventRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyEvent", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyEventResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyEvent", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyMonitoringReportRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyMonitoringReport", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyMonitoringReportResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyMonitoringReport", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyReportRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnNotifyReport", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnNotifyReportResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnNotifyReport", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnSecurityEventNotificationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnSecurityEventNotification", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnSecurityEventNotificationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnSecurityEventNotification", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        // Firmware
        eventsSource.addEventListener('OnBootNotificationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnBootNotification", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnBootNotificationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnBootNotification", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnFirmwareStatusNotificationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnFirmwareStatusNotification", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnFirmwareStatusNotificationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnFirmwareStatusNotification", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnHeartbeatRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnHeartbeat", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnHeartbeatResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnHeartbeat", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnPublishFirmwareStatusNotificationRequestReceived', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.destinationNodeId, request.eventTrackingId, "OnPublishFirmwareStatusNotification", JSON.stringify(request.data), request.networkPath[0] // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnPublishFirmwareStatusNotificationResponseSent', function (event) {
            try {
                var data = JSON.parse(event.data);
                var request = data.request;
                var response = data.response;
                AppendLogEntry(response.timestamp, data.chargeBoxId, 
                // 1) Search for a logline with this command
                "OnPublishFirmwareStatusNotification", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + data.eventTrackingId, " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
    }
}
//# sourceMappingURL=events.js.map