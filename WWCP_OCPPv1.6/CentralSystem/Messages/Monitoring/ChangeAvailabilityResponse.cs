/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A change availability response.
    /// </summary>
    public class ChangeAvailabilityResponse : AResponse<CS.ChangeAvailabilityRequest,
                                                           ChangeAvailabilityResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/changeAvailabilityResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the change availability command.
        /// </summary>
        public AvailabilityStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ChangeAvailabilityResponse(Request, Status)

        /// <summary>
        /// Create a new change availability response.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        /// <param name="Status">The success or failure of the change availability command.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChangeAvailabilityResponse(CS.ChangeAvailabilityRequest  Request,
                                          AvailabilityStatus            Status,

                                          DateTime?                     ResponseTimestamp   = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                          CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region ChangeAvailabilityResponse(Request, Result)

        /// <summary>
        /// Create a new change availability response.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ChangeAvailabilityResponse(CS.ChangeAvailabilityRequest  Request,
                                          Result                        Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:changeAvailabilityResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:changeAvailabilityResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeAvailabilityResponse",
        //     "title":   "ChangeAvailabilityResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "Scheduled"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a change availability response.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static ChangeAvailabilityResponse Parse(CS.ChangeAvailabilityRequest  Request,
                                                       XElement                      XML)
        {

            if (TryParse(Request,
                         XML,
                         out var changeAvailabilityResponse,
                         out var errorResponse))
            {
                return changeAvailabilityResponse!;
            }

            throw new ArgumentException("The given XML representation of a change availability response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomChangeAvailabilityResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a change availability response.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChangeAvailabilityResponseParser">A delegate to parse custom change availability responses.</param>
        public static ChangeAvailabilityResponse Parse(CS.ChangeAvailabilityRequest                              Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var changeAvailabilityResponse,
                         out var errorResponse,
                         CustomChangeAvailabilityResponseParser) &&
                changeAvailabilityResponse is not null)
            {
                return changeAvailabilityResponse;
            }

            throw new ArgumentException("The given JSON representation of a change availability response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out ChangeAvailabilityResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a change availability response.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed change availability response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.ChangeAvailabilityRequest     Request,
                                       XElement                         XML,
                                       out ChangeAvailabilityResponse?  ChangeAvailabilityResponse,
                                       out String?                      ErrorResponse)
        {

            try
            {

                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(

                                                 Request,

                                                 XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                    AvailabilityStatusExtensions.Parse)

                                             );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityResponse  = null;
                ErrorResponse               = "The given XML representation of a change availability response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ChangeAvailabilityResponse, out ErrorResponse, CustomChangeAvailabilityResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a change availability response.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed change availability response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChangeAvailabilityResponseParser">A delegate to parse custom change availability responses.</param>
        public static Boolean TryParse(CS.ChangeAvailabilityRequest                              Request,
                                       JObject                                                   JSON,
                                       out ChangeAvailabilityResponse?                           ChangeAvailabilityResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseParser   = null)
        {

            try
            {

                ChangeAvailabilityResponse = null;

                #region AvailabilityStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "availability status",
                                       AvailabilityStatusExtensions.Parse,
                                       out AvailabilityStatus AvailabilityStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData            [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(

                                                 Request,
                                                 AvailabilityStatus,
                                                 null,

                                                 null,
                                                 null,
                                                 Signatures,

                                                 CustomData

                                             );

                if (CustomChangeAvailabilityResponseParser is not null)
                    ChangeAvailabilityResponse = CustomChangeAvailabilityResponseParser(JSON,
                                                                                        ChangeAvailabilityResponse);

                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityResponse  = null;
                ErrorResponse               = "The given JSON representation of a change availability response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "changeAvailabilityResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabilityResponseSerializer">A delegate to serialize custom change availability responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?              CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomChangeAvailabilityResponseSerializer is not null
                       ? CustomChangeAvailabilityResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The change availability command failed.
        /// </summary>
        /// <param name="Request">The change availability request leading to this response.</param>
        public static ChangeAvailabilityResponse Failed(CS.ChangeAvailabilityRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two change availability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A change availability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another change availability response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityResponse? ChangeAvailabilityResponse1,
                                           ChangeAvailabilityResponse? ChangeAvailabilityResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityResponse1, ChangeAvailabilityResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeAvailabilityResponse1 is null || ChangeAvailabilityResponse2 is null)
                return false;

            return ChangeAvailabilityResponse1.Equals(ChangeAvailabilityResponse2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two change availability responses for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A change availability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another change availability response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityResponse? ChangeAvailabilityResponse1,
                                           ChangeAvailabilityResponse? ChangeAvailabilityResponse2)

            => !(ChangeAvailabilityResponse1 == ChangeAvailabilityResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two change availability responses for equality.
        /// </summary>
        /// <param name="Object">A change availability response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeAvailabilityResponse changeAvailabilityResponse &&
                   Equals(changeAvailabilityResponse);

        #endregion

        #region Equals(ChangeAvailabilityResponse)

        /// <summary>
        /// Compares two change availability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse">A change availability response to compare with.</param>
        public override Boolean Equals(ChangeAvailabilityResponse? ChangeAvailabilityResponse)

            => ChangeAvailabilityResponse is not null &&
                   Status.Equals(ChangeAvailabilityResponse.Status);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

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
