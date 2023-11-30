///<reference path="../../../../../../libs/UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartEventsSSE() {
    //MenuHighlight('Events');
    const connectionColors = {};
    const eventsDiv = document.getElementById('eventsDiv');
    const streamFilterInput = document.getElementById('eventsFilterDiv').getElementsByTagName('input')[0];
    streamFilterInput.onchange = () => {
        const AllLogLines = eventsDiv.getElementsByClassName('logLine');
        for (let i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].innerHTML.indexOf(streamFilterInput.value) > -1)
                AllLogLines[i].style.display = 'table-row';
            else
                AllLogLines[i].style.display = 'none';
        }
    };
    function GetConnectionColors(connectionId) {
        const colors = connectionColors[connectionId];
        if (colors !== undefined)
            return colors;
        else {
            const red = Math.floor(Math.random() * 80 + 165).toString(16);
            const green = Math.floor(Math.random() * 80 + 165).toString(16);
            const blue = Math.floor(Math.random() * 80 + 165).toString(16);
            const connectionColor = red + green + blue;
            connectionColors[connectionId] = new Object();
            connectionColors[connectionId].textcolor = "000000";
            connectionColors[connectionId].background = connectionColor;
            return connectionColors[connectionId];
        }
    }
    function CreateLogEntry(timestamp, roamingNetwork, eventTrackingId, command, message, connectionColorKey) {
        const connectionColor = GetConnectionColors(connectionColorKey);
        if (typeof message === 'string') {
            message = [message];
        }
        const div = document.createElement('div');
        div.className = "logLine";
        div.style.color = "#" + connectionColor.textcolor;
        div.style.background = "#" + connectionColor.background;
        div.innerHTML = "<div class=\"timestamp\">" + new Date(timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
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
        const AllLogLines = eventsDiv.getElementsByClassName('logLine');
        for (let i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].getElementsByClassName("command")[0].innerHTML == command) {
                if (AllLogLines[i].innerHTML.indexOf(searchPattern) > -1) {
                    AllLogLines[i].getElementsByClassName("message")[0].innerHTML += message;
                    break;
                }
            }
        }
    }
    const eventsSource = window.EventSource !== undefined
        ? new EventSource('/events')
        : null;
    if (eventsSource !== null) {
        eventsSource.onmessage = function (event) {
            console.debug(event);
        };
        eventsSource.onerror = function (event) {
            console.debug(event);
        };
        eventsSource.addEventListener('OnNewTCPConnection', function (event) {
            var _a, _b;
            try {
                const request = JSON.parse(event.data);
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
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, (_b = (_a = request.connection.customData) === null || _a === void 0 ? void 0 : _a.chargeBoxId) !== null && _b !== void 0 ? _b : "-", request.eventTrackingId, "OnNewWebSocketConnection", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTextMessageRequestReceived', function (event) {
            try {
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnTextMessageRequestReceived", request.message, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTextMessageResponseSent', function (event) {
            try {
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnTextMessageResponseSent", request.message, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTextMessageErrorSent', function (event) {
            try {
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnTextMessageErrorSent", JSON.stringify(request), request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnClosesMessageReceived', function (event) {
            try {
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnClosesMessageReceived", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnTCPConnectionClosed', function (event) {
            try {
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.connection.customData.chargeBoxId, request.eventTrackingId, "OnTCPConnectionClosed", request.connection.remoteSocket, request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnBootNotificationRequest', function (event) {
            try {
                const request = JSON.parse(event.data);
                CreateLogEntry(request.timestamp, request.chargeBoxId, request.eventTrackingId, "OnBootNotification", JSON.stringify(request.data), request.connection.remoteSocket // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('OnBootNotificationResponse', function (event) {
            try {
                const data = JSON.parse(event.data);
                const request = data.request;
                const response = data.response;
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