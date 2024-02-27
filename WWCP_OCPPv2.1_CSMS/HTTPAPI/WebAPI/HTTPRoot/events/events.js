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
        eventsSource.addEventListener('OnTextMessageErrorSent', function (event) {
            try {
                var request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnTextMessageErrorSent", JSON.stringify(request), request.connection.remoteSocket // ConnectionColorKey
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
    }
}
//# sourceMappingURL=events.js.map