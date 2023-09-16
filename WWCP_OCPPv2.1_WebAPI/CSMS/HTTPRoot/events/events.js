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
        eventsSource.addEventListener('AUTHSTARTRequest', function (event) {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v;
            try {
                const request = JSON.parse(event.data);
                const authentication = (_m = (_k = (_h = (_f = (_d = (_b = (_a = request.authentication) === null || _a === void 0 ? void 0 : _a.authToken) !== null && _b !== void 0 ? _b : (_c = request.authentication) === null || _c === void 0 ? void 0 : _c.QRCodeIdentification) !== null && _d !== void 0 ? _d : (_e = request.authentication) === null || _e === void 0 ? void 0 : _e.plugAndChargeIdentification) !== null && _f !== void 0 ? _f : (_g = request.authentication) === null || _g === void 0 ? void 0 : _g.remoteIdentification) !== null && _h !== void 0 ? _h : (_j = request.authentication) === null || _j === void 0 ? void 0 : _j.PIN) !== null && _k !== void 0 ? _k : (_l = request.authentication) === null || _l === void 0 ? void 0 : _l.publicKey) !== null && _m !== void 0 ? _m : null;
                const location = (_v = (_t = (_r = (_p = (_o = request.chargingLocation) === null || _o === void 0 ? void 0 : _o.EVSEId) !== null && _p !== void 0 ? _p : (_q = request.chargingLocation) === null || _q === void 0 ? void 0 : _q.chargingStationId) !== null && _r !== void 0 ? _r : (_s = request.chargingLocation) === null || _s === void 0 ? void 0 : _s.chargingPoolId) !== null && _t !== void 0 ? _t : (_u = request.chargingLocation) === null || _u === void 0 ? void 0 : _u.chargingStationOperatorId) !== null && _v !== void 0 ? _v : null;
                const operatorId = request.operatorId && (location === null || location === void 0 ? void 0 : location.indexOf(request.operatorId)) < 0 ? request.operatorId : null;
                CreateLogEntry(request.timestamp, request.roamingNetworkId, request.eventTrackingId, "AUTHSTART", "<div class=\"authentication\">" + authentication + "</div>" +
                    (location ? " at <div class=\"location\">" + location + "</div>" : "") +
                    (operatorId ? "    <div class=\"operatorId\">" + operatorId + "</div>" : "") +
                    "<span class=\"hidden\">(partnerSessionId " + request.partnerSessionId + ")</span>", request.EVSEId // ConnectionColorKey
                );
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
        eventsSource.addEventListener('AUTHSTARTResponse', function (event) {
            try {
                const response = JSON.parse(event.data);
                const result = response.result;
                AppendLogEntry(response.timestamp, response.roamingNetwork, 
                // 1) Search for a logline with this command
                "AUTHSTART", 
                // 2) Search for a logline with this pattern
                "\"eventTrackingId\">" + response.eventTrackingId, " &rArr; " + result.result +
                    (result.description ? " '" + result.description["eng"] + "'" : "") +
                    (result.providerId ? " by <div class=\"providerId\">" + result.providerId + "</div>" : "") +
                    (result.authorizatorId ? " via <div class=\"authorizatorId\">" + result.authorizatorId + "</div>" : "") +
                    (result.EMPRoamingProviderId ? " via <div class=\"EMPRoamingProviderId\">" + result.EMPRoamingProviderId + "</div>" : "") +
                    (result.CSORoamingProviderId ? " via <div class=\"CSORoamingProviderId\">" + result.CSORoamingProviderId + "</div>" : "") +
                    (result.sessionId ? " <a href=\"../RNs/" + response.roamingNetworkId + "/chargingSessions/" + result.sessionId + "\" class=\"sessionId\"><i class=\"fas fa-file-contract\"></i></a>" : "") +
                    " [" + response.runtime + " ms]");
            }
            catch (exception) {
                console.debug(exception);
            }
        }, false);
    }
}
//# sourceMappingURL=events.js.map