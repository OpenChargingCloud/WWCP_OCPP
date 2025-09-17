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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify periodic event stream message.
    /// </summary>
    /// <remarks>There will be NO RESPONSE to this request!</remarks>
    public class NotifyPeriodicEventStreamMessage : AMessage<NotifyPeriodicEventStreamMessage>,
                                                    IMessage
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyPeriodicEventStreamMessage");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the periodic event stream.
        /// </summary>
        [Mandatory]
        public PeriodicEventStream_Id          Id                    { get; }

        /// <summary>
        /// The enumeration of periodic event stream data elements.
        /// </summary>
        [Mandatory]
        public IEnumerable<StreamDataElement>  StreamDataElements    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new notify periodic event stream request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Id">The unique identification of the periodic event stream.</param>
        /// <param name="StreamDataElements">An enumeration of periodic event stream data elements.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="MessageId">An optional request identification.</param>
        /// <param name="MessageTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyPeriodicEventStreamMessage(SourceRouting                   Destination,
                                                PeriodicEventStream_Id          Id,
                                                IEnumerable<StreamDataElement>  StreamDataElements,

                                                IEnumerable<KeyPair>?           SignKeys              = null,
                                                IEnumerable<SignInfo>?          SignInfos             = null,
                                                IEnumerable<Signature>?         Signatures            = null,

                                                CustomData?                     CustomData            = null,

                                                Request_Id?                     MessageId             = null,
                                                DateTimeOffset?                 MessageTimestamp      = null,
                                                EventTracking_Id?               EventTrackingId       = null,
                                                NetworkPath?                    NetworkPath           = null,
                                                SerializationFormats?           SerializationFormat   = null,
                                                CancellationToken               CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyPeriodicEventStreamMessage)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   MessageId,
                   MessageTimestamp,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            if (!StreamDataElements.Any())
                throw new ArgumentException("The given enumeration of periodic event stream data elements must not be empty!",
                                            nameof(StreamDataElements));

            this.Id                  = Id;
            this.StreamDataElements  = StreamDataElements.Distinct();


            unchecked
            {

                hashCode = this.Id.                GetHashCode()  * 5 ^
                           this.StreamDataElements.CalcHashCode() * 3 ^
                           base.                   GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyPeriodicEventStream",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "StreamDataElementType": {
        //             "javaType": "StreamDataElement",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "t": {
        //                     "description": "Offset relative to _basetime_ of this message. _basetime_ + _t_ is timestamp of recorded value.",
        //                     "type": "number"
        //                 },
        //                 "v": {
        //                     "type": "string",
        //                     "maxLength": 2500
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "t",
        //                 "v"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "data": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/StreamDataElementType"
        //             },
        //             "minItems": 1
        //         },
        //         "id": {
        //             "description": "Id of stream.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "pending": {
        //             "description": "Number of data elements still pending to be sent.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "basetime": {
        //             "description": "Base timestamp to add to time offset of values.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id",
        //         "pending",
        //         "basetime",
        //         "data"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an NotifyPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomNotifyPeriodicEventStreamMessageParser">A delegate to parse custom NotifyPeriodicEventStream requests.</param>
        public static NotifyPeriodicEventStreamMessage Parse(JObject                                                         JSON,
                                                             Request_Id                                                      RequestId,
                                                             SourceRouting                                                   Destination,
                                                             NetworkPath                                                     NetworkPath,
                                                             CustomJObjectParserDelegate<NotifyPeriodicEventStreamMessage>?  CustomNotifyPeriodicEventStreamMessageParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyPeriodicEventStreamRequest,
                         out var errorResponse,
                         CustomNotifyPeriodicEventStreamMessageParser))
            {
                return notifyPeriodicEventStreamRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyPeriodicEventStream request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyPeriodicEventStreamMessage, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyPeriodicEventStreamMessage">The parsed NotifyPeriodicEventStream request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out NotifyPeriodicEventStreamMessage?  NotifyPeriodicEventStreamMessage,
                                       [NotNullWhen(false)] out String?                            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        Destination,
                        NetworkPath,
                        out NotifyPeriodicEventStreamMessage,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a NotifyPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyPeriodicEventStreamMessage">The parsed NotifyPeriodicEventStream message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyPeriodicEventStreamMessageParser">A delegate to parse custom NotifyPeriodicEventStream requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       SourceRouting                                                   Destination,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out NotifyPeriodicEventStreamMessage?      NotifyPeriodicEventStreamMessage,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyPeriodicEventStreamMessage>?  CustomNotifyPeriodicEventStreamMessageParser)
        {

            try
            {

                NotifyPeriodicEventStreamMessage = null;

                #region Id                    [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "periodic event stream identification",
                                         PeriodicEventStream_Id.TryParse,
                                         out PeriodicEventStream_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StreamDataElements    [mandatory]

                if (!JSON.ParseMandatoryHashSet("data",
                                                "periodic event stream data elements",
                                                StreamDataElement.TryParse,
                                                out HashSet<StreamDataElement> StreamDataElements,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData            [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyPeriodicEventStreamMessage = new NotifyPeriodicEventStreamMessage(

                                                       Destination,
                                                       Id,
                                                       StreamDataElements,

                                                       null,
                                                       null,
                                                       Signatures,

                                                       CustomData,

                                                       RequestId,
                                                       null,
                                                       null,
                                                       NetworkPath

                                                   );

                if (CustomNotifyPeriodicEventStreamMessageParser is not null)
                    NotifyPeriodicEventStreamMessage = CustomNotifyPeriodicEventStreamMessageParser(JSON,
                                                                                                    NotifyPeriodicEventStreamMessage);

                return true;

            }
            catch (Exception e)
            {
                NotifyPeriodicEventStreamMessage  = null;
                ErrorResponse                     = "The given JSON representation of a NotifyPeriodicEventStream request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyPeriodicEventStreamMessageSerializer = null, CustomStreamDataElementSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyPeriodicEventStreamMessageSerializer">A delegate to serialize custom NotifyPeriodicEventStream requests.</param>
        /// <param name="CustomStreamDataElementSerializer">A delegate to serialize custom stream data elements.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                             IncludeJSONLDContext                               = false,
                              CustomJObjectSerializerDelegate<NotifyPeriodicEventStreamMessage>?  CustomNotifyPeriodicEventStreamMessageSerializer   = null,
                              CustomJObjectSerializerDelegate<StreamDataElement>?                 CustomStreamDataElementSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("id",           Id.                  ToString()),
                                 new JProperty("data",         new JArray(StreamDataElements.Select(streamDataElement => streamDataElement.ToJSON(CustomStreamDataElementSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.        Select(signature         => signature.        ToJSON(CustomSignatureSerializer,
                                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyPeriodicEventStreamMessageSerializer is not null
                       ? CustomNotifyPeriodicEventStreamMessageSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyPeriodicEventStreamMessage1, NotifyPeriodicEventStreamMessage2)

        /// <summary>
        /// Compares two NotifyPeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="NotifyPeriodicEventStreamMessage1">A NotifyPeriodicEventStream request.</param>
        /// <param name="NotifyPeriodicEventStreamMessage2">Another NotifyPeriodicEventStream request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyPeriodicEventStreamMessage? NotifyPeriodicEventStreamMessage1,
                                           NotifyPeriodicEventStreamMessage? NotifyPeriodicEventStreamMessage2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyPeriodicEventStreamMessage1, NotifyPeriodicEventStreamMessage2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyPeriodicEventStreamMessage1 is null || NotifyPeriodicEventStreamMessage2 is null)
                return false;

            return NotifyPeriodicEventStreamMessage1.Equals(NotifyPeriodicEventStreamMessage2);

        }

        #endregion

        #region Operator != (NotifyPeriodicEventStreamMessage1, NotifyPeriodicEventStreamMessage2)

        /// <summary>
        /// Compares two NotifyPeriodicEventStream requests for inequality.
        /// </summary>
        /// <param name="NotifyPeriodicEventStreamMessage1">A NotifyPeriodicEventStream request.</param>
        /// <param name="NotifyPeriodicEventStreamMessage2">Another NotifyPeriodicEventStream request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyPeriodicEventStreamMessage? NotifyPeriodicEventStreamMessage1,
                                           NotifyPeriodicEventStreamMessage? NotifyPeriodicEventStreamMessage2)

            => !(NotifyPeriodicEventStreamMessage1 == NotifyPeriodicEventStreamMessage2);

        #endregion

        #endregion

        #region IEquatable<NotifyPeriodicEventStreamMessage> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyPeriodicEventStreamMessage requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyPeriodicEventStreamMessage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyPeriodicEventStreamMessage notifyPeriodicEventStreamRequest &&
                   Equals(notifyPeriodicEventStreamRequest);

        #endregion

        #region Equals(NotifyPeriodicEventStreamMessage)

        /// <summary>
        /// Compares two NotifyPeriodicEventStreamMessage requests for equality.
        /// </summary>
        /// <param name="NotifyPeriodicEventStreamMessage">A NotifyPeriodicEventStreamMessage request to compare with.</param>
        public override Boolean Equals(NotifyPeriodicEventStreamMessage? NotifyPeriodicEventStreamMessage)

            => NotifyPeriodicEventStreamMessage is not null &&

               Id.                        Equals(NotifyPeriodicEventStreamMessage.Id) &&

               StreamDataElements.Count().Equals(NotifyPeriodicEventStreamMessage.StreamDataElements.Count()) &&
               StreamDataElements.All(NotifyPeriodicEventStreamMessage.StreamDataElements.Contains)           &&

               base.               GenericEquals(NotifyPeriodicEventStreamMessage);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Id}: {StreamDataElements.AggregateWith(", ")}";

        #endregion

    }

}
