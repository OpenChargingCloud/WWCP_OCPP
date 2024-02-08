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

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public class EMHSignature : IEMHSignature
    {

        public String                   Algorithm        { get; }
        public SignatureFormats         Format           { get; }
        public String                   Value            { get; }
        public String?                  R                { get; }
        public String?                  S                { get; }
        public String?                  PreviousValue    { get; }

        public EMHSignature(String            Algorithm,
                            SignatureFormats  Format,
                            String            Value,
                            String?           PreviousValue  = null)
        {

            this.Algorithm      = Algorithm;
            this.Format         = Format;
            this.Value          = Value;
            this.PreviousValue  = PreviousValue;

        }

        //public static implicit operator Signature(Signature2 Signature)

        //    => new Signature(Signature.Algorithm,
        //                     Signature.Format,
        //                     Signature.PreviousValue,
        //                     Signature.Value);

    }

    //public class Signature2 : ISignature2
    //{

    //    public String                   Algorithm        { get; }
    //    public SignatureFormats         Format           { get; }
    //    public String                   PreviousValue    { get; set; }
    //    public String                   Value            { get; set; }

    //           String                   ISignature.PreviousValue
    //        => this.PreviousValue;

    //           String                   ISignature.Value
    //        => this.Value;


    //    public Signature2(String            Algorithm,
    //                      SignatureFormats  Format,
    //                      String            PreviousValue,
    //                      String            Value)
    //    {

    //        this.Algorithm      = Algorithm;
    //        this.Format         = Format;
    //        this.PreviousValue  = PreviousValue;
    //        this.Value          = Value;

    //    }

    //}

}
