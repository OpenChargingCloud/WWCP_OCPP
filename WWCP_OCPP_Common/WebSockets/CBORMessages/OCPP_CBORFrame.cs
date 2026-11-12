/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// The frame of an OCPP message in CBOR: the array an OCPP message is in JSON,
    /// item by item - the message type a number, identifications and actions texts,
    /// a source routing or a network path a text or an array of texts - and in its
    /// last place a map: the payload of the message, or the details of an error.
    ///
    /// Whatever stands in front of that map is read and written by the JSON message
    /// of the same kind, so that both frames have one definition and cannot drift apart.
    /// </summary>
    public static class OCPP_CBORFrame
    {

        #region TryParse(Binary, out CBORArray, out ErrorResponse)

        /// <summary>
        /// Try to read the given bytes as the CBOR array of an OCPP message.
        /// </summary>
        /// <param name="Binary">The bytes of a binary frame.</param>
        /// <param name="CBORArray">The CBOR array.</param>
        /// <param name="ErrorResponse">Why the bytes are no CBOR array.</param>
        public static Boolean TryParse(Byte[]                              Binary,
                                       [NotNullWhen(true)]  out CBORValue  CBORArray,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            CBORArray = default;

            if (Binary is null || Binary.Length == 0)
            {
                ErrorResponse = "The given binary frame is empty!";
                return false;
            }

            try
            {

                CBORArray = CBORValue.Parse(Binary);

                if (CBORArray.Kind != CBORValueKind.Array)
                {
                    ErrorResponse = "The given binary frame is CBOR, but not an array!";
                    return false;
                }

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given binary frame is not CBOR: " + e.Message;
                return false;
            }

        }

        #endregion

        #region TrySplit(CBORArray, out Header, out Last, out ErrorResponse)

        /// <summary>
        /// Try to take the given CBOR array apart: into what stands in front of its
        /// last item, as the JSON array of the same message with an empty object in
        /// its last place, and that last item, which must be a map.
        /// </summary>
        /// <param name="CBORArray">The CBOR array of an OCPP message.</param>
        /// <param name="Header">The JSON array of the same message, its last item an empty object.</param>
        /// <param name="Last">The last item of the CBOR array: the payload, or the details of an error.</param>
        /// <param name="ErrorResponse">Why the array is no OCPP message.</param>
        public static Boolean TrySplit(CBORValue                           CBORArray,
                                       [NotNullWhen(true)]  out JArray?    Header,
                                       [NotNullWhen(true)]  out CBORValue  Last,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            Header = null;
            Last   = default;

            if (CBORArray.Kind != CBORValueKind.Array)
            {
                ErrorResponse = "The given CBOR is not an array!";
                return false;
            }

            var items = CBORArray.AsArray();

            if (items.Count < 3)
            {
                ErrorResponse = $"The given CBOR array has {items.Count} items, an OCPP message at least 3!";
                return false;
            }

            Last = items[^1];

            if (Last.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The last item of the given CBOR array is not a map!";
                return false;
            }

            var header = new JArray();

            for (var i = 0; i < items.Count - 1; i++)
            {

                var item = items[i];

                if (item.TryGetUInt64(out var number) && number <= Int64.MaxValue)
                    header.Add(new JValue((Int64) number));

                else if (item.TryGetText(out var text))
                    header.Add(new JValue(text));

                else if (item.Kind == CBORValueKind.Array)
                {

                    var texts = new JArray();

                    foreach (var hop in item.AsArray())
                    {

                        if (!hop.TryGetText(out var hopText))
                        {
                            ErrorResponse = $"The array at position {i} of the given CBOR array holds something that is not a text!";
                            return false;
                        }

                        texts.Add(new JValue(hopText));

                    }

                    header.Add(texts);

                }

                else
                {
                    ErrorResponse = $"The item at position {i} of the given CBOR array is neither a number, a text nor an array of texts!";
                    return false;
                }

            }

            header.Add(new JObject());

            Header         = header;
            ErrorResponse  = null;
            return true;

        }

        #endregion

        #region Join(JSONArray, Last)

        /// <summary>
        /// The CBOR array of an OCPP message: what stands in front of the last item
        /// of the given JSON array of that message, and the given map in its place.
        /// </summary>
        /// <param name="JSONArray">The JSON array of an OCPP message; its last item is not looked at.</param>
        /// <param name="Last">The payload of the message, or the details of an error.</param>
        public static CBORValue Join(JArray     JSONArray,
                                     CBORValue  Last)
        {

            var items = new List<CBORValue>(JSONArray.Count);

            for (var i = 0; i < JSONArray.Count - 1; i++)
            {

                var item = JSONArray[i];

                switch (item.Type)
                {

                    case JTokenType.Integer:
                        items.Add(CBORValue.FromUInt64(item.Value<UInt64>()));
                        break;

                    case JTokenType.String:
                        items.Add(CBORValue.FromText(item.Value<String>() ?? ""));
                        break;

                    case JTokenType.Array:
                        items.Add(CBORValue.FromArray(item.Select(hop => CBORValue.FromText(hop.Value<String>() ?? ""))));
                        break;

                    default:
                        throw new InvalidOperationException($"The item at position {i} of an OCPP message is a JSON {item.Type}, for which its CBOR array has no place!");

                }

            }

            items.Add(Last);

            return CBORValue.FromArray(items);

        }

        #endregion

    }

}
