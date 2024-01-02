/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A central system client.
    /// </summary>
    public partial class CentralSystemSOAPClient : ASOAPClient
    {

        /// <summary>
        /// A CS client (HTTP/SOAP client) logger.
        /// </summary>
        public class CSClientLogger : HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String DefaultContext = "OCPP_CSClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached central system client.
            /// </summary>
            public ICSOutgoingMessages CSClient    { get; }

            #endregion

            #region Constructor(s)

            #region CSClientLogger(CSClient, Context = DefaultContext, LogfileCreator = null)

            /// <summary>
            /// Create a new CS client logger using the default logging delegates.
            /// </summary>
            /// <param name="CSClient">A OCPP CS client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public CSClientLogger(CentralSystemSOAPClient  CSClient,
                                  String                   LoggingPath,
                                  String                   Context          = DefaultContext,
                                  LogfileCreatorDelegate?  LogfileCreator   = null)

                : this(CSClient,
                       LoggingPath,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,
                       null,
                       null,
                       null,
                       null,

                       LogfileCreator: LogfileCreator)

            { }

            #endregion

            #region CSClientLogger(CSClient, Context, ... Logging delegates ...)

            /// <summary>
            /// Create a new CS client logger using the given logging delegates.
            /// </summary>
            /// <param name="CSClient">A OCPP CS client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// 
            /// <param name="LogHTTPRequest_toConsole">A delegate to log incoming HTTP requests to console.</param>
            /// <param name="LogHTTPResponse_toConsole">A delegate to log HTTP requests/responses to console.</param>
            /// <param name="LogHTTPRequest_toDisc">A delegate to log incoming HTTP requests to disc.</param>
            /// <param name="LogHTTPResponse_toDisc">A delegate to log HTTP requests/responses to disc.</param>
            /// 
            /// <param name="LogHTTPRequest_toNetwork">A delegate to log incoming HTTP requests to a network target.</param>
            /// <param name="LogHTTPResponse_toNetwork">A delegate to log HTTP requests/responses to a network target.</param>
            /// <param name="LogHTTPRequest_toHTTPSSE">A delegate to log incoming HTTP requests to a HTTP client sent events source.</param>
            /// <param name="LogHTTPResponse_toHTTPSSE">A delegate to log HTTP requests/responses to a HTTP client sent events source.</param>
            /// 
            /// <param name="LogHTTPError_toConsole">A delegate to log HTTP errors to console.</param>
            /// <param name="LogHTTPError_toDisc">A delegate to log HTTP errors to disc.</param>
            /// <param name="LogHTTPError_toNetwork">A delegate to log HTTP errors to a network target.</param>
            /// <param name="LogHTTPError_toHTTPSSE">A delegate to log HTTP errors to a HTTP client sent events source.</param>
            /// 
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public CSClientLogger(ICentralSystemSOAPClient     CSClient,
                                  String                       LoggingPath,
                                  String                       Context,

                                  HTTPRequestLoggerDelegate?   LogHTTPRequest_toConsole    = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPResponse_toConsole   = null,
                                  HTTPRequestLoggerDelegate?   LogHTTPRequest_toDisc       = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPResponse_toDisc      = null,

                                  HTTPRequestLoggerDelegate?   LogHTTPRequest_toNetwork    = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPResponse_toNetwork   = null,
                                  HTTPRequestLoggerDelegate?   LogHTTPRequest_toHTTPSSE    = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPResponse_toHTTPSSE   = null,

                                  HTTPResponseLoggerDelegate?  LogHTTPError_toConsole      = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPError_toDisc         = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPError_toNetwork      = null,
                                  HTTPResponseLoggerDelegate?  LogHTTPError_toHTTPSSE      = null,

                                  LogfileCreatorDelegate?      LogfileCreator              = null)

                : base(CSClient,
                       LoggingPath,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,

                       LogHTTPRequest_toConsole,
                       LogHTTPResponse_toConsole,
                       LogHTTPRequest_toDisc,
                       LogHTTPResponse_toDisc,

                       LogHTTPRequest_toNetwork,
                       LogHTTPResponse_toNetwork,
                       LogHTTPRequest_toHTTPSSE,
                       LogHTTPResponse_toHTTPSSE,

                       LogHTTPError_toConsole,
                       LogHTTPError_toDisc,
                       LogHTTPError_toNetwork,
                       LogHTTPError_toHTTPSSE,

                       LogfileCreator)

            {

                #region Initial checks

                this.CSClient = CSClient ?? throw new ArgumentNullException(nameof(CSClient), "The given EMP client must not be null!");

                #endregion


                //#region GetChargePointList

                //RegisterEvent("GetChargePointListRequest",
                //              handler => CSClient.OnGetChargePointListSOAPRequest += handler,
                //              handler => CSClient.OnGetChargePointListSOAPRequest -= handler,
                //              "GetChargePointList", "OCPP", "Requests", "All").
                //    RegisterDefaultConsoleLogTarget(this).
                //    RegisterDefaultDiscLogTarget(this);

                //RegisterEvent("GetChargePointListResponse",
                //              handler => CSClient.OnGetChargePointListSOAPResponse += handler,
                //              handler => CSClient.OnGetChargePointListSOAPResponse -= handler,
                //              "GetChargePointList", "OCPP", "Responses", "All").
                //    RegisterDefaultConsoleLogTarget(this).
                //    RegisterDefaultDiscLogTarget(this);

                //#endregion



            }

            #endregion

            #endregion

        }

    }

}
