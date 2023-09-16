///<reference path="../../../../../../libs/UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartEventsSSE() {

    //MenuHighlight('Events');

    const connectionColors   = {};
    const eventsDiv          = document.getElementById('eventsDiv');
    const streamFilterInput  = document.getElementById('eventsFilterDiv').getElementsByTagName('input')[0] as HTMLInputElement;
    streamFilterInput.onchange = () => {

        const AllLogLines = eventsDiv.getElementsByClassName('logLine') as HTMLCollectionOf<HTMLDivElement>;

        for (let i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].innerHTML.indexOf(streamFilterInput.value) > -1)
                AllLogLines[i].style.display = 'table-row';
            else
                AllLogLines[i].style.display = 'none';
        }

    }

    function GetConnectionColors(connectionId) {

        const colors = connectionColors[connectionId];

        if (colors !== undefined)
            return colors;

        else
        {

            const red   = Math.floor(Math.random() * 80 + 165).toString(16);
            const green = Math.floor(Math.random() * 80 + 165).toString(16);
            const blue  = Math.floor(Math.random() * 80 + 165).toString(16);

            const connectionColor = red + green + blue;

            connectionColors[connectionId]             = new Object();
            connectionColors[connectionId].textcolor   = "000000";
            connectionColors[connectionId].background  = connectionColor;

            return connectionColors[connectionId];

        }

    }

    function CreateLogEntry(timestamp, roamingNetwork, eventTrackingId, command, message, connectionColorKey) {

        const connectionColor = GetConnectionColors(connectionColorKey);

        if (typeof message === 'string') {
            message = [message];
        }

        const div = document.createElement('div');
        div.className         = "logLine";
        div.style.color       = "#" + connectionColor.textcolor;
        div.style.background  = "#" + connectionColor.background;
        div.innerHTML         = "<div class=\"timestamp\">"       + new Date(timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
                                "<div class=\"roamingNetwork\">"  + roamingNetwork  + "</div>" +
                                "<div class=\"eventTrackingId\">" + eventTrackingId + "</div>" +
                                "<div class=\"command\">"         + command         + "</div>" +
                                "<div class=\"message\">"         + message.reduce(function (a, b) { return a + "<br />" + b; }); + "</div>";

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

    if (eventsSource !== null)
    {

        eventsSource.onmessage = function (event) {
            console.debug(event);
        };

        eventsSource.onerror = function (event) {
            console.debug(event);
        };



        eventsSource.addEventListener('AUTHSTARTRequest',                function (event) {

            try
            {

                const request         = JSON.parse((event as MessageEvent).data);

                const authentication  = request.authentication?.authToken                   ??
                                        request.authentication?.QRCodeIdentification        ??
                                        request.authentication?.plugAndChargeIdentification ??
                                        request.authentication?.remoteIdentification        ??
                                        request.authentication?.PIN                         ??
                                        request.authentication?.publicKey                   ??
                                        null;

                const location        = request.chargingLocation?.EVSEId                    ??
                                        request.chargingLocation?.chargingStationId         ??
                                        request.chargingLocation?.chargingPoolId            ??
                                        request.chargingLocation?.chargingStationOperatorId ??
                                        null;
                const operatorId      = request.operatorId && location?.indexOf(request.operatorId) < 0 ? request.operatorId : null;

                CreateLogEntry(request.timestamp,
                               request.roamingNetworkId,
                               request.eventTrackingId,
                               "AUTHSTART",
                               "<div class=\"authentication\">" + authentication + "</div>" +
                               (location   ? " at <div class=\"location\">"   + location   + "</div>" : "") +
                               (operatorId ? "    <div class=\"operatorId\">" + operatorId + "</div>" : "") +
                               "<span class=\"hidden\">(partnerSessionId " + request.partnerSessionId + ")</span>",
                               request.EVSEId // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('AUTHSTARTResponse',               function (event) {

            try
            {

                const response = JSON.parse((event as MessageEvent).data);
                const result   = response.result;

                AppendLogEntry(response.timestamp,
                               response.roamingNetwork,
                               // 1) Search for a logline with this command
                               "AUTHSTART",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + response.eventTrackingId,

                               " &rArr; " + result.result +
                               (result.description          ? " '"                                        + result.description["eng"]   + "'"      : "") +
                               (result.providerId           ? " by <div class=\"providerId\">"            + result.providerId           + "</div>" : "") +
                               (result.authorizatorId       ? " via <div class=\"authorizatorId\">"       + result.authorizatorId       + "</div>" : "") +
                               (result.EMPRoamingProviderId ? " via <div class=\"EMPRoamingProviderId\">" + result.EMPRoamingProviderId + "</div>" : "") +
                               (result.CSORoamingProviderId ? " via <div class=\"CSORoamingProviderId\">" + result.CSORoamingProviderId + "</div>" : "") +
                               (result.sessionId            ? " <a href=\"../RNs/" + response.roamingNetworkId + "/chargingSessions/" + result.sessionId + "\" class=\"sessionId\"><i class=\"fas fa-file-contract\"></i></a>" : "") +
                                " [" + response.runtime + " ms]");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);



    }

}

