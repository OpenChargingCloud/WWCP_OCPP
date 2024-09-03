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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A general OCPP result.
    /// </summary>
    public class Result : IEquatable<Result>,
                          WWCP.IResult
    {

        #region Properties

        /// <summary>
        /// The machine-readable result code.
        /// </summary>
        public ResultCode  ResultCode        { get; }

        /// <summary>
        /// The optional human-readable error description.
        /// </summary>
        public String?     Description       { get; }

        /// <summary>
        /// Optional error details.
        /// </summary>
        public JObject?    Details           { get; }

        /// <summary>
        /// The optional response message.
        /// </summary>
        public JObject?    Response          { get; }

        /// <summary>
        /// The optional binary response message.
        /// </summary>
        public Byte[]?     BinaryResponse    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic OCPP result.
        /// </summary>
        /// <param name="ResultCode">The machine-readable result code.</param>
        /// <param name="Description">An optional human-readable error description.</param>
        /// <param name="Details">Optional error details.</param>
        /// <param name="Response">An optional response message.</param>
        /// <param name="BinaryResponse">An optional binary response message.</param>
        private Result(ResultCode  ResultCode,
                       String?     Description      = null,
                       JObject?    Details          = null,
                       JObject?    Response         = null,
                       Byte[]?     BinaryResponse   = null)
        {

            this.ResultCode      = ResultCode;
            this.Description     = Description?.Trim();
            this.Details         = Details;
            this.Response        = Response;
            this.BinaryResponse  = BinaryResponse;

        }

        #endregion


        public static Result FromSendRequestState(SendRequestState SendRequestState)

            => new (
                   SendRequestState.JSONRequestErrorMessage?.ErrorCode ?? ResultCode.GenericError,
                   SendRequestState.JSONRequestErrorMessage?.ErrorDescription,
                   SendRequestState.JSONRequestErrorMessage?.ErrorDetails,
                   SendRequestState.JSONResponse?.           Payload,
                   SendRequestState.BinaryResponse?.         Payload
               );

        public static Result FromErrorResponse(ResultCode  ResultCode,
                                               String?     Description      = null,
                                               JObject?    Details          = null,
                                               JObject?    Response         = null,
                                               Byte[]?     BinaryResponse   = null)

            => new (
                   ResultCode,
                   Description,
                   Details,
                   Response,
                   BinaryResponse
               );


        #region Static Definitions

        /// <summary>
        /// Unknown result code.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result GenericError(String?  Description   = null,
                                          JObject? Details       = null)

            => new (ResultCode.GenericError,
                    Description,
                    Details);


        /// <summary>
        /// An exception occured.
        /// </summary>
        /// <param name="Exception">An exception.</param>
        public static Result FromException(Exception Exception)

            => new (
                   ResultCode.GenericError,
                   Exception.Message,
                   JSONObject.Create(
                       new JProperty("message",     Exception.Message),
                       new JProperty("stackTrace",  Exception.StackTrace)
                   )
               );


        /// <summary>
        /// Data accepted and processed.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result OK(String? Description = null)

            => new (ResultCode.OK,
                    Description);


        ///// <summary>
        ///// Only part of the data was accepted.
        ///// </summary>
        ///// <param name="Description">A human-readable error description.</param>
        //public static Result Partly(String? Description = null)

        //    => new (ResultCodes.Partly,
        //            Description);


        ///// <summary>
        ///// Wrong username and/or password.
        ///// </summary>
        ///// <param name="Description">A human-readable error description.</param>
        //public static Result NotAuthorized(String? Description = null)

        //    => new (ResultCodes.NotAuthorized,
        //            Description);


        ///// <summary>
        ///// One or more ID (EVSE/Contract) were not valid for this user.
        ///// </summary>
        ///// <param name="Description">A human-readable error description.</param>
        //public static Result InvalidId(String? Description = null)

        //    => new (ResultCodes.InvalidId,
        //            Description);


        /// <summary>
        /// Internal server error.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result Server(String? Description = null)

            => new (ResultCode.NetworkError,
                    Description);

        /// <summary>
        /// Unknown or unreachable networking node.
        /// </summary>
        /// <param name="DestinationId">The unique identification of the unknown or unreachable networking node.</param>
        public static Result UnknownOrUnreachable(NetworkingNode_Id DestinationId)

            => new (ResultCode.NetworkError,
                    $"Unknown or unreachable networking node '{DestinationId}'!");


        /// <summary>
        /// Data has technical errors.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result FormationViolation(String? Description = null)

            => new (ResultCode.FormationViolation,
                    Description);


        /// <summary>
        /// The message could not be signed cryptographically.
        /// </summary>
        /// <param name="Description">A human-readable error description.</param>
        public static Result SignatureError(String? Description = null)

            => new (ResultCode.SecurityError,
                    Description);

        /// <summary>
        /// The message was filtered by administrative filter rules.
        /// </summary>
        /// <param name="Description">A human-readable description.</param>
        public static Result Filtered(String? Description = null)

            => new (ResultCode.Filtered,
                    Description);

        #endregion


        #region Operator overloading

        #region Operator == (Result1, Result2)

        /// <summary>
        /// Compares two results for equality.
        /// </summary>
        /// <param name="Result1">A result.</param>
        /// <param name="Result2">Another result.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Result? Result1,
                                           Result? Result2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Result1, Result2))
                return true;

            // If one is null, but not both, return false.
            if (Result1 is null || Result2 is null)
                return false;

            return Result1.Equals(Result2);

        }

        #endregion

        #region Operator != (Result1, Result2)

        /// <summary>
        /// Compares two results for inequality.
        /// </summary>
        /// <param name="Result1">A result.</param>
        /// <param name="Result2">Another result.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Result? Result1,
                                           Result? Result2)

            => !(Result1 == Result2);

        #endregion

        #endregion

        #region IEquatable<Result> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two results for equality.
        /// </summary>
        /// <param name="Object">A result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Result result &&
                   Equals(result);

        #endregion

        #region Equals(Result)

        /// <summary>
        /// Compares two results for equality.
        /// </summary>
        /// <param name="Result">A result to compare with.</param>
        public Boolean Equals(Result? Result)

            => Result is not null &&

               ResultCode. Equals(Result.ResultCode) &&

             ((Description is     null && Result.Description is     null) ||
              (Description is not null && Result.Description is not null && String.Equals(Description, Result.Description, StringComparison.OrdinalIgnoreCase)));

        #endregion

        #region Equals(IResult)

        /// <summary>
        /// Compares two results for equality.
        /// </summary>
        /// <param name="Result">A result to compare with.</param>
        public Boolean Equals(WWCP.IResult? Result)

            => Result is not null &&

               //ResultCode. Equals(Result.ResultCode) &&

             ((Description is     null && Result.Description is     null) ||
              (Description is not null && Result.Description is not null && String.Equals(Description, Result.Description, StringComparison.OrdinalIgnoreCase)));

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

                return ResultCode.            GetHashCode() * 3 ^

                       Description?.ToLower().GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ResultCode +
              (Description.IsNotNullOrEmpty() ? " - " + Description : "");

        #endregion

    }

}
