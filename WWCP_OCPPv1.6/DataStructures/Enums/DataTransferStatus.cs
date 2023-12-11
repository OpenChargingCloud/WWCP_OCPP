///*
// * Copyright (c) 2014-2023 GraphDefined GmbH
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

//namespace cloud.charging.open.protocols.OCPPv1_6
//{

//    /// <summary>
//    /// Extensions methods for the data transfer status.
//    /// </summary>
//    public static class DataTransferStatusExtensions
//    {

//        #region Parse(Text)

//        /// <summary>
//        /// Parse the given string as a data transfer status.
//        /// </summary>
//        /// <param name="Text">A string representation of a data transfer status.</param>
//        public static DataTransferStatus Parse(String Text)

//            => Text.Trim() switch {
//                   "Accepted"          => DataTransferStatus.Accepted,
//                   "Rejected"          => DataTransferStatus.Rejected,
//                   "UnknownMessageId"  => DataTransferStatus.UnknownMessageId,
//                   "UnknownVendorId"   => DataTransferStatus.UnknownVendorId,
//                   _                   => DataTransferStatus.Unknown
//               };

//        #endregion

//        #region AsText(this DataTransferStatus)

//        /// <summary>
//        /// Return a string representation of the given data transfer status.
//        /// </summary>
//        /// <param name="DataTransferStatus">A data transfer status.</param>
//        public static String AsText(this DataTransferStatus DataTransferStatus)

//            => DataTransferStatus switch {
//                   DataTransferStatus.Accepted          => "Accepted",
//                   DataTransferStatus.Rejected          => "Rejected",
//                   DataTransferStatus.UnknownMessageId  => "UnknownMessageId",
//                   DataTransferStatus.UnknownVendorId   => "UnknownVendorId",
//                   _                                    => "Unknown"
//               };

//        #endregion

//    }


//    /// <summary>
//    /// Defines the result of a data transfer.
//    /// </summary>
//    public enum DataTransferStatus
//    {

//        /// <summary>
//        /// Unknown data transfer status.
//        /// </summary>
//        Unknown,

//        /// <summary>
//        /// Message has been accepted, and the contained request is accepted.
//        /// </summary>
//        Accepted,

//        /// <summary>
//        /// Message has been accepted, but the contained request is rejected.
//        /// </summary>
//        Rejected,

//        /// <summary>
//        /// Message could not be interpreted due to unknown MessageId string.
//        /// </summary>
//        UnknownMessageId,

//        /// <summary>
//        /// Message could not be interpreted due to unknown VendorId string.
//        /// </summary>
//        UnknownVendorId

//    }

//}
