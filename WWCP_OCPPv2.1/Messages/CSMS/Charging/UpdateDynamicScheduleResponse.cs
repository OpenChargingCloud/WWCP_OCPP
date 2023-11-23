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
    /// An update dynamic schedule response.
    /// </summary>
    public class UpdateDynamicScheduleResponse : AResponse<CSMS.UpdateDynamicScheduleRequest,
                                                                UpdateDynamicScheduleResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/updateDynamicScheduleResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the update.
        /// </summary>
        [Mandatory]
        public ChargingProfileStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?            StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region UpdateDynamicScheduleResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new update dynamic schedule response.
        /// </summary>
        /// <param name="Request">The update dynamic schedule request leading to this response.</param>
        /// <param name="Status">The success or failure of the update.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UpdateDynamicScheduleResponse(CSMS.UpdateDynamicScheduleRequest  Request,
                                             ChargingProfileStatus              Status,
                                             StatusInfo?                        StatusInfo          = null,
                                             DateTime?                          ResponseTimestamp   = null,

                                             IEnumerable<KeyPair>?              SignKeys            = null,
                                             IEnumerable<SignInfo>?             SignInfos           = null,
                                             IEnumerable<Signature>?            Signatures          = null,

                                             CustomData?                        CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region UpdateDynamicScheduleResponse(Request, Result)

        /// <summary>
        /// Create a new update dynamic schedule response.
        /// </summary>
        /// <param name="Request">The update dynamic schedule request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UpdateDynamicScheduleResponse(CSMS.UpdateDynamicScheduleRequest  Request,
                                             Result                             Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomUpdateDynamicScheduleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an update dynamic schedule response.
        /// </summary>
        /// <param name="Request">The update dynamic schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUpdateDynamicScheduleResponseParser">A delegate to parse custom update dynamic schedule responses.</param>
        public static UpdateDynamicScheduleResponse Parse(CSMS.UpdateDynamicScheduleRequest                            Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<UpdateDynamicScheduleResponse>?  CustomUpdateDynamicScheduleResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var updateDynamicScheduleResponse,
                         out var errorResponse,
                         CustomUpdateDynamicScheduleResponseParser) &&
                updateDynamicScheduleResponse is not null)
            {
                return updateDynamicScheduleResponse;
            }

            throw new ArgumentException("The given JSON representation of an update dynamic schedule response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UpdateDynamicScheduleResponse, out ErrorResponse, CustomUpdateDynamicScheduleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an update dynamic schedule response.
        /// </summary>
        /// <param name="Request">The update dynamic schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UpdateDynamicScheduleResponse">The parsed update dynamic schedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateDynamicScheduleResponseParser">A delegate to parse custom update dynamic schedule responses.</param>
        public static Boolean TryParse(CSMS.UpdateDynamicScheduleRequest                            Request,
                                       JObject                                                      JSON,
                                       out UpdateDynamicScheduleResponse?                           UpdateDynamicScheduleResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateDynamicScheduleResponse>?  CustomUpdateDynamicScheduleResponseParser   = null)
        {

            try
            {

                UpdateDynamicScheduleResponse = null;

                #region ChargingProfileStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "charging profile status",
                                         ChargingProfileStatusExtensions.TryParse,
                                         out ChargingProfileStatus ChargingProfileStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo               [optional]

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

                #region Signatures               [optional, OCPP_CSE]

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

                #region CustomData               [optional]

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


                UpdateDynamicScheduleResponse = new UpdateDynamicScheduleResponse(
                                                    Request,
                                                    ChargingProfileStatus,
                                                    StatusInfo,
                                                    null,
                                                    null,
                                                    null,
                                                    Signatures,
                                                    CustomData
                                                );

                if (CustomUpdateDynamicScheduleResponseParser is not null)
                    UpdateDynamicScheduleResponse = CustomUpdateDynamicScheduleResponseParser(JSON,
                                                                                              UpdateDynamicScheduleResponse);

                return true;

            }
            catch (Exception e)
            {
                UpdateDynamicScheduleResponse  = null;
                ErrorResponse                  = "The given JSON representation of an update dynamic schedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateDynamicScheduleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateDynamicScheduleResponseSerializer">A delegate to serialize custom update dynamic schedule responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateDynamicScheduleResponse>?  CustomUpdateDynamicScheduleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

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

            return CustomUpdateDynamicScheduleResponseSerializer is not null
                       ? CustomUpdateDynamicScheduleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The update dynamic schedule failed.
        /// </summary>
        /// <param name="Request">The update dynamic schedule request leading to this response.</param>
        public static UpdateDynamicScheduleResponse Failed(CSMS.UpdateDynamicScheduleRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UpdateDynamicScheduleResponse1, UpdateDynamicScheduleResponse2)

        /// <summary>
        /// Compares two update dynamic schedule responses for equality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleResponse1">An update dynamic schedule response.</param>
        /// <param name="UpdateDynamicScheduleResponse2">Another update dynamic schedule response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateDynamicScheduleResponse? UpdateDynamicScheduleResponse1,
                                           UpdateDynamicScheduleResponse? UpdateDynamicScheduleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateDynamicScheduleResponse1, UpdateDynamicScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateDynamicScheduleResponse1 is null || UpdateDynamicScheduleResponse2 is null)
                return false;

            return UpdateDynamicScheduleResponse1.Equals(UpdateDynamicScheduleResponse2);

        }

        #endregion

        #region Operator != (UpdateDynamicScheduleResponse1, UpdateDynamicScheduleResponse2)

        /// <summary>
        /// Compares two update dynamic schedule responses for inequality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleResponse1">An update dynamic schedule response.</param>
        /// <param name="UpdateDynamicScheduleResponse2">Another update dynamic schedule response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateDynamicScheduleResponse? UpdateDynamicScheduleResponse1,
                                           UpdateDynamicScheduleResponse? UpdateDynamicScheduleResponse2)

            => !(UpdateDynamicScheduleResponse1 == UpdateDynamicScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateDynamicScheduleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two update dynamic schedule responses for equality.
        /// </summary>
        /// <param name="Object">An update dynamic schedule response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateDynamicScheduleResponse updateDynamicScheduleResponse &&
                   Equals(updateDynamicScheduleResponse);

        #endregion

        #region Equals(UpdateDynamicScheduleResponse)

        /// <summary>
        /// Compares two update dynamic schedule responses for equality.
        /// </summary>
        /// <param name="UpdateDynamicScheduleResponse">An update dynamic schedule response to compare with.</param>
        public override Boolean Equals(UpdateDynamicScheduleResponse? UpdateDynamicScheduleResponse)

            => UpdateDynamicScheduleResponse is not null &&

               Status.     Equals(UpdateDynamicScheduleResponse.Status) &&

             ((StatusInfo is     null && UpdateDynamicScheduleResponse.StatusInfo is     null) ||
               StatusInfo is not null && UpdateDynamicScheduleResponse.StatusInfo is not null && StatusInfo.Equals(UpdateDynamicScheduleResponse.StatusInfo)) &&

               base.GenericEquals(UpdateDynamicScheduleResponse);

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

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.AsText();

        #endregion

    }

}
