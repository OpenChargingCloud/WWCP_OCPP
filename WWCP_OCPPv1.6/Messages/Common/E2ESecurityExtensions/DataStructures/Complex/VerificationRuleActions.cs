///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
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
//    /// Extension methods for verification rule actions.
//    /// </summary>
//    public static class VerificationRuleActionsExtensions
//    {

//        #region Parse   (Text)

//        /// <summary>
//        /// Parse the given text as a verification rule action.
//        /// </summary>
//        /// <param name="Text">A text representation of a verification rule action.</param>
//        public static VerificationRuleActions Parse(String Text)
//        {

//            if (TryParse(Text, out var type))
//                return type;

//            return VerificationRuleActions.Reject;

//        }

//        #endregion

//        #region TryParse(Text)

//        /// <summary>
//        /// Try to parse the given text as a verification rule action.
//        /// </summary>
//        /// <param name="Text">A text representation of a verification rule action.</param>
//        public static VerificationRuleActions? TryParse(String Text)
//        {

//            if (TryParse(Text, out var type))
//                return type;

//            return null;

//        }

//        #endregion

//        #region TryParse(Text, out VerificationRuleAction)

//        /// <summary>
//        /// Try to parse the given text as a verification rule action.
//        /// </summary>
//        /// <param name="Text">A text representation of a verification rule action.</param>
//        /// <param name="VerificationRuleAction">The parsed verification rule action.</param>
//        public static Boolean TryParse(String Text, out VerificationRuleActions VerificationRuleAction)
//        {
//            switch (Text.Trim())
//            {

//                case "acceptUnverified":
//                    VerificationRuleAction = VerificationRuleActions.AcceptUnverified;
//                    return true;

//                case "drop":
//                    VerificationRuleAction = VerificationRuleActions.Drop;
//                    return true;

//                case "verifyAny":
//                    VerificationRuleAction = VerificationRuleActions.VerifyAny;
//                    return true;

//                case "verifyAll":
//                    VerificationRuleAction = VerificationRuleActions.VerifyAll;
//                    return true;

//                default:
//                    VerificationRuleAction = VerificationRuleActions.Reject;
//                    return false;

//            }
//        }

//        #endregion


//        #region AsText(this VerificationRuleAction)

//        public static String AsText(this VerificationRuleActions VerificationRuleAction)

//            => VerificationRuleAction switch {
//                   VerificationRuleActions.AcceptUnverified  => "acceptUnverified",
//                   VerificationRuleActions.Drop              => "drop",
//                   VerificationRuleActions.VerifyAny         => "verifyAny",
//                   VerificationRuleActions.VerifyAll         => "verifyAll",
//                   _                                         => "reject"
//               };

//        #endregion

//    }


//    /// <summary>
//    /// An OCPP CSE cryptographic signature cerification policy action.
//    /// </summary>
//    public enum VerificationRuleActions
//    {

//        /// <summary>
//        /// Just accept the message without any further verification.
//        /// </summary>
//        AcceptUnverified,

//        /// <summary>
//        /// Silently drop this message.
//        /// </summary>
//        Drop,

//        /// <summary>
//        /// Reject this message as invalid.
//        /// </summary>
//        Reject,

//        /// <summary>
//        /// Verify the signatures of the incoming message and
//        /// accept it if at least one signature is valid.
//        /// </summary>
//        VerifyAny,

//        /// <summary>
//        /// Verify the signatures of the incoming message and
//        /// accept it only if all signatures are valid.
//        /// </summary>
//        VerifyAll

//    }

//}
