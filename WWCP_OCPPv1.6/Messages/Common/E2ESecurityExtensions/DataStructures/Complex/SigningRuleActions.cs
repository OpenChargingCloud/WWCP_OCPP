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
//    /// Extension methods for signing rule actions.
//    /// </summary>
//    public static class SigningRuleActionsExtensions
//    {

//        #region Parse   (Text)

//        /// <summary>
//        /// Parse the given text as a signing rule action.
//        /// </summary>
//        /// <param name="Text">A text representation of a signing rule action.</param>
//        public static SigningRuleActions Parse(String Text)
//        {

//            if (TryParse(Text, out var type))
//                return type;

//            return SigningRuleActions.Reject;

//        }

//        #endregion

//        #region TryParse(Text)

//        /// <summary>
//        /// Try to parse the given text as a signing rule action.
//        /// </summary>
//        /// <param name="Text">A text representation of a signing rule action.</param>
//        public static SigningRuleActions? TryParse(String Text)
//        {

//            if (TryParse(Text, out var type))
//                return type;

//            return null;

//        }

//        #endregion

//        #region TryParse(Text, out SigningRuleAction)

//        /// <summary>
//        /// Try to parse the given text as a signing rule action.
//        /// </summary>
//        /// <param name="Text">A text representation of a signing rule action.</param>
//        /// <param name="SigningRuleAction">The parsed signing rule action.</param>
//        public static Boolean TryParse(String Text, out SigningRuleActions SigningRuleAction)
//        {
//            switch (Text.Trim())
//            {

//                case "forwardUnsigned":
//                    SigningRuleAction = SigningRuleActions.ForwardUnsigned;
//                    return true;

//                case "drop":
//                    SigningRuleAction = SigningRuleActions.Drop;
//                    return true;

//                case "sign":
//                    SigningRuleAction = SigningRuleActions.Sign;
//                    return true;

//                default:
//                    SigningRuleAction = SigningRuleActions.Reject;
//                    return false;

//            }
//        }

//        #endregion


//        #region AsText(this SigningRuleAction)

//        public static String AsText(this SigningRuleActions SigningRuleAction)

//            => SigningRuleAction switch {
//                   SigningRuleActions.Reject  => "forwardUnsigned",
//                   SigningRuleActions.Drop    => "drop",
//                   SigningRuleActions.Sign    => "sign",
//                   _                          => "reject"
//               };

//        #endregion

//    }


//    /// <summary>
//    /// An OCPP CSE cryptographic signature policy action.
//    /// </summary>
//    public enum SigningRuleActions
//    {

//        /// <summary>
//        /// Just forward this message.
//        /// </summary>
//        ForwardUnsigned,

//        /// <summary>
//        /// Silently drop this message.
//        /// </summary>
//        Drop,

//        /// <summary>
//        /// Reject this message as invalid.
//        /// </summary>
//        Reject,

//        /// <summary>
//        /// Sign this outgoing message.
//        /// </summary>
//        Sign

//    }

//}
