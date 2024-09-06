///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6
//{

//    /// <summary>
//    /// Extension methods for OCPP result codes.
//    /// </summary>
//    public static class ResultCodeExtensions
//    {

//        /// <summary>
//        /// Indicates whether this OCPP result code is null or empty.
//        /// </summary>
//        /// <param name="ResultCode">An OCPP result code.</param>
//        public static Boolean IsNullOrEmpty(this ResultCode? ResultCode)
//            => !ResultCode.HasValue || ResultCode.Value.IsNullOrEmpty;

//        /// <summary>
//        /// Indicates whether this OCPP result code is null or empty.
//        /// </summary>
//        /// <param name="ResultCode">An OCPP result code.</param>
//        public static Boolean IsNotNullOrEmpty(this ResultCode? ResultCode)
//            => ResultCode.HasValue && ResultCode.Value.IsNotNullOrEmpty;

//    }


//    /// <summary>
//    /// An OCPP result code.
//    /// </summary>
//    public readonly struct ResultCode : IId,
//                                        IEquatable<ResultCode>,
//                                        IComparable<ResultCode>
//    {

//        #region Data

//        private readonly static Dictionary<String, ResultCode>  lookup = new (StringComparer.OrdinalIgnoreCase);
//        private readonly        String                          InternalId;

//        #endregion

//        #region Properties

//        /// <summary>
//        /// Indicates whether this identification is null or empty.
//        /// </summary>
//        public readonly Boolean IsNullOrEmpty
//            => InternalId.IsNullOrEmpty();

//        /// <summary>
//        /// Indicates whether this identification is NOT null or empty.
//        /// </summary>
//        public readonly Boolean IsNotNullOrEmpty
//            => InternalId.IsNotNullOrEmpty();

//        /// <summary>
//        /// The length of the result code.
//        /// </summary>
//        public readonly UInt64 Length
//            => (UInt64) (InternalId?.Length ?? 0);

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new OCPP result code based on the given text.
//        /// </summary>
//        /// <param name="Text">The text representation of the result code.</param>
//        private ResultCode(String Text)
//        {
//            this.InternalId  = Text;
//        }

//        #endregion


//        #region (private static) Register(Text)

//        private static ResultCode Register(String Text)

//            => lookup.AddAndReturnValue(
//                   Text,
//                   new ResultCode(Text)
//               );

//        #endregion


//        #region (static) Parse   (Text)

//        /// <summary>
//        /// Parse the given string as an OCPP result code.
//        /// </summary>
//        /// <param name="Text">A text representation of an OCPP result code.</param>
//        public static ResultCode Parse(String Text)
//        {

//            if (TryParse(Text, out var resultCode))
//                return resultCode;

//            throw new ArgumentException($"Invalid text representation of an OCPP result code: '{Text}'!",
//                                        nameof(Text));

//        }

//        #endregion

//        #region (static) TryParse(Text)

//        /// <summary>
//        /// Try to parse the given text as an OCPP result code.
//        /// </summary>
//        /// <param name="Text">A text representation of an OCPP result code.</param>
//        public static ResultCode? TryParse(String Text)
//        {

//            if (TryParse(Text, out var resultCode))
//                return resultCode;

//            return null;

//        }

//        #endregion

//        #region (static) TryParse(Text, out resultCode)

//        /// <summary>
//        /// Try to parse the given text as an OCPP result code.
//        /// </summary>
//        /// <param name="Text">A text representation of an OCPP result code.</param>
//        /// <param name="resultCode">The parsed OCPP result code.</param>
//        public static Boolean TryParse(String Text, out ResultCode ResultCode)
//        {

//            Text = Text.Trim();

//            if (Text.IsNotNullOrEmpty())
//            {

//                if (!lookup.TryGetValue(Text, out ResultCode))
//                    ResultCode = Register(Text);

//                return true;

//            }

//            ResultCode = default;
//            return false;

//        }

//        #endregion

//        #region Clone

//        /// <summary>
//        /// Clone this OCPP result code.
//        /// </summary>
//        public ResultCode Clone

//            => new (
//                   new String(InternalId?.ToCharArray())
//               );

//        #endregion


//        #region Static definitions

//        /// <summary>
//        /// Requested Action is not known by receiver.
//        /// </summary>
//        public static ResultCode OK                              { get; }
//            = Register("OK");

//        /// <summary>
//        /// Requested Action is not known by receiver.
//        /// </summary>
//        public static ResultCode NotImplemented                  { get; }
//            = Register("NotImplemented");

//        /// <summary>
//        /// Requested Action is recognized but not supported by the receiver.
//        /// </summary>
//        public static ResultCode NotSupported                    { get; }
//            = Register("NotSupported");

//        /// <summary>
//        /// An internal error occurred and the receiver was not able to process the requested Action successfully.
//        /// </summary>
//        public static ResultCode InternalError                   { get; }
//            = Register("InternalError");

//        /// <summary>
//        /// Payload for Action is incomplete.
//        /// </summary>
//        public static ResultCode ProtocolError                   { get; }
//            = Register("ProtocolError");

//        /// <summary>
//        /// During the processing of Action a security issue occurred preventing receiver from completing the Action successfully.
//        /// </summary>
//        public static ResultCode SecurityError                   { get; }
//            = Register("SecurityError");

