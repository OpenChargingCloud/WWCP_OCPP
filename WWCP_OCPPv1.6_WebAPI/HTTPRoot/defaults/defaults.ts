
//var HTTPCookieId: string = "OCPIWebAPI";
//var mapboxgl = null;

interface IOCPIResponse {
    data:                   any;
    statusCode:             number;
    statusMessage:          string;
}

interface ISearchResult<T> {
    totalCount:             number;
    filteredCount:          number;
    searchResults:          Array<T>;
}

interface IVersion {
    version:                string;
    url:                    string;
}

interface IVersionDetails {
    version:                string;
    endpoints:              Array<IEndpoints>;
}

interface IEndpoints {
    identifier:             string;
    role:                   string;
    url:                    string;
}

interface IRemoteParty {
    "@id":                  string;
    "@context":             string;
    countryCode:            string;
    partyId:                string;
    role:                   string;
    businessDetails:        IBusinessDetails;
    partyStatus:            string;
    accessInfos:            Array<IAccessInfo>;
    remoteAccessInfos:      Array<IRemoteAccessInfo>;
    last_updated:           string;
}

interface IBusinessDetails {
    name:                   string;
    website:                string;
    logo:                   string;
}

interface IAccessInfo {
    token:                  string;
    status:                 string;
}

interface IRemoteAccessInfo {
    token:                  string;
    versionsURL:            string;
    versionIds:             Array<string>;
    selectedVersionId:      string;
    status:                 string;
}


function EncodeToken(str) {

    var buf = [];

    for (let i = str.length - 1; i >= 0; i--) {
        buf.unshift(['&#', str[i].charCodeAt(), ';'].join(''));
    }

    return buf.join('');
}


// #region OCPIGet(RessourceURI, AccessToken, OnSuccess, OnError)

function OCPIGet(RessourceURI: string,
                 OnSuccess,
                 OnError) {

    const ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept",   "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");

    if (localStorage.getItem("OCPIAccessToken") != null)
        ajax.setRequestHeader("Authorization", "Token " + btoa(localStorage.getItem("OCPIAccessToken")));

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    ajax.send();

}

// #endregion

