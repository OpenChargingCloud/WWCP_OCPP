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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for OCPP error codes.
    /// </summary>
    public static class OCPP_WebSocket_ErrorCodesExtensions
    {

        /// <summary>
        /// Indicates whether this OCPP WebSocket error code. is null or empty.
        /// </summary>
        /// <param name="errorCode">An OCPP WebSocket error code.</param>
        public static Boolean IsNullOrEmpty(this ResultCodes? errorCode)
            => !errorCode.HasValue || errorCode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this OCPP WebSocket error code. is null or empty.
        /// </summary>
        /// <param name="errorCode">An OCPP WebSocket error code.</param>
        public static Boolean IsNotNullOrEmpty(this ResultCodes? errorCode)
            => errorCode.HasValue && errorCode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An OCPP WebSocket error code.
    /// </summary>
    public readonly struct ResultCodes : IId,
                                         IEquatable<ResultCodes>,
                                         IComparable<ResultCodes>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the error code.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new error code based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the error code.</param>
        private ResultCodes(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an OCPP WebSocket error code.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP WebSocket error code.</param>
        public static ResultCodes Parse(String Text)
        {

            if (TryParse(Text, out var errorCode))
                return errorCode;

            throw new ArgumentException($"Invalid text representation of an OCPP WebSocket error code: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an OCPP WebSocket error code.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP WebSocket error code.</param>
        public static ResultCodes? TryParse(String Text)
        {

            if (TryParse(Text, out var errorCode))
                return errorCode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out errorCode)

        /// <summary>
        /// Try to parse the given text as an OCPP WebSocket error code.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP WebSocket error code.</param>
        /// <param name="errorCode">The parsed OCPP WebSocket error code.</param>
        public static Boolean TryParse(String Text, out ResultCodes errorCode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                errorCode = new ResultCodes(Text);
                return true;
            }

            errorCode = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this OCPP WebSocket error code.
        /// </summary>
        public ResultCodes Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        /// <summary>
        /// Requested Action is not known by receiver.
        /// </summary>
        public static readonly ResultCodes OK                              = new ("OK");

        /// <summary>
        /// Requested Action is not known by receiver.
        /// </summary>
        public static readonly ResultCodes NotImplemented                  = new ("NotImplemented");

        /// <summary>
        /// Requested Action is recognized but not supported by the receiver.
        /// </summary>
        public static readonly ResultCodes NotSupported                    = new ("NotSupported");

        /// <summary>
        /// An internal error occurred and the receiver was not able to process the requested Action successfully.
        /// </summary>
        public static readonly ResultCodes InternalError                   = new ("InternalError");

        /// <summary>
        /// Payload for Action is incomplete.
        /// </summary>
        public static readonly ResultCodes ProtocolError                   = new ("ProtocolError");

        /// <summary>
        /// During the processing of Action a security issue occurred preventing receiver from completing the Action successfully.
        /// </summary>
        public static readonly ResultCodes SecurityError                   = new ("SecurityError");

        /// <summary>
        /// Payload for Action is syntactically incorrect or not conform the PDU structure for Action.
        /// </summary>
        public static readonly ResultCodes FormationViolation              = new ("FormationViolation");

        /// <summary>
        /// Payload is syntactically correct but at least one field contains an invalid value.
        /// </summary>
        public static readonly ResultCodes PropertyConstraintViolation     = new ("PropertyConstraintViolation");

        /// <summary>
        /// Payload for Action is syntactically correct but at least one of the fields violates occurence constraints.
        /// </summary>
        public static readonly ResultCodes OccurenceConstraintViolation    = new ("OccurenceConstraintViolation");

        /// <summary>
        /// Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12).
        /// </summary>
        public static readonly ResultCodes TypeConstraintViolation         = new ("TypeConstraintViolation");

        /// <summary>
        /// Any other error not covered by the previous ones.
        /// </summary>
        public static readonly ResultCodes GenericError                    = new ("GenericError");


        public static readonly ResultCodes UnknownClient                   = new ("UnknownClient");
        public static readonly ResultCodes NetworkError                    = new ("NetworkError");
        public static readonly ResultCodes Timeout                         = new ("Timeout");



        #region Operator overloading

        #region Operator == (ErrorCode1, ErrorCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ErrorCode1">An OCPP WebSocket error code.</param>
        /// <param name="ErrorCode2">Another OCPP WebSocket error code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ResultCodes ErrorCode1,
                                           ResultCodes ErrorCode2)

            => ErrorCode1.Equals(ErrorCode2);

        #endregion

        #region Operator != (ErrorCode1, ErrorCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ErrorCode1">An OCPP WebSocket error code.</param>
        /// <param name="ErrorCode2">Another OCPP WebSocket error code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ResultCodes ErrorCode1,
                                           ResultCodes ErrorCode2)

            => !ErrorCode1.Equals(ErrorCode2);

        #endregion

        #region Operator <  (ErrorCode1, ErrorCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ErrorCode1">An OCPP WebSocket error code.</param>
        /// <param name="ErrorCode2">Another OCPP WebSocket error code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ResultCodes ErrorCode1,
                                          ResultCodes ErrorCode2)

            => ErrorCode1.CompareTo(ErrorCode2) < 0;

        #endregion

        #region Operator <= (ErrorCode1, ErrorCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ErrorCode1">An OCPP WebSocket error code.</param>
        /// <param name="ErrorCode2">Another OCPP WebSocket error code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ResultCodes ErrorCode1,
                                           ResultCodes ErrorCode2)

            => ErrorCode1.CompareTo(ErrorCode2) <= 0;

        #endregion

        #region Operator >  (ErrorCode1, ErrorCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ErrorCode1">An OCPP WebSocket error code.</param>
        /// <param name="ErrorCode2">Another OCPP WebSocket error code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ResultCodes ErrorCode1,
                                          ResultCodes ErrorCode2)

            => ErrorCode1.CompareTo(ErrorCode2) > 0;

        #endregion

        #region Operator >= (ErrorCode1, ErrorCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ErrorCode1">An OCPP WebSocket error code.</param>
        /// <param name="ErrorCode2">Another OCPP WebSocket error code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ResultCodes ErrorCode1,
                                           ResultCodes ErrorCode2)

            => ErrorCode1.CompareTo(ErrorCode2) >= 0;

        #endregion

        #endregion

        #region IComparable<errorCode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCPP WebSocket error codes.
        /// </summary>
        /// <param name="Object">An OCPP WebSocket error code to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ResultCodes errorCode
                   ? CompareTo(errorCode)
                   : throw new ArgumentException("The given object is not an error code!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(errorCode)

        /// <summary>
        /// Compares two OCPP WebSocket error codes.
        /// </summary>
        /// <param name="errorCode">An OCPP WebSocket error code to compare with.</param>
        public Int32 CompareTo(ResultCodes errorCode)

            => String.Compare(InternalId,
                              errorCode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<errorCode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCPP WebSocket error codes for equality.
        /// </summary>
        /// <param name="Object">An OCPP WebSocket error code to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResultCodes errorCode &&
                   Equals(errorCode);

        #endregion

        #region Equals(errorCode)

        /// <summary>
        /// Compares two OCPP WebSocket error codes for equality.
        /// </summary>
        /// <param name="errorCode">An OCPP WebSocket error code to compare with.</param>
        public Boolean Equals(ResultCodes errorCode)

            => String.Equals(InternalId,
                             errorCode.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
