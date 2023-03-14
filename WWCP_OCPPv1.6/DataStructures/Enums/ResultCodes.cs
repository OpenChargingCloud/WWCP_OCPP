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
//    /// Extensions methods for result and error codes.
//    /// </summary>
//    public static class ResultCodesExtensions
//    {

//        #region Parse(Text)

//        public static ResultCodes Parse(String Text)

//            => Text.Trim() switch {
//                   "OK"             => ResultCodes.OK,
//                   "Partly"         => ResultCodes.Partly,
//                   "NotAuthorized"  => ResultCodes.NotAuthorized,
//                   "InvalidId"      => ResultCodes.InvalidId,
//                   "Server"         => ResultCodes.Server,
//                   "Format"         => ResultCodes.Format,
//                   _                => ResultCodes.Unknown
//               };

//        #endregion

//        #region AsText(this ResultCodes)

//        public static String AsText(this ResultCodes ResultCodes)

//            => ResultCodes switch {
//                   ResultCodes.OK             => "OK",
//                   ResultCodes.Partly         => "Partly",
//                   ResultCodes.NotAuthorized  => "NotAuthorized",
//                   ResultCodes.InvalidId      => "InvalidId",
//                   ResultCodes.Server         => "Server",
//                   ResultCodes.Format         => "Format",
//                   _                          => "Unknown"
//               };

//        #endregion

//    }


//    /// <summary>
//    /// Result and error codes for the class Result as return value for method calls.
//    /// </summary>
//    public enum ResultCodes
//    {

//        /// <summary>
//        /// Unknown result code.
//        /// </summary>
//        Unknown,

//        /// <summary>
//        /// Data accepted and processed.
//        /// </summary>
//        OK,

//        /// <summary>
//        /// Only part of the data was accepted.
//        /// </summary>
//        Partly,

//        /// <summary>
//        /// Wrong username and/or password.
//        /// </summary>
//        NotAuthorized,

//        /// <summary>
//        /// One or more ID (EVSE/Contract) were not valid for this user.
//        /// </summary>
//        InvalidId,

//        /// <summary>
//        /// Internal server error.
//        /// </summary>
//        Server,

//        /// <summary>
//        /// Data has technical errors.
//        /// </summary>
//        Format

//    }

//}
