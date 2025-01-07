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

using cloud.charging.open.protocols.OCPP;
using System;
using System.Linq;
using System.Text;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public static class ChargyLib
    {

        #region UnixEpoch

        /// <summary>
        /// The UNIX epoch.
        /// </summary>
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        public static Byte[] SetHex(this Byte[] CryptoBuffer, Byte[] Hex, UInt32 offset, Boolean reverse = false)
        {

            if (Hex == null || Hex.Length == 0)
                return Hex;

            Array.Copy(Hex, 0, CryptoBuffer, offset, Hex.Length);

            return Hex;

        }

        public static OBIS SetHex(this Byte[] CryptoBuffer, OBIS Obis, UInt32 offset, Boolean reverse = false)
        {
            var Bytes = Obis.Bytes;
            Array.Copy(Bytes, 0, CryptoBuffer, offset, Bytes.Length);
            return Obis;
        }

        public static Meter_Id SetHex(this Byte[] CryptoBuffer, Meter_Id EnergyMeterId, UInt32 offset, Boolean reverse = false)
        {

            var HexValue = EnergyMeterId.ToString();

            var bytes = Enumerable.Range(0, HexValue.Length).
                                   Where (x => x % 2 == 0).
                                   Select(x => Convert.ToByte(HexValue.Substring(x, 2), 16)).
                                   ToArray();

            Array.Copy(bytes, 0, CryptoBuffer, offset, bytes.Length);

            return EnergyMeterId;

        }

        public static String SetHex(this Byte[] CryptoBuffer, String HexValue, UInt32 offset, Boolean reverse = false)
        {

            Byte[] bytes = null;

            if (HexValue != null)
                HexValue = HexValue.Trim();

            if (String.IsNullOrEmpty(HexValue))
                return HexValue;

            if (HexValue.Length % 2 == 1)
                throw new ArgumentException("Wrong size of the input string!", nameof(HexValue));

            bytes = Enumerable.Range(0, HexValue.Length).
                               Where (x => x % 2 == 0).
                               Select(x => Convert.ToByte(HexValue.Substring(x, 2), 16)).
                               ToArray();

            Array.Copy(bytes, 0, CryptoBuffer, offset, bytes.Length);

            return HexValue;

        }

        public static DateTime SetTimestamp(this Byte[] CryptoBuffer, DateTime timestamp, UInt32 offset)
        {

            var unixtime  = (Int64) Math.Floor(timestamp.Subtract(UnixEpoch).TotalSeconds);
            var bytes     = BitConverter.GetBytes(unixtime);

            Array.Copy(bytes, 0, CryptoBuffer, offset, bytes.Length);

            return timestamp;

        }

        public static DateTime SetTimestamp32(this Byte[] CryptoBuffer, DateTime timestamp, UInt32 offset)
        {

            // Usage of utcOffset() is afaik EMH specific!
            var t         = new DateTimeOffset(timestamp);
            var unixtime  = (Int64) Math.Floor(timestamp.Subtract(UnixEpoch).TotalSeconds) + 60 * t.Offset.Minutes;
            var bytes     = BitConverter.GetBytes(unixtime);

            Array.Copy(bytes, 0, CryptoBuffer, offset, 4);

            //var unixtime  = timestamp.unix() + 60 * timestamp.utcOffset();
            //var bytes     = getInt64Bytes(unixtime);
            //var buffer    = new ArrayBuffer(4);
            //var tv        = new Byte[](buffer);

            //for (var i = 4; i < bytes.length; i++) {
            //    CryptoBuffer.setUint8(offset + (bytes.length - i - 1), bytes[i]);
            //    tv.setUint8(bytes.length - i - 1,            bytes[i]);
            //}

            return timestamp;

        }

        public static Byte SetInt8(this Byte[] CryptoBuffer, Byte value, UInt32 offset)
        {
            CryptoBuffer[offset] = value;
            return value;
        }

        public static Int32 SetInt8(this Byte[] CryptoBuffer, Int32 value, UInt32 offset)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, CryptoBuffer, offset, 1);
            return value;
        }

        public static UInt32 SetUInt32(this Byte[] CryptoBuffer, UInt32 value, UInt32 offset, Boolean reverse = false)
        {

            var bytes = BitConverter.GetBytes(value);

            if (reverse)
                Array.Reverse(bytes);

            Array.Copy(bytes, 0, CryptoBuffer, offset, bytes.Length);

            return value;

        }

        public static UInt64 SetUInt64(this Byte[] CryptoBuffer, UInt64 value, UInt32 offset, Boolean reverse = false)
        {

            var bytes = BitConverter.GetBytes(value);

            if (reverse)
                Array.Reverse(bytes);

            Array.Copy(bytes, 0, CryptoBuffer, offset, bytes.Length);


            return value;

        }

        public static String SetText(this Byte[] CryptoBuffer, String text, UInt32 offset)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            Array.Copy(bytes, 0, CryptoBuffer, offset, bytes.Length);
            return text;
        }

    }

}
