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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A get log response.
    /// </summary>
    public class GetLogResponse : AResponse<CSMS.GetLogRequest,
                                            GetLogResponse>,
                                  IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getLogResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the get log command.
        /// </summary>
        [Mandatory]
        public LogStatus      Status        { get; }

        /// <summary>
        /// The name of the log file that will be uploaded.
        /// This field is not present when no logging information is available.
        /// </summary>
        [Optional]
        public String?        Filename      { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region GetLogResponse(Request, Status, Filename = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new get log response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Status">The success or failure of the get log command.</param>
        /// <param name="Filename">The name of the log file that will be uploaded. This field is not present when no logging information is available.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetLogResponse(CSMS.GetLogRequest       Request,
                              LogStatus                Status,
                              String?                  Filename            = null,
                              StatusInfo?              StatusInfo          = null,
                              DateTime?                ResponseTimestamp   = null,

                              IEnumerable<KeyPair>?    SignKeys            = null,
                              IEnumerable<SignInfo>?   SignInfos           = null,
                              IEnumerable<Signature>?  Signatures          = null,

                              CustomData?              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.Filename    = Filename;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetLogResponse(Request, Result)

        /// <summary>
        /// Create a new get log response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetLogResponse(CSMS.GetLogRequest  Request,
                              Result              Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:GetLog.conf",
        //   "definitions": {
        //     "LogStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "AcceptedCanceled"
        //       ]
        //     }
        // },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //         "$ref": "#/definitions/LogStatusEnumType"
        //     },
        //     "filename": {
        //         "type": "string",
        //       "maxLength": 255
        //     }
        // },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetLogResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get log response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetLogResponseParser">A delegate to parse custom get log responses.</param>
        public static GetLogResponse Parse(CSMS.GetLogRequest                            Request,
                                           JObject                                       JSON,
                                           CustomJObjectParserDelegate<GetLogResponse>?  CustomGetLogResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getLogResponse,
                         out var errorResponse,
                         CustomGetLogResponseParser))
            {
                return getLogResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get log response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetLogResponse, out ErrorResponse, CustomGetLogResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get log response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetLogResponse">The parsed get log response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLogResponseParser">A delegate to parse custom get log responses.</param>
        public static Boolean TryParse(CSMS.GetLogRequest                            Request,
                                       JObject                                       JSON,
                                       out GetLogResponse?                           GetLogResponse,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<GetLogResponse>?  CustomGetLogResponseParser   = null)
        {

            try
            {

                GetLogResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "get log status",
                                         LogStatusExtensions.TryParse,
                                         out LogStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Filename      [optional]

                var Filename = JSON.GetOptional("filename");

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetLogResponse = new GetLogResponse(
                                     Request,
                                     Status,
                                     Filename,
                                     StatusInfo,
                                     null,
                                     null,
                                     null,
                                     Signatures,
                                     CustomData
                                 );

                if (CustomGetLogResponseParser is not null)
                    GetLogResponse = CustomGetLogResponseParser(JSON,
                                                                GetLogResponse);

                return true;

            }
            catch (Exception e)
            {
                GetLogResponse  = null;
                ErrorResponse   = "The given JSON representation of a get log response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLogResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLogResponseSerializer">A delegate to serialize custom get log responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLogResponse>?  CustomGetLogResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?      CustomStatusInfoSerializer       = null,
                              CustomJObjectSerializerDelegate<Signature>?       CustomSignatureSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.AsText()),

                           Filename.IsNotNullOrEmpty()
                               ? new JProperty("filename",     Filename)
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetLogResponseSerializer is not null
                       ? CustomGetLogResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get log command failed.
        /// </summary>
        /// <param name="Request">The get log request leading to this response.</param>
        public static GetLogResponse Failed(CSMS.GetLogRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetLogResponse1, GetLogResponse2)

        /// <summary>
        /// Compares two get log responses for equality.
        /// </summary>
        /// <param name="GetLogResponse1">A get log response.</param>
        /// <param name="GetLogResponse2">Another get log response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLogResponse? GetLogResponse1,
                                           GetLogResponse? GetLogResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLogResponse1, GetLogResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetLogResponse1 is null || GetLogResponse2 is null)
                return false;

            return GetLogResponse1.Equals(GetLogResponse2);

        }

        #endregion

        #region Operator != (GetLogResponse1, GetLogResponse2)

        /// <summary>
        /// Compares two get log responses for inequality.
        /// </summary>
        /// <param name="GetLogResponse1">A get log response.</param>
        /// <param name="GetLogResponse2">Another get log response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLogResponse? GetLogResponse1,
                                           GetLogResponse? GetLogResponse2)

            => !(GetLogResponse1 == GetLogResponse2);

        #endregion

        #endregion

        #region IEquatable<GetLogResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get log responses for equality.
        /// </summary>
        /// <param name="Object">A get log response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLogResponse getLogResponse &&
                   Equals(getLogResponse);

        #endregion

        #region Equals(GetLogResponse)

        /// <summary>
        /// Compares two get log responses for equality.
        /// </summary>
        /// <param name="GetLogResponse">A get log response to compare with.</param>
        public override Boolean Equals(GetLogResponse? GetLogResponse)

            => GetLogResponse is not null &&

               Status.     Equals(GetLogResponse.Status) &&

             ((Filename   is     null && GetLogResponse.Filename   is     null) ||
              (Filename   is not null && GetLogResponse.Filename   is not null && Filename.  Equals(GetLogResponse.Filename)))  &&

             ((StatusInfo is     null && GetLogResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetLogResponse.StatusInfo is not null && StatusInfo.Equals(GetLogResponse.StatusInfo)) &&

               base.GenericEquals(GetLogResponse);

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

                return Status.     GetHashCode()       * 7 ^
                      (Filename?.  GetHashCode() ?? 0) * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.GetHashCode();

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
