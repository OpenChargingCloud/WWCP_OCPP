﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Text;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// A OCPP WebSocket JSON request error message.
    /// </summary>
    /// <param name="ResponseTimestamp">The response time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The OCPP networking mode to use.</param>
    /// <param name="Destination">The networking node identification of the message destination.</param>
    /// <param name="NetworkPath">The optional (recorded) path of the request through the overlay network.</param>
    /// <param name="RequestId">An unique request identification.</param>
    /// <param name="ErrorCode">An OCPP error code.</param>
    /// <param name="ErrorDescription">An optional error description.</param>
    /// <param name="ErrorDetails">Optional error details.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_JSONRequestErrorMessage(DateTime           ResponseTimestamp,
                                              EventTracking_Id   EventTrackingId,
                                              NetworkingMode     NetworkingMode,
                                              SourceRouting      Destination,
                                              NetworkPath        NetworkPath,
                                              Request_Id         RequestId,
                                              ResultCode         ErrorCode,
                                              String?            ErrorDescription    = null,
                                              JObject?           ErrorDetails        = null,
                                              CancellationToken  CancellationToken   = default)
    {

        #region Properties

        /// <summary>
        /// The response time stamp.
        /// </summary>
        public DateTime           ResponseTimestamp    { get; }      = ResponseTimestamp;

        /// <summary>
        /// The event tracking identification.
        /// </summary>
        public EventTracking_Id   EventTrackingId      { get; }      = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; set; } = NetworkingMode;

        /// <summary>
        /// The networking node identification of the message destination.
        /// </summary>
        public SourceRouting      Destination          { get; }      = Destination;

        /// <summary>
        /// The (recorded) path of the request through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id         RequestId            { get; }      = RequestId;

        /// <summary>
        /// The OCPP error code.
        /// </summary>
        public ResultCode         ErrorCode            { get; }      = ErrorCode;

        /// <summary>
        /// The optional error description.
        /// </summary>
        public String             ErrorDescription     { get; }      = ErrorDescription ?? "";

        /// <summary>
        /// Optional error details.
        /// </summary>
        public JObject            ErrorDetails         { get; }      = ErrorDetails     ?? [];

        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken  CancellationToken    { get; }      = CancellationToken;

        #endregion


        #region (static) CouldNotParse     (RequestId, Action, JSONObjectRequest, ErrorResponse = null)

        public static OCPP_JSONRequestErrorMessage CouldNotParse(Request_Id  RequestId,
                                                                 String      Action,
                                                                 JObject     JSONObjectRequest,
                                                                 String?     ErrorResponse   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request could not be parsed!",
                    JSONObject.Create(

                              new JProperty("request",   JSONObjectRequest),

                        ErrorResponse is not null
                            ? new JProperty("error",     ErrorResponse)
                            : null

                    ));

        public static OCPP_JSONRequestErrorMessage CouldNotParse(EventTracking_Id  EventTrackingId,
                                                                 Request_Id        RequestId,
                                                                 String            Action,
                                                                 JObject           JSONObjectRequest,
                                                                 String?           ErrorResponse   = null)

            => new (Timestamp.Now,
                    EventTrackingId,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request could not be parsed!",
                    JSONObject.Create(

                              new JProperty("request",   JSONObjectRequest),

                        ErrorResponse is not null
                            ? new JProperty("error",     ErrorResponse)
                            : null

                    ));

        #endregion

        #region (static) CouldNotParse     (RequestId, Action, JSONArrayRequest,  ErrorResponse = null)

        public static OCPP_JSONRequestErrorMessage CouldNotParse(Request_Id  RequestId,
                                                                 String      Action,
                                                                 JArray      JSONArrayRequest,
                                                                 String?     ErrorResponse   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request could not be parsed!",
                    JSONObject.Create(

                              new JProperty("request",   JSONArrayRequest),

                        ErrorResponse is not null
                            ? new JProperty("error",     ErrorResponse)
                            : null

                    ));

        #endregion

        #region (static) CouldNotParse     (RequestId, Action, BinaryRequest,     ErrorResponse = null)

        public static OCPP_JSONRequestErrorMessage CouldNotParse(Request_Id  RequestId,
                                                                 String      Action,
                                                                 Byte[]      BinaryRequest,
                                                                 String?     ErrorResponse   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request could not be parsed!",
                    JSONObject.Create(

                              new JProperty("request",   BinaryRequest.ToBase64()),

                        ErrorResponse is not null
                            ? new JProperty("error",     ErrorResponse)
                            : null

                    ));


        public static OCPP_JSONRequestErrorMessage CouldNotParse(EventTracking_Id  EventTrackingId,
                                                                 Request_Id        RequestId,
                                                                 String            Action,
                                                                 Byte[]            BinaryRequest,
                                                                 String?           ErrorResponse   = null)

            => new (Timestamp.Now,
                    EventTrackingId,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request could not be parsed!",
                    JSONObject.Create(

                              new JProperty("request",   BinaryRequest.ToBase64()),

                        ErrorResponse is not null
                            ? new JProperty("error",     ErrorResponse)
                            : null

                    ));

        #endregion


        #region (static) FormationViolation(RequestId, Action, JSONObjectRequest, Exception)

        public static OCPP_JSONRequestErrorMessage FormationViolation(Request_Id  RequestId,
                                                                      String      Action,
                                                                      JObject     JSONObjectRequest,
                                                                      Exception   Exception)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",      JSONObjectRequest),
                        new JProperty("exception",    Exception.Message),
                        new JProperty("stacktrace",   Exception.StackTrace)
                    ));


        public static OCPP_JSONRequestErrorMessage FormationViolation(EventTracking_Id  EventTrackingId,
                                                                      Request_Id        RequestId,
                                                                      String            Action,
                                                                      JObject           JSONObjectRequest,
                                                                      Exception         Exception)

            => new (Timestamp.Now,
                    EventTrackingId,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",      JSONObjectRequest),
                        new JProperty("exception",    Exception.Message),
                        new JProperty("stacktrace",   Exception.StackTrace)
                    ));

        #endregion

        #region (static) FormationViolation(RequestId, Action, JSONArrayRequest,  Exception)

        public static OCPP_JSONRequestErrorMessage FormationViolation(Request_Id  RequestId,
                                                                      String      Action,
                                                                      JArray      JSONArrayRequest,
                                                                      Exception   Exception)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",     JSONArrayRequest),
                        new JProperty("exception",   Exception.Message),
                        new JProperty("stacktrace",  Exception.StackTrace)
                    ));

        #endregion

        #region (static) FormationViolation(RequestId, Action, BinaryRequest,     Exception)

        public static OCPP_JSONRequestErrorMessage FormationViolation(Request_Id  RequestId,
                                                                      String      Action,
                                                                      Byte[]      BinaryRequest,
                                                                      Exception   Exception)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",      BinaryRequest.ToBase64()),
                        new JProperty("exception",    Exception.Message),
                        new JProperty("stacktrace",   Exception.StackTrace)
                    ));


        public static OCPP_JSONRequestErrorMessage FormationViolation(EventTracking_Id  EventTrackingId,
                                                                      Request_Id        RequestId,
                                                                      String            Action,
                                                                      Byte[]            BinaryRequest,
                                                                      Exception         Exception)

            => new (Timestamp.Now,
                    EventTrackingId,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.FormationViolation,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",      BinaryRequest.ToBase64()),
                        new JProperty("exception",    Exception.Message),
                        new JProperty("stacktrace",   Exception.StackTrace)
                    ));

        #endregion


        #region (static) ExceptionOccurred(...)

        public static OCPP_JSONRequestErrorMessage ExceptionOccurred(EventTracking_Id  EventTrackingId,
                                                                     Request_Id        RequestId,
                                                                     String            Action,
                                                                     JObject           JSONObjectRequest,
                                                                     Exception         Exception)

            => new (Timestamp.Now,
                    EventTrackingId,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.InternalError,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",      JSONObjectRequest),
                        new JProperty("exception",    Exception.Message),
                        new JProperty("stacktrace",   Exception.StackTrace)
                    ));

        public static OCPP_JSONRequestErrorMessage ExceptionOccurred(EventTracking_Id  EventTrackingId,
                                                                     Request_Id        RequestId,
                                                                     String            Action,
                                                                     Byte[]            BinaryRequest,
                                                                     Exception         Exception)

            => new (Timestamp.Now,
                    EventTrackingId,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId,
                    ResultCode.InternalError,
                    $"Processing the given '{Action}' request led to an exception!",
                    new JObject(
                        new JProperty("request",      BinaryRequest.ToBase64()),
                        new JProperty("exception",    Exception.Message),
                        new JProperty("stacktrace",   Exception.StackTrace)
                    ));

        #endregion


        #region (static) InternalError     (Sender, EventTrackingId, JSONTextRequest,   Exception, RequestId = null)

        public static OCPP_JSONRequestErrorMessage InternalError(String            Sender,
                                                                 EventTracking_Id  EventTrackingId,
                                                                 String            JSONTextRequest,
                                                                 Exception         Exception,
                                                                 Request_Id?       RequestId   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId ?? Request_Id.Zero,
                    ResultCode.InternalError,
                    $"The OCPP message received in '{Sender}' led to an exception!",
                    new JObject(
                        new JProperty("eventTrackingId",  EventTrackingId.ToString()),
                        new JProperty("request",          JSONTextRequest),
                        new JProperty("exception",        Exception.Message),
                        new JProperty("stacktrace",       Exception.StackTrace)
                    ));

        #endregion

        #region (static) InternalError     (Sender, EventTrackingId, JSONObjectRequest, Exception, RequestId = null)

        public static OCPP_JSONRequestErrorMessage InternalError(String            Sender,
                                                                 EventTracking_Id  EventTrackingId,
                                                                 JObject           JSONObjectRequest,
                                                                 Exception         Exception,
                                                                 Request_Id?       RequestId   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId ?? Request_Id.Zero,
                    ResultCode.InternalError,
                    $"The OCPP message received in '{Sender}' led to an exception!",
                    new JObject(
                        new JProperty("eventTrackingId",  EventTrackingId.ToString()),
                        new JProperty("request",          JSONObjectRequest),
                        new JProperty("exception",        Exception.Message),
                        new JProperty("stacktrace",       Exception.StackTrace)
                    ));

        #endregion

        #region (static) InternalError     (Sender, EventTrackingId, OCPPArrayRequest,  Exception, RequestId = null)

        public static OCPP_JSONRequestErrorMessage InternalError(String            Sender,
                                                                 EventTracking_Id  EventTrackingId,
                                                                 JArray            OCPPArrayRequest,
                                                                 Exception         Exception,
                                                                 Request_Id?       RequestId   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId ?? Request_Id.Zero,
                    ResultCode.InternalError,
                    $"The OCPP message received in '{Sender}' led to an exception!",
                    new JObject(
                        new JProperty("eventTrackingId",  EventTrackingId.ToString()),
                        new JProperty("request",          OCPPArrayRequest),
                        new JProperty("exception",        Exception.Message),
                        new JProperty("stacktrace",       Exception.StackTrace)
                    ));

        #endregion

        #region (static) InternalError     (Sender, EventTrackingId, OCPPBinaryRequest, Exception, RequestId = null)

        public static OCPP_JSONRequestErrorMessage InternalError(String            Sender,
                                                                 EventTracking_Id  EventTrackingId,
                                                                 Byte[]            OCPPBinaryRequest,
                                                                 Exception         Exception,
                                                                 Request_Id?       RequestId   = null)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode.Standard,
                    SourceRouting.Zero,
                    NetworkPath.Empty,
                    RequestId ?? Request_Id.Zero,
                    ResultCode.InternalError,
                    $"The OCPP message received in '{Sender}' led to an exception!",
                    new JObject(
                        new JProperty("eventTrackingId",  EventTrackingId.ToString()),
                        new JProperty("request",          OCPPBinaryRequest.ToBase64()),
                        new JProperty("exception",        Exception.Message),
                        new JProperty("stacktrace",       Exception.StackTrace)
                    ));

        #endregion


        // Error Code                     Description
        // ------------------------------------------------------------------------------------------------
        // NotImplemented                 Requested Action is not known by receiver
        // NotSupported                   Requested Action is recognized but not supported by the receiver
        // InternalError                  An internal error occurred and the receiver was not able to process the requested Action successfully
        // ProtocolError                  Payload for Action is incomplete
        // SecurityError                  During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
        // FormationViolation             Payload for Action is syntactically incorrect or not conform the PDU structure for Action
        // PropertyConstraintViolation    Payload is syntactically correct but at least one field contains an invalid value
        // OccurenceConstraintViolation   Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
        // TypeConstraintViolation        Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
        // GenericError                   Any other error not covered by the previous ones

        #region TryParse(JSONArray, out ResponseErrorMessage, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given text representation of an error message.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="ResponseErrorMessage">The parsed OCPP RequestErrorMessage.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        public static Boolean TryParse(JArray                                                  JSONArray,
                                       [NotNullWhen(true)]  out OCPP_JSONRequestErrorMessage?  ResponseErrorMessage,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       NetworkingNode_Id?                                      ImplicitSourceNodeId   = null)
        {

            ResponseErrorMessage = null;
            ErrorResponse         = null;

            try
            {

                #region OCPP standard mode

                // [
                //     4,                         // MessageType: CALLERROR
                //    "19223201",                 // RequestId from request
                //    "<errorCode>",
                //    "<errorDescription>",
                //    {
                //        <errorDetails>
                //    }
                // ]

                if (JSONArray.Count            == 5                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 4                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.String   &&
                    JSONArray[3].Type          == JTokenType.String   &&
                    JSONArray[4].Type          == JTokenType.Object)
                {

                    var networkPath = NetworkPath.Empty;

                    if (ImplicitSourceNodeId.HasValue &&
                        ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    {
                        networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                    }

                    if (!Request_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var requestId))
                    {
                        ErrorResponse = $"Could not parse the given request identification: {JSONArray[1]}";
                        return false;
                    }

                    if (!ResultCode.TryParse(JSONArray[2]?.Value<String>() ?? "", out var wsErrorCode))
                    {
                        ErrorResponse = $"Could not parse the given request error code: {JSONArray[2]}";
                        return false;
                    }

                    var errorDescription = JSONArray[3]?.Value<String>();

                    if (JSONArray[4] is not JObject errorDetails)
                    {
                        ErrorResponse = $"Could not parse the given request error details: {JSONArray[4]}";
                        return false;
                    }

                    ResponseErrorMessage = new OCPP_JSONRequestErrorMessage(
                                               Timestamp.Now,
                                               EventTracking_Id.New,
                                               NetworkingMode.Standard,
                                               SourceRouting.Zero,
                                               networkPath,
                                               requestId,
                                               wsErrorCode,
                                               errorDescription,
                                               errorDetails
                                           );

                    return true;

                }

                #endregion

                #region OCPP Overlay Network mode

                // [
                //     4,                         // MessageType: CALLERROR
                //    DestinationId,
                //    [ NetworkPath ],
                //    "19223201",                 // RequestId from request
                //    "<errorCode>",
                //    "<errorDescription>",
                //    {
                //        <errorDetails>
                //    }
                // ]

                if (JSONArray.Count            == 7                   &&
                    JSONArray[0].Type          == JTokenType.Integer  &&
                    JSONArray[0].Value<Byte>() == 4                   &&
                    JSONArray[1].Type          == JTokenType.String   &&
                    JSONArray[2].Type          == JTokenType.Array    &&
                    JSONArray[3].Type          == JTokenType.String   &&
                    JSONArray[4].Type          == JTokenType.String   &&
                    JSONArray[5].Type          == JTokenType.String   &&
                    JSONArray[6].Type          == JTokenType.Object)
                {

                    if (!NetworkingNode_Id.TryParse(JSONArray[1]?.Value<String>() ?? "", out var destinationId))
                    {
                        ErrorResponse = $"Could not parse the given destination networking (node) identification: {JSONArray[1]}";
                        return false;
                    }

                    if (JSONArray[2] is not JArray networkPathJSONArray ||
                        !NetworkPath.TryParse(networkPathJSONArray, out var networkPath, out _) || networkPath is null)
                    {
                        ErrorResponse = $"Could not parse the given network path: {JSONArray[2]}";
                        return false;
                    }

                    if (ImplicitSourceNodeId.HasValue &&
                        ImplicitSourceNodeId.Value != NetworkingNode_Id.Zero)
                    {

                        //if (networkPath.Length > 0 &&
                        //    networkPath.Last() != ImplicitSourceNodeId)
                        //{
                        //    networkPath = networkPath.Append(ImplicitSourceNodeId.Value);
                        //}

                        if (networkPath.Length == 0)
                            networkPath = networkPath.Append(ImplicitSourceNodeId.Value);

                    }

                    if (!Request_Id.TryParse(JSONArray[3]?.Value<String>() ?? "", out var requestId))
                    {
                        ErrorResponse = $"Could not parse the given request identification: {JSONArray[3]}";
                        return false;
                    }

                    if (!ResultCode.TryParse(JSONArray[4]?.Value<String>() ?? "", out var wsErrorCode))
                    {
                        ErrorResponse = $"Could not parse the given request error code: {JSONArray[4]}";
                        return false;
                    }

                    var errorDescription = JSONArray[5]?.Value<String>();

                    // We allow null or empty error descriptions!
                    if (JSONArray[6] is not JObject errorDetails)
                    {
                        ErrorResponse = $"Could not parse the given request error details: {JSONArray[6]}";
                        return false;
                    }

                    ResponseErrorMessage = new OCPP_JSONRequestErrorMessage(
                                               Timestamp.Now,
                                               EventTracking_Id.New,
                                               NetworkingMode.OverlayNetwork,
                                               SourceRouting.To(destinationId),
                                               networkPath,
                                               requestId,
                                               wsErrorCode,
                                               errorDescription,
                                               errorDetails.Count > 0
                                                   ? errorDetails
                                                   : null
                                           );

                    return true;

                }

                #endregion

            }
            catch (Exception e)
            {
                ErrorResponse = $"Could not parse the given JSON response message: {e.Message}";
                return false;
            }

            ErrorResponse = $"Could not parse the given JSON response message: {JSONArray}";
            return false;

        }

        #endregion

        #region ToJSON(ForcedNetworkingMode = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given </param>
        public JArray ToJSON(NetworkingMode? ForcedNetworkingMode = null)

            => (ForcedNetworkingMode ?? NetworkingMode) switch {

                   #region OCPP Standard Mode

                   // [
                   //     4,            // MessageType: CALLERROR
                   //    "19223201",    // RequestId from request
                   //    "<errorCode>",
                   //    "<errorDescription>",
                   //    {
                   //        <errorDetails>
                   //    }
                   // ]
                   NetworkingMode.Unknown or
                   NetworkingMode.Standard
                       => new (4,
                              RequestId.ToString(),
                              ErrorCode.ToString(),
                              ErrorDescription,
                              ErrorDetails),

                   #endregion

                   #region OCPP Overlay Network Mode

                   // [
                   //     4,                    // MessageType: CALLRESULT
                   //     "CS1",                // Destination Identification/Any-/Multicast
                   //     [ "LC" ],             // Network Source Path
                   //    "19223201",            // RequestId copied from request
                   //    "<errorCode>",
                   //    "<errorDescription>",
                   //    {
                   //        <errorDetails>
                   //    }
                   // ]

                   _ => new (4,
                             Destination.Next.ToString(),
                             NetworkPath.     ToJSON(),
                             RequestId.       ToString(),
                             ErrorCode.       ToString(),
                             ErrorDescription,
                             ErrorDetails)

                   #endregion

               };

        #endregion

        #region ToByteArray(Format = None)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="Format">A JSON format.</param>
        public Byte[] ToByteArray(Formatting Format = Formatting.None)

            => Encoding.UTF8.GetBytes(ToJSON().ToString(Format));

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"REQUEST ERROR[{RequestId}] => {ErrorCode}: {ErrorDescription}";

        #endregion


    }

}
