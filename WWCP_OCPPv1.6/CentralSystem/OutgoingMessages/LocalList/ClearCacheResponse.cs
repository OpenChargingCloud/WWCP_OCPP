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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A clear cache response.
    /// </summary>
    public class ClearCacheResponse : AResponse<CS.ClearCacheRequest,
                                                   ClearCacheResponse>,
                                      IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/clearCacheResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the clear cache command.
        /// </summary>
        public ClearCacheStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ClearCacheResponse(Request, Status)

        /// <summary>
        /// Create a new clear cache response.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        /// <param name="Status">The success or failure of the clear cache command.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ClearCacheResponse(CS.ClearCacheRequest          Request,
                                  ClearCacheStatus              Status,

                                  DateTime?                     ResponseTimestamp   = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<OCPP.Signature>?  Signatures          = null,

                                  CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region ClearCacheResponse(Request, Result)

        /// <summary>
        /// Create a new clear cache response.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearCacheResponse(CS.ClearCacheRequest  Request,
                                  Result                Result)

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
        //       <ns:clearCacheResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:clearCacheResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static ClearCacheResponse Parse(CS.ClearCacheRequest  Request,
                                               XElement              XML)
        {

            if (TryParse(Request,
                         XML,
                         out var clearCacheResponse,
                         out var errorResponse) &&
                clearCacheResponse is not null)
            {
                return clearCacheResponse;
            }

            throw new ArgumentException("The given XML representation of a clear cache response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomClearCacheResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearCacheResponseParser">A delegate to parse custom clear cache responses.</param>
        public static ClearCacheResponse Parse(CS.ClearCacheRequest                              Request,
                                               JObject                                           JSON,
                                               CustomJObjectParserDelegate<ClearCacheResponse>?  CustomClearCacheResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var clearCacheResponse,
                         out var errorResponse,
                         CustomClearCacheResponseParser) &&
                clearCacheResponse is not null)
            {
                return clearCacheResponse;
            }

            throw new ArgumentException("The given JSON representation of a clear cache response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out ClearCacheResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.ClearCacheRequest     Request,
                                       XElement                 XML,
                                       out ClearCacheResponse?  ClearCacheResponse,
                                       out String?              ErrorResponse)
        {

            try
            {

                ClearCacheResponse = new ClearCacheResponse(

                                         Request,

                                         XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                            ClearCacheStatusExtensions.Parse)

                                     );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ClearCacheResponse  = null;
                ErrorResponse       = "The given XML representation of a clear cache response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearCacheResponse, out ErrorResponse, CustomClearCacheResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearCacheResponseParser">A delegate to parse custom clear cache responses.</param>
        public static Boolean TryParse(CS.ClearCacheRequest                              Request,
                                       JObject                                           JSON,
                                       out ClearCacheResponse?                           ClearCacheResponse,
                                       out String?                                       ErrorResponse,
                                       CustomJObjectParserDelegate<ClearCacheResponse>?  CustomClearCacheResponseParser   = null)
        {

            try
            {

                ClearCacheResponse = null;

                #region ClearCacheStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "clear cache status",
                                       ClearCacheStatusExtensions.Parse,
                                       out ClearCacheStatus ClearCacheStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures          [optional, OCPP_CSE]

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

                #region CustomData          [optional]

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


                ClearCacheResponse = new ClearCacheResponse(

                                         Request,
                                         ClearCacheStatus,
                                         null,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomClearCacheResponseParser is not null)
                    ClearCacheResponse = CustomClearCacheResponseParser(JSON,
                                                                        ClearCacheResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearCacheResponse  = null;
                ErrorResponse       = "The given JSON representation of a clear cache response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "clearCacheResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomClearCacheResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheResponseSerializer">A delegate to serialize custom clear cache responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheResponse>?  CustomClearCacheResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?      CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
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

            return CustomClearCacheResponseSerializer is not null
                       ? CustomClearCacheResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear cache command failed.
        /// </summary>
        /// <param name="Request">The clear cache request leading to this response.</param>
        public static ClearCacheResponse Failed(CS.ClearCacheRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two clear cache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A clear cache response.</param>
        /// <param name="ClearCacheResponse2">Another clear cache response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheResponse? ClearCacheResponse1,
                                           ClearCacheResponse? ClearCacheResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearCacheResponse1, ClearCacheResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearCacheResponse1 is null || ClearCacheResponse2 is null)
                return false;

            return ClearCacheResponse1.Equals(ClearCacheResponse2);

        }

        #endregion

        #region Operator != (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two clear cache responses for inequality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A clear cache response.</param>
        /// <param name="ClearCacheResponse2">Another clear cache response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheResponse? ClearCacheResponse1,
                                           ClearCacheResponse? ClearCacheResponse2)

            => !(ClearCacheResponse1 == ClearCacheResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear cache responses for equality.
        /// </summary>
        /// <param name="Object">A clear cache response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearCacheResponse clearCacheResponse &&
                   Equals(clearCacheResponse);

        #endregion

        #region Equals(ClearCacheResponse)

        /// <summary>
        /// Compares two clear cache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse">A clear cache response to compare with.</param>
        public override Boolean Equals(ClearCacheResponse? ClearCacheResponse)

            => ClearCacheResponse is not null &&
                   Status.Equals(ClearCacheResponse.Status);

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
