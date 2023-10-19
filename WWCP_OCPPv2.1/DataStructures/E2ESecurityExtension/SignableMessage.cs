﻿/*
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

using Newtonsoft.Json;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{
    public class SignableMessage : ISignableMessage
    {

        public static readonly JsonConverter[] DefaultJSONConverters  = new[] {
                                                                            new Newtonsoft.Json.Converters.IsoDateTimeConverter {
                                                                                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffZ"
                                                                            }
                                                                        };

        public JSONLDContext Context
            => JSONLDContext.Parse("none");

        public IEnumerable<KeyPair> SignKeys
            => Array.Empty<KeyPair>();

        public IEnumerable<SignInfo> SignInfos
            => Array.Empty<SignInfo>();

        public IEnumerable<Signature> Signatures
            => Array.Empty<Signature>();

        public void AddSignature(Signature Signature)
        { }

    }

}
