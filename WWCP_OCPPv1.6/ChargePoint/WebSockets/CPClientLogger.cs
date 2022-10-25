/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A CP client.
    /// </summary>
    public partial class ChargePointWSClient
    {

        /// <summary>
        /// A CP client (HTTP/SOAP client) logger.
        /// </summary>
        public class CPClientLogger : HTTPClientLogger
        {

            #region Data

            /// <summary>
            /// The default context for this logger.
            /// </summary>
            public const String DefaultContext = "OCPP_WS_CPClient";

            #endregion

            #region Properties

            /// <summary>
            /// The attached OCPP SOAP CP client.
            /// </summary>
            public ICPClient  CPClient    { get; }

            #endregion

            #region Constructor(s)

            #region CPClientLogger(CPClient, Context = DefaultContext, LogfileCreator = null)

            /// <summary>
            /// Create a new CP client logger using the default logging delegates.
            /// </summary>
            /// <param name="CPClient">A OCPP CP client.</param>
            /// <param name="LoggingPath">The logging path.</param>
            /// <param name="Context">A context of this API.</param>
            /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
            public CPClientLogger(ChargePointSOAPClient    CPClient,
                                  String                   LoggingPath,
                                  String                   Context          = DefaultContext,
                                  LogfileCreatorDelegate?  LogfileCreator   = null)

                : this(CPClient,
                       LoggingPath,
                       Context.IsNotNullOrEmpty() ? Context : DefaultContext,
                       null,
                       null,
                       null,
                       null,

                       LogfileCreator: LogfileCreator)

            { }

            #endregion

            #region CPClientLogger(CPClient, Context, ... Logging delegates ...)

            /// <summary>
            /// Create a new CP client logger using the given logging delegates.
            /// </summary>
            /// <param name="CPClient">A OCPP CP client.</param>
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
            public CPClientLogger(ICPClient                    CPClient,
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

                : base(CPClient,
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

                this.CPClient = CPClient ?? throw new ArgumentNullException(nameof(CPClient), "The given EMP client must not be null!");

                #endregion


                //#region GetChargePointList

                //RegisterEvent("GetChargePointListRequest",
                //              handler => CPClient.OnGetChargePointListSOAPRequest += handler,
                //              handler => CPClient.OnGetChargePointListSOAPRequest -= handler,
                //              "GetChargePointList", "OCPP", "Requests", "All").
                //    RegisterDefaultConsoleLogTarget(this).
                //    RegisterDefaultDiscLogTarget(this);

                //RegisterEvent("GetChargePointListResponse",
                //              handler => CPClient.OnGetChargePointListSOAPResponse += handler,
                //              handler => CPClient.OnGetChargePointListSOAPResponse -= handler,
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
