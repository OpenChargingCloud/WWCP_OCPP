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
    /// A notify allowed energy transfer response.
    /// </summary>
    public class NotifyAllowedEnergyTransferResponse : AResponse<CSMS.NotifyAllowedEnergyTransferRequest,
                                                                      NotifyAllowedEnergyTransferResponse>
    {

        #region Properties

        /// <summary>
        /// The charging station will indicate if it was able to process the request.
        /// </summary>
        [Mandatory]
        public NotifyAllowedEnergyTransferStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                        StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region NotifyAllowedEnergyTransferResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new notify allowed energy transfer response.
        /// </summary>
        /// <param name="Request">The notify allowed energy transfer request leading to this response.</param>
        /// <param name="Status">The charging station will indicate if it was able to process the request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyAllowedEnergyTransferResponse(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                   NotifyAllowedEnergyTransferStatus        Status,
                                                   StatusInfo?                              StatusInfo   = null,
                                                   CustomData?                              CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region NotifyAllowedEnergyTransferResponse(Request, Result)

        /// <summary>
        /// Create a new notify allowed energy transfer response.
        /// </summary>
        /// <param name="Request">The notify allowed energy transfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyAllowedEnergyTransferResponse(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                   Result                                   Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyAllowedEnergyTransferResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify allowed energy transfer response.
        /// </summary>
        /// <param name="Request">The notify allowed energy transfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferResponseParser">A delegate to parse custom notify allowed energy transfer responses.</param>
        public static NotifyAllowedEnergyTransferResponse Parse(CSMS.NotifyAllowedEnergyTransferRequest                            Request,
                                                                JObject                                                            JSON,
                                                                CustomJObjectParserDelegate<NotifyAllowedEnergyTransferResponse>?  CustomNotifyAllowedEnergyTransferResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyAllowedEnergyTransferResponse,
                         out var errorResponse,
                         CustomNotifyAllowedEnergyTransferResponseParser))
            {
                return notifyAllowedEnergyTransferResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify allowed energy transfer response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyAllowedEnergyTransferResponse, out ErrorResponse, CustomNotifyAllowedEnergyTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify allowed energy transfer response.
        /// </summary>
        /// <param name="Request">The notify allowed energy transfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyAllowedEnergyTransferResponse">The parsed notify allowed energy transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferResponseParser">A delegate to parse custom notify allowed energy transfer responses.</param>
        public static Boolean TryParse(CSMS.NotifyAllowedEnergyTransferRequest                            Request,
                                       JObject                                                            JSON,
                                       out NotifyAllowedEnergyTransferResponse?                           NotifyAllowedEnergyTransferResponse,
                                       out String?                                                        ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyAllowedEnergyTransferResponse>?  CustomNotifyAllowedEnergyTransferResponseParser   = null)
        {

            try
            {

                NotifyAllowedEnergyTransferResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "notify allowed energy transfer status",
                                         NotifyAllowedEnergyTransferStatusExtensions.TryParse,
                                         out NotifyAllowedEnergyTransferStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

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


                NotifyAllowedEnergyTransferResponse = new NotifyAllowedEnergyTransferResponse(
                                                          Request,
                                                          Status,
                                                          StatusInfo,
                                                          CustomData
                                                      );

                if (CustomNotifyAllowedEnergyTransferResponseParser is not null)
                    NotifyAllowedEnergyTransferResponse = CustomNotifyAllowedEnergyTransferResponseParser(JSON,
                                                                                                          NotifyAllowedEnergyTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyAllowedEnergyTransferResponse  = null;
                ErrorResponse                        = "The given JSON representation of a notify allowed energy transfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyAllowedEnergyTransferResponseSerializer = null, CustomCompositeScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyAllowedEnergyTransferResponseSerializer">A delegate to serialize custom notify allowed energy transfer responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferResponse>?  CustomNotifyAllowedEnergyTransferResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                           CustomStatusInfoSerializer                            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                           CustomCustomDataSerializer                            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyAllowedEnergyTransferResponseSerializer is not null
                       ? CustomNotifyAllowedEnergyTransferResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify allowed energy transfer request failed.
        /// </summary>
        /// <param name="Request">The notify allowed energy transfer request leading to this response.</param>
        public static NotifyAllowedEnergyTransferResponse Failed(CSMS.NotifyAllowedEnergyTransferRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyAllowedEnergyTransferResponse1, NotifyAllowedEnergyTransferResponse2)

        /// <summary>
        /// Compares two notify allowed energy transfer responses for equality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferResponse1">A notify allowed energy transfer response.</param>
        /// <param name="NotifyAllowedEnergyTransferResponse2">Another notify allowed energy transfer response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse1,
                                           NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyAllowedEnergyTransferResponse1, NotifyAllowedEnergyTransferResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyAllowedEnergyTransferResponse1 is null || NotifyAllowedEnergyTransferResponse2 is null)
                return false;

            return NotifyAllowedEnergyTransferResponse1.Equals(NotifyAllowedEnergyTransferResponse2);

        }

        #endregion

        #region Operator != (NotifyAllowedEnergyTransferResponse1, NotifyAllowedEnergyTransferResponse2)

        /// <summary>
        /// Compares two notify allowed energy transfer responses for inequality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferResponse1">A notify allowed energy transfer response.</param>
        /// <param name="NotifyAllowedEnergyTransferResponse2">Another notify allowed energy transfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse1,
                                           NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse2)

            => !(NotifyAllowedEnergyTransferResponse1 == NotifyAllowedEnergyTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyAllowedEnergyTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify allowed energy transfer responses for equality.
        /// </summary>
        /// <param name="Object">A notify allowed energy transfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyAllowedEnergyTransferResponse notifyAllowedEnergyTransferResponse &&
                   Equals(notifyAllowedEnergyTransferResponse);

        #endregion

        #region Equals(NotifyAllowedEnergyTransferResponse)

        /// <summary>
        /// Compares two notify allowed energy transfer responses for equality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferResponse">A notify allowed energy transfer response to compare with.</param>
        public override Boolean Equals(NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse)

            => NotifyAllowedEnergyTransferResponse is not null &&

               Status.     Equals(NotifyAllowedEnergyTransferResponse.Status) &&

             ((StatusInfo is     null && NotifyAllowedEnergyTransferResponse.StatusInfo is     null) ||
               StatusInfo is not null && NotifyAllowedEnergyTransferResponse.StatusInfo is not null && StatusInfo.Equals(NotifyAllowedEnergyTransferResponse.StatusInfo)) &&

               base.GenericEquals(NotifyAllowedEnergyTransferResponse);

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
