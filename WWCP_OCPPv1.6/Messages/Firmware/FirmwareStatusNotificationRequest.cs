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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The firmware status notification request.
    /// </summary>
    public class FirmwareStatusNotificationRequest : ARequest<FirmwareStatusNotificationRequest>,
                                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/firmwareStatusNotificationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status of the firmware installation.
        /// </summary>
        public FirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new firmware status notification request.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="Status">The status of the firmware installation.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public FirmwareStatusNotificationRequest(NetworkingNode_Id             NetworkingNodeId,
                                                 FirmwareStatus                Status,

                                                 IEnumerable<WWCP.KeyPair>?    SignKeys            = null,
                                                 IEnumerable<WWCP.SignInfo>?   SignInfos           = null,
                                                 IEnumerable<Signature>?  Signatures          = null,

                                                 CustomData?                   CustomData          = null,

                                                 Request_Id?                   RequestId           = null,
                                                 DateTime?                     RequestTimestamp    = null,
                                                 TimeSpan?                     RequestTimeout      = null,
                                                 EventTracking_Id?             EventTrackingId     = null,
                                                 NetworkPath?                  NetworkPath         = null,
                                                 CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(FirmwareStatusNotificationRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.Status = Status;

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:firmwareStatusNotificationRequest>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:firmwareStatusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:FirmwareStatusStatusNotificationRequest",
        //     "title":   "FirmwareStatusStatusNotificationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Downloaded",
        //                 "DownloadFailed",
        //                 "Downloading",
        //                 "Idle",
        //                 "InstallationFailed",
        //                 "Installing",
        //                 "Installed"
        //             ]
        //     }
        // },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, NetworkingNodeId)

        /// <summary>
        /// Parse the given XML representation of a firmware status notification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        public static FirmwareStatusNotificationRequest Parse(XElement           XML,
                                                              Request_Id         RequestId,
                                                              NetworkingNode_Id  NetworkingNodeId)
        {

            if (TryParse(XML,
                         RequestId,
                         NetworkingNodeId,
                         out var firmwareStatusNotificationRequest,
                         out var errorResponse) &&
                firmwareStatusNotificationRequest is not null)
            {
                return firmwareStatusNotificationRequest;
            }

            throw new ArgumentException("The given XML representation of a firmware status notification request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">An optional delegate to parse custom firmware status notification requests.</param>
        public static FirmwareStatusNotificationRequest Parse(JObject                                                          JSON,
                                                              Request_Id                                                       RequestId,
                                                              NetworkingNode_Id                                                NetworkingNodeId,
                                                              NetworkPath                                                      NetworkPath,
                                                              CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var firmwareStatusNotificationRequest,
                         out var errorResponse,
                         CustomFirmwareStatusNotificationRequestParser) &&
                firmwareStatusNotificationRequest is not null)
            {
                return firmwareStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a firmware status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, out FirmwareStatusNotificationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a firmware status notification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                XML,
                                       Request_Id                              RequestId,
                                       NetworkingNode_Id                       NetworkingNodeId,
                                       out FirmwareStatusNotificationRequest?  FirmwareStatusNotificationRequest,
                                       out String?                             ErrorResponse)
        {

            try
            {

                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(
                                                        NetworkingNodeId,
                                                        XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                           FirmwareStatusExtensions.Parse),
                                                        RequestId: RequestId
                                                    );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                FirmwareStatusNotificationRequest  = null;
                ErrorResponse                      = "The given XML representation of a firmware status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out FirmwareStatusNotificationRequest, out ErrorResponse)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       Request_Id                              RequestId,
                                       NetworkingNode_Id                       NetworkingNodeId,
                                       NetworkPath                             NetworkPath,
                                       out FirmwareStatusNotificationRequest?  FirmwareStatusNotificationRequest,
                                       out String?                             ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out FirmwareStatusNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The unique identification of the sending charge point/networking node.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">An optional delegate to parse custom FirmwareStatusNotification requests.</param>
        public static Boolean TryParse(JObject                                                          JSON,
                                       Request_Id                                                       RequestId,
                                       NetworkingNode_Id                                                NetworkingNodeId,
                                       NetworkPath                                                      NetworkPath,
                                       out FirmwareStatusNotificationRequest?                           FirmwareStatusNotificationRequest,
                                       out String?                                                      ErrorResponse,
                                       CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestParser)
        {

            try
            {

                FirmwareStatusNotificationRequest = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "firmware status",
                                       FirmwareStatusExtensions.Parse,
                                       out FirmwareStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(

                                                        NetworkingNodeId,
                                                        Status,

                                                        null,
                                                        null,
                                                        Signatures,

                                                        CustomData,

                                                        RequestId,
                                                        null,
                                                        null,
                                                        null,
                                                        NetworkPath

                                                    );

                if (CustomFirmwareStatusNotificationRequestParser is not null)
                    FirmwareStatusNotificationRequest = CustomFirmwareStatusNotificationRequestParser(JSON,
                                                                                                      FirmwareStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                FirmwareStatusNotificationRequest  = null;
                ErrorResponse                      = "The given JSON representation of a firmware status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationRequest",
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomFirmwareStatusNotificationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom firmware status notification requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?  CustomFirmwareStatusNotificationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFirmwareStatusNotificationRequestSerializer is not null
                       ? CustomFirmwareStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest1,
                                           FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (FirmwareStatusNotificationRequest1 is null || FirmwareStatusNotificationRequest2 is null)
                return false;

            return FirmwareStatusNotificationRequest1.Equals(FirmwareStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest1,
                                           FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest2)

            => !(FirmwareStatusNotificationRequest1 == FirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="Object">A firmware status notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FirmwareStatusNotificationRequest firmwareStatusNotificationRequest &&
                   Equals(firmwareStatusNotificationRequest);


        #endregion

        #region Equals(FirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request to compare with.</param>
        public override Boolean Equals(FirmwareStatusNotificationRequest? FirmwareStatusNotificationRequest)

            => FirmwareStatusNotificationRequest is not null &&

               Status.     Equals(FirmwareStatusNotificationRequest.Status) &&

               base.GenericEquals(FirmwareStatusNotificationRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.GetHashCode() * 3 ^
                       base.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
