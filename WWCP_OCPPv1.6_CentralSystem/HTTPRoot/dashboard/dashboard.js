///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartDashboard(ZoomToBoundingBox) {
    HTTPGet("/tokens", (status, response) => {
        try {
            let list = JSON.parse(response);
        }
        catch (exception) { }
    }, (status, statusText, response) => {
    });
    //var refresh = setTimeout(StartDashboard, 30000);
}
//# sourceMappingURL=dashboard.js.map