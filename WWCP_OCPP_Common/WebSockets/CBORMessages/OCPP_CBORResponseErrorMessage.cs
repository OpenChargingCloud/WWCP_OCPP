/*
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// An OCPP CBOR response error message transport container: the array of a JSON
    /// response error message, as CBOR, with its error details as a CBOR map.
    ///
    /// The error details are kept as the JSON object they are for every other
    /// transport, so that an error is made and read by the same code whatever
    /// the connection speaks.
    /// </summary>
    /// <param name="ResponseTimestamp">The response time stamp.</param>
    /// <param name="EventTrackingId">An optional event tracking identification.</param>
    /// <param name="NetworkingMode">The networking mode to use.</param>
    /// <param name="Destination">The networking node identification or Any- or Multicast address of the message destination.</param>
    /// <param name="NetworkPath">The (recorded) path of the message through the overlay network.</param>
    /// <param name="RequestId">The unique request identification copied from the request.</param>
    /// <param name="ErrorCode">An OCPP error code.</param>
    /// <param name="ErrorDescription">An optional error description.</param>
    /// <param name="ErrorDetails">Optional error details.</param>
    /// <param name="CancellationToken">The cancellation token.</param>
    public class OCPP_CBORResponseErrorMessage(DateTimeOffset     ResponseTimestamp,
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
        public DateTimeOffset     ResponseTimestamp    { get; }      = ResponseTimestamp;

        /// <summary>
        /// The event tracking identification for correlating this message with other events.
        /// </summary>
        public EventTracking_Id   EventTrackingId      { get; }      = EventTrackingId;

        /// <summary>
        /// The OCPP networking mode to use.
        /// </summary>
        public NetworkingMode     NetworkingMode       { get; set; } = NetworkingMode;

        /// <summary>
        /// The networking node identification or Any- or Multicast address of the message destination.
        /// </summary>
        public SourceRouting      Destination          { get; }      = Destination;

        /// <summary>
        /// The (recorded) path of the message through the overlay network.
        /// </summary>
        public NetworkPath        NetworkPath          { get; }      = NetworkPath;

        /// <summary>
        /// The unique request identification copied from the request.
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


        #region (static) From(JSONResponseErrorMessage)

        /// <summary>
        /// The given JSON response error message, for a connection that speaks CBOR.
        /// </summary>
        /// <param name="JSONResponseErrorMessage">A JSON response error message.</param>
        public static OCPP_CBORResponseErrorMessage From(OCPP_JSONResponseErrorMessage JSONResponseErrorMessage)

            => new (JSONResponseErrorMessage.ResponseTimestamp,
                    JSONResponseErrorMessage.EventTrackingId,
                    JSONResponseErrorMessage.NetworkingMode,
                    JSONResponseErrorMessage.Destination,
                    JSONResponseErrorMessage.NetworkPath,
                    JSONResponseErrorMessage.RequestId,
                    JSONResponseErrorMessage.ErrorCode,
                    JSONResponseErrorMessage.ErrorDescription,
                    JSONResponseErrorMessage.ErrorDetails,
                    JSONResponseErrorMessage.CancellationToken);

        #endregion

        #region ToJSONMessage()

        /// <summary>
        /// This response error message, for a connection that speaks JSON.
        /// </summary>
        public OCPP_JSONResponseErrorMessage ToJSONMessage()

            => new (ResponseTimestamp,
                    EventTrackingId,
                    NetworkingMode,
                    Destination,
                    NetworkPath,
                    RequestId,
                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails,
                    CancellationToken);

        #endregion


        #region TryParse(CBORArray, out ResponseErrorMessage, out ErrorResponse, ImplicitSourceNodeId = null)

        /// <summary>
        /// Try to parse the given CBOR representation of a response error message.
        /// </summary>
        /// <param name="CBORArray">The CBOR array to be parsed.</param>
        /// <param name="ResponseErrorMessage">The parsed OCPP response error message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ImplicitSourceNodeId">An optional source networking node identification, e.g. from the HTTP WebSockets connection.</param>
        public static Boolean TryParse(CBORValue                                               CBORArray,
                                       [NotNullWhen(true)]  out OCPP_CBORResponseErrorMessage?  ResponseErrorMessage,
                                                            out String?                        ErrorResponse,
                                       NetworkingNode_Id?                                      ImplicitSourceNodeId   = null)
        {

            ResponseErrorMessage = null;

            if (!OCPP_CBORFrame.TrySplit(CBORArray,
                                         out var header,
                                         out var errorDetails,
                                         out ErrorResponse))
            {
                return false;
            }

            if (!OCPP_JSONResponseErrorMessage.TryParse(header,
                                                       out var jsonResponseErrorMessage,
                                                       ImplicitSourceNodeId))
            {
                // No error response: This is not a response error message, but may be a message of another kind.
                return false;
            }

            JObject? details;

            try
            {
                details = CBORJSON.ToJSON(errorDetails) as JObject;
            }
            catch (Exception e)
            {
                ErrorResponse = "The error details of the given response error message could not be read: " + e.Message;
                return false;
            }

            ResponseErrorMessage = new OCPP_CBORResponseErrorMessage(
                                      jsonResponseErrorMessage.ResponseTimestamp,
                                      jsonResponseErrorMessage.EventTrackingId,
                                      jsonResponseErrorMessage.NetworkingMode,
                                      jsonResponseErrorMessage.Destination,
                                      jsonResponseErrorMessage.NetworkPath,
                                      jsonResponseErrorMessage.RequestId,
                                      jsonResponseErrorMessage.ErrorCode,
                                      jsonResponseErrorMessage.ErrorDescription,
                                      details,
                                      jsonResponseErrorMessage.CancellationToken
                                  );

            return true;

        }

        #endregion

        #region ToCBOR(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the CBOR array of this response error message.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public CBORValue ToCBOR(NetworkingMode? ForcedNetworkingMode = null)

            => OCPP_CBORFrame.Join(
                   ToJSONMessage().ToJSON(ForcedNetworkingMode),
                   CBORJSON.ToCBOR(ErrorDetails)
               );

        #endregion

        #region ToByteArray(ForcedNetworkingMode = null)

        /// <summary>
        /// Return the bytes of this response error message, as they are sent in a binary frame.
        /// </summary>
        /// <param name="ForcedNetworkingMode">Optionally enforce the given networking mode.</param>
        public Byte[] ToByteArray(NetworkingMode? ForcedNetworkingMode = null)

            => ToCBOR(ForcedNetworkingMode).ToByteArray();

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"RESPONSE ERROR[{RequestId}] => {ErrorCode}: {ErrorDescription}";

        #endregion

    }

}
