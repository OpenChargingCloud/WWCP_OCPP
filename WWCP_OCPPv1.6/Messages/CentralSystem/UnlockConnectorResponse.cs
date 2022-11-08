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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A unlock connector response.
    /// </summary>
    public class UnlockConnectorResponse : AResponse<CS.UnlockConnectorRequest,
                                                        UnlockConnectorResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the unlock connector request.
        /// </summary>
        public UnlockStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region UnlockConnectorResponse(Request, Status)

        /// <summary>
        /// Create a new unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="Status">The success or failure of the unlock connector request.</param>
        public UnlockConnectorResponse(CS.UnlockConnectorRequest  Request,
                                       UnlockStatus               Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region UnlockConnectorResponse(Result)

        /// <summary>
        /// Create a new unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UnlockConnectorResponse(CS.UnlockConnectorRequest  Request,
                                       Result                     Result)

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
        //       <ns:unlockConnectorResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:unlockConnectorResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UnlockConnectorResponse",
        //     "title":   "UnlockConnectorResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Unlocked",
        //                 "UnlockFailed",
        //                 "NotSupported"
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
        /// Parse the given XML representation of an unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static UnlockConnectorResponse Parse(CS.UnlockConnectorRequest  Request,
                                                    XElement                   XML)
        {

            if (TryParse(Request,
                         XML,
                         out var unlockConnectorResponse,
                         out var errorResponse))
            {
                return unlockConnectorResponse!;
            }

            throw new ArgumentException("The given XML representation of an unlock connector response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomUnlockConnectorResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUnlockConnectorResponseParser">A delegate to parse custom unlock connector responses.</param>
        public static UnlockConnectorResponse Parse(CS.UnlockConnectorRequest                              Request,
                                                    JObject                                                JSON,
                                                    CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var unlockConnectorResponse,
                         out var errorResponse,
                         CustomUnlockConnectorResponseParser))
            {
                return unlockConnectorResponse!;
            }

            throw new ArgumentException("The given JSON representation of an unlock connector response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out UnlockConnectorResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.UnlockConnectorRequest     Request,
                                       XElement                      XML,
                                       out UnlockConnectorResponse?  UnlockConnectorResponse,
                                       out String?                   ErrorResponse)
        {

            try
            {

                UnlockConnectorResponse = new UnlockConnectorResponse(

                                              Request,

                                              XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                        UnlockStatusExtentions.Parse)

                                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorResponse  = null;
                ErrorResponse            = "The given XML representation of an unlock connector response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UnlockConnectorResponse, out ErrorResponse, CustomUnlockConnectorResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorResponseParser">A delegate to parse custom unlock connector responses.</param>
        public static Boolean TryParse(CS.UnlockConnectorRequest                              Request,
                                       JObject                                                JSON,
                                       out UnlockConnectorResponse?                           UnlockConnectorResponse,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null)
        {

            try
            {

                UnlockConnectorResponse = null;

                #region UnlockStatus

                if (!JSON.MapMandatory("status",
                                       "unlock status",
                                       UnlockStatusExtentions.Parse,
                                       out UnlockStatus UnlockStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                UnlockConnectorResponse = new UnlockConnectorResponse(Request,
                                                                      UnlockStatus);

                if (CustomUnlockConnectorResponseParser is not null)
                    UnlockConnectorResponse = CustomUnlockConnectorResponseParser(JSON,
                                                                                  UnlockConnectorResponse);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorResponse  = null;
                ErrorResponse            = "The given JSON representation of an unlock connector response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "unlockConnectorResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomUnlockConnectorResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorResponseSerializer">A delegate to serialize custom unlock connector responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorResponse>? CustomUnlockConnectorResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomUnlockConnectorResponseSerializer is not null
                       ? CustomUnlockConnectorResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The unlock connector command failed.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        public static UnlockConnectorResponse Failed(CS.UnlockConnectorRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">A unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorResponse UnlockConnectorResponse1,
                                           UnlockConnectorResponse UnlockConnectorResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorResponse1, UnlockConnectorResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorResponse1 is null || UnlockConnectorResponse2 is null)
                return false;

            return UnlockConnectorResponse1.Equals(UnlockConnectorResponse2);

        }

        #endregion

        #region Operator != (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for inequality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">A unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorResponse UnlockConnectorResponse1,
                                           UnlockConnectorResponse UnlockConnectorResponse2)

            => !(UnlockConnectorResponse1 == UnlockConnectorResponse2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">A unlock connector response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorResponse unlockConnectorResponse &&
                   Equals(unlockConnectorResponse);

        #endregion

        #region Equals(UnlockConnectorResponse)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">A unlock connector response to compare with.</param>
        public override Boolean Equals(UnlockConnectorResponse? UnlockConnectorResponse)

            => UnlockConnectorResponse is not null &&
                   Status.Equals(UnlockConnectorResponse.Status);

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
