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
    /// A clear charging profile response.
    /// </summary>
    public class ClearChargingProfileResponse : AResponse<CS.ClearChargingProfileRequest,
                                                             ClearChargingProfileResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear charging profile command.
        /// </summary>
        public ClearChargingProfileStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ClearChargingProfileResponse(Request, Status)

        /// <summary>
        /// Create a new clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        public ClearChargingProfileResponse(CS.ClearChargingProfileRequest  Request,
                                            ClearChargingProfileStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ClearChargingProfileResponse(Request, Result)

        /// <summary>
        /// Create a new clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearChargingProfileResponse(CS.ClearChargingProfileRequest  Request,
                                            Result                          Result)

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
        //       <ns:clearChargingProfileResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:clearChargingProfileResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static ClearChargingProfileResponse Parse(CS.ClearChargingProfileRequest  Request,
                                                         XElement                        XML)
        {

            if (TryParse(Request,
                         XML,
                         out var clearChargingProfileResponse,
                         out var errorResponse))
            {
                return clearChargingProfileResponse!;
            }

            throw new ArgumentException("The given XML representation of a clear charging profile response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomClearChargingProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearChargingProfileResponseParser">A delegate to parse custom clear charging profile responses.</param>
        public static ClearChargingProfileResponse Parse(CS.ClearChargingProfileRequest                              Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var clearChargingProfileResponse,
                         out var errorResponse,
                         CustomClearChargingProfileResponseParser))
            {
                return clearChargingProfileResponse!;
            }

            throw new ArgumentException("The given JSON representation of a clear charging profile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out ClearChargingProfileResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.ClearChargingProfileRequest     Request,
                                       XElement                           XML,
                                       out ClearChargingProfileResponse?  ClearChargingProfileResponse,
                                       out String?                        ErrorResponse)
        {

            try
            {

                ClearChargingProfileResponse = new ClearChargingProfileResponse(

                                                   Request,

                                                   XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                      ClearChargingProfileStatusExtentions.Parse)

                                               );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileResponse  = null;
                ErrorResponse                 = "The given JSON representation of a clear charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearChargingProfileResponse, out ErrorResponse, CustomClearChargingProfileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileResponseParser">A delegate to parse custom clear charging profile responses.</param>
        public static Boolean TryParse(CS.ClearChargingProfileRequest                              Request,
                                       JObject                                                     JSON,
                                       out ClearChargingProfileResponse?                           ClearChargingProfileResponse,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseParser   = null)
        {

            try
            {

                ClearChargingProfileResponse = null;

                #region ClearChargingProfileStatus

                if (!JSON.MapMandatory("status",
                                       "clear charging profile status",
                                       ClearChargingProfileStatusExtentions.Parse,
                                       out ClearChargingProfileStatus ClearChargingProfileStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ClearChargingProfileResponse = new ClearChargingProfileResponse(Request,
                                                                                ClearChargingProfileStatus);

                if (CustomClearChargingProfileResponseParser is not null)
                    ClearChargingProfileResponse = CustomClearChargingProfileResponseParser(JSON,
                                                                                            ClearChargingProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileResponse  = null;
                ErrorResponse                 = "The given JSON representation of a clear charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "clearChargingProfileResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomClearChargingProfileResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileResponseSerializer">A delegate to serialize custom clear charging profile responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomClearChargingProfileResponseSerializer is not null
                       ? CustomClearChargingProfileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear charging profile command failed.
        /// </summary>
        /// <param name="Request">The clear charging profile request leading to this response.</param>
        public static ClearChargingProfileResponse Failed(CS.ClearChargingProfileRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileResponse? ClearChargingProfileResponse1,
                                           ClearChargingProfileResponse? ClearChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileResponse1, ClearChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearChargingProfileResponse1 is null || ClearChargingProfileResponse2 is null)
                return false;

            return ClearChargingProfileResponse1.Equals(ClearChargingProfileResponse2);

        }

        #endregion

        #region Operator != (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileResponse? ClearChargingProfileResponse1,
                                           ClearChargingProfileResponse? ClearChargingProfileResponse2)

            => !(ClearChargingProfileResponse1 == ClearChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="Object">A clear charging profile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileResponse clearChargingProfileResponse &&
                   Equals(clearChargingProfileResponse);

        #endregion

        #region Equals(ClearChargingProfileResponse)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse">A clear charging profile response to compare with.</param>
        public override Boolean Equals(ClearChargingProfileResponse? ClearChargingProfileResponse)

            => ClearChargingProfileResponse is not null &&
                   Status.Equals(ClearChargingProfileResponse.Status);

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
