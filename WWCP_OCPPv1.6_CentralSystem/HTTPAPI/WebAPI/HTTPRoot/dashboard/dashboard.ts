///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartDashboard(ZoomToBoundingBox: boolean) {

    HTTPGet("/tokens",

        (status, response) => {

            try
            {

                let list = JSON.parse(response) as any[];

                

            }
            catch (exception) { }

        },

        (status, statusText, response) => {
        }

    );

    //var refresh = setTimeout(StartDashboard, 30000);

}
