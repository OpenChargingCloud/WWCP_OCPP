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
    /// An update firmware response.
    /// </summary>
    public class UpdateFirmwareResponse : AResponse<CS.UpdateFirmwareRequest,
                                                       UpdateFirmwareResponse>
    {

        #region Constructor(s)

        #region UpdateFirmwareResponse(Request)

        /// <summary>
        /// Create a new update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        public UpdateFirmwareResponse(CS.UpdateFirmwareRequest  Request)

            : base(Request,
                   Result.OK())

        { }

        #endregion

        #region UpdateFirmwareResponse(Request, Result)

        /// <summary>
        /// Create a new update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UpdateFirmwareResponse(CS.UpdateFirmwareRequest  Request,
                                      Result                    Result)

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
        //
        //       <ns:updateFirmwareResponse />
        //
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UpdateFirmwareResponse",
        //     "title":   "UpdateFirmwareResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of an update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static UpdateFirmwareResponse Parse(CS.UpdateFirmwareRequest  Request,
                                                   XElement                  XML)
        {

            if (TryParse(Request,
                         XML,
                         out var updateFirmwareResponse,
                         out var errorResponse))
            {
                return updateFirmwareResponse!;
            }

            throw new ArgumentException("The given XML representation of a update firmware response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUpdateFirmwareResponseParser">A delegate to parse custom update firmware responses.</param>
        public static UpdateFirmwareResponse Parse(CS.UpdateFirmwareRequest                              Request,
                                                   JObject                                               JSON,
                                                   CustomJObjectParserDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var updateFirmwareResponse,
                         out var errorResponse,
                         CustomUpdateFirmwareResponseParser))
            {
                return updateFirmwareResponse!;
            }

            throw new ArgumentException("The given JSON representation of a update firmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out UpdateFirmwareResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="UpdateFirmwareResponse">The parsed update firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.UpdateFirmwareRequest     Request,
                                       XElement                     XML,
                                       out UpdateFirmwareResponse?  UpdateFirmwareResponse,
                                       out String?                  ErrorResponse)
        {

            try
            {

                UpdateFirmwareResponse = new UpdateFirmwareResponse(Request);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareResponse  = null;
                ErrorResponse           = "The given XML representation of an update firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UpdateFirmwareResponse, out ErrorResponse, CustomUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an update firmware response.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UpdateFirmwareResponse">The parsed update firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateFirmwareResponseParser">A delegate to parse custom update firmware responses.</param>
        public static Boolean TryParse(CS.UpdateFirmwareRequest                              Request,
                                       JObject                                               JSON,
                                       out UpdateFirmwareResponse?                           UpdateFirmwareResponse,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseParser   = null)
        {

            try
            {

                UpdateFirmwareResponse = new UpdateFirmwareResponse(Request);

                if (CustomUpdateFirmwareResponseParser is not null)
                    UpdateFirmwareResponse = CustomUpdateFirmwareResponseParser(JSON,
                                                                                UpdateFirmwareResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareResponse  = null;
                ErrorResponse           = "The given JSON representation of an update firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "updateFirmwareResponse");

        #endregion

        #region ToJSON(CustomUpdateFirmwareResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateFirmwareResponseSerializer">A delegate to serialize custom update firmware responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?          CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create();

            return CustomUpdateFirmwareResponseSerializer is not null
                       ? CustomUpdateFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The update firmware command failed.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        public static UpdateFirmwareResponse Failed(CS.UpdateFirmwareRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">An update firmware response.</param>
        /// <param name="UpdateFirmwareResponse2">Another update firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareResponse? UpdateFirmwareResponse1,
                                           UpdateFirmwareResponse? UpdateFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareResponse1, UpdateFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateFirmwareResponse1 is null || UpdateFirmwareResponse2 is null)
                return false;

            return UpdateFirmwareResponse1.Equals(UpdateFirmwareResponse2);

        }

        #endregion

        #region Operator != (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two update firmware responses for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">An update firmware response.</param>
        /// <param name="UpdateFirmwareResponse2">Another update firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateFirmwareResponse? UpdateFirmwareResponse1,
                                           UpdateFirmwareResponse? UpdateFirmwareResponse2)

            => !(UpdateFirmwareResponse1 == UpdateFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="Object">An update firmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateFirmwareResponse updateFirmwareResponse &&
                   Equals(updateFirmwareResponse);

        #endregion

        #region Equals(UpdateFirmwareResponse)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse">An update firmware response to compare with.</param>
        public override Boolean Equals(UpdateFirmwareResponse? UpdateFirmwareResponse)

            => UpdateFirmwareResponse is not null;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "UpdateFirmwareResponse";

        #endregion

    }

}