//        /// <summary>
//        /// The message was filtered by administrative filter rules.
//        /// </summary>
//        public static ResultCode Filtered                        { get; }
//            = Register("Filtered");

//        /// <summary>
//        /// Payload for Action is syntactically incorrect or not conform the PDU structure for Action.
//        /// </summary>
//        public static ResultCode FormationViolation              { get; }
//            = Register("FormationViolation");

//        /// <summary>
//        /// Payload is syntactically correct but at least one field contains an invalid value.
//        /// </summary>
//        public static ResultCode PropertyConstraintViolation     { get; }
//            = Register("PropertyConstraintViolation");

//        /// <summary>
//        /// Payload for Action is syntactically correct but at least one of the fields violates occurence constraints.
//        /// </summary>
//        public static ResultCode OccurenceConstraintViolation    { get; }
//            = Register("OccurenceConstraintViolation");

//        /// <summary>
//        /// Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12).
//        /// </summary>
//        public static ResultCode TypeConstraintViolation         { get; }
//            = Register("TypeConstraintViolation");

//        /// <summary>
//        /// Any other error not covered by the previous ones.
//        /// </summary>
//        public static ResultCode GenericError                    { get; }
//            = Register("GenericError");


//        public static ResultCode UnknownClient                   { get; }
//            = Register("UnknownClient");

//        public static ResultCode NetworkError                    { get; }
//            = Register("NetworkError");

//        public static ResultCode Timeout                         { get; }
//            = Register("Timeout");

//        #endregion


//        #region Operator overloading

//        #region Operator == (ResultCode1, ResultCode2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ResultCode1">An OCPP result code.</param>
//        /// <param name="ResultCode2">Another OCPP result code.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator == (ResultCode ResultCode1,
//                                           ResultCode ResultCode2)

//            => ResultCode1.Equals(ResultCode2);

//        #endregion

//        #region Operator != (ResultCode1, ResultCode2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ResultCode1">An OCPP result code.</param>
//        /// <param name="ResultCode2">Another OCPP result code.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator != (ResultCode ResultCode1,
//                                           ResultCode ResultCode2)

//            => !ResultCode1.Equals(ResultCode2);

//        #endregion

//        #region Operator <  (ResultCode1, ResultCode2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ResultCode1">An OCPP result code.</param>
//        /// <param name="ResultCode2">Another OCPP result code.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator < (ResultCode ResultCode1,
//                                          ResultCode ResultCode2)

//            => ResultCode1.CompareTo(ResultCode2) < 0;

//        #endregion

//        #region Operator <= (ResultCode1, ResultCode2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ResultCode1">An OCPP result code.</param>
//        /// <param name="ResultCode2">Another OCPP result code.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator <= (ResultCode ResultCode1,
//                                           ResultCode ResultCode2)

//            => ResultCode1.CompareTo(ResultCode2) <= 0;

//        #endregion

//        #region Operator >  (ResultCode1, ResultCode2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ResultCode1">An OCPP result code.</param>
//        /// <param name="ResultCode2">Another OCPP result code.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator > (ResultCode ResultCode1,
//                                          ResultCode ResultCode2)

//            => ResultCode1.CompareTo(ResultCode2) > 0;

//        #endregion

//        #region Operator >= (ResultCode1, ResultCode2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ResultCode1">An OCPP result code.</param>
//        /// <param name="ResultCode2">Another OCPP result code.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator >= (ResultCode ResultCode1,
//                                           ResultCode ResultCode2)

//            => ResultCode1.CompareTo(ResultCode2) >= 0;

//        #endregion

//        #endregion

//        #region IComparable<resultCode> Members

//        #region CompareTo(Object)

//        /// <summary>
//        /// Compares two OCPP result codes.
//        /// </summary>
//        /// <param name="Object">An OCPP result code to compare with.</param>
//        public Int32 CompareTo(Object? Object)

//            => Object is ResultCode resultCode
//                   ? CompareTo(resultCode)
//                   : throw new ArgumentException("The given object is not an OCPP result code!",
//                                                 nameof(Object));

//        #endregion

//        #region CompareTo(resultCode)

//        /// <summary>
//        /// Compares two OCPP result codes.
//        /// </summary>
//        /// <param name="resultCode">An OCPP result code to compare with.</param>
//        public Int32 CompareTo(ResultCode resultCode)

//            => String.Compare(InternalId,
//                              resultCode.InternalId,
//                              StringComparison.OrdinalIgnoreCase);

//        #endregion

//        #endregion

//        #region IEquatable<resultCode> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two OCPP result codes for equality.
//        /// </summary>
//        /// <param name="Object">An OCPP result code to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is ResultCode resultCode &&
//                   Equals(resultCode);

//        #endregion

//        #region Equals(resultCode)

//        /// <summary>
//        /// Compares two OCPP result codes for equality.
//        /// </summary>
//        /// <param name="resultCode">An OCPP result code to compare with.</param>
//        public Boolean Equals(ResultCode resultCode)

//            => String.Equals(InternalId,
//                             resultCode.InternalId,
//                             StringComparison.OrdinalIgnoreCase);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        /// <returns>The hash code of this object.</returns>
//        public override Int32 GetHashCode()

//            => InternalId?.ToLower().GetHashCode() ?? 0;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => InternalId ?? "";

//        #endregion

//    }

//}
