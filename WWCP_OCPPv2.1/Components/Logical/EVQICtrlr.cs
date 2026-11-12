/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where EVQICtrlr[TLSServer] reads the TLS server keys of a Local Controller.
    /// </summary>
    /// <remarks>
    /// The controller keeps no value of its own: whatever holds the keys answers each read, so that a
    /// next key made or a switch done by the Local Controller's command line or web page is what the next
    /// read sees. A source that is an <see cref="ITLSServerKeySwitch"/> as well makes ActiveKeyPin
    /// writable: a CSMS switches the listener to the announced next key by writing that key's pin.
    /// </remarks>
    public interface IEVQICtrlrSource
    {

        /// <summary>
        /// The variables of EVQICtrlr[TLSServer] this source answers: of KeyPins, NextKeyPin, KeyCreated,
        /// RawPublicKeys and ActiveKeyPin.
        /// </summary>
        IEnumerable<String> TLSServerVariables { get; }

        /// <summary>
        /// The value of a variable of EVQICtrlr[TLSServer], in the words of the white paper; null for none.
        /// Key pins as 64 lowercase hexadecimal digits, KeyCreated in RFC 3339.
        /// </summary>
        /// <param name="Variable">The variable, as the paper spells it.</param>
        String? TLSServerValue(String Variable);

    }


    /// <summary>
    /// What switches a Local Controller's TLS listener from its current key to its announced next key.
    /// </summary>
    public interface ITLSServerKeySwitch
    {

        /// <summary>
        /// Make the key with the given key pin, which EVQICtrlr found to be the announced next key, the
        /// current one: the key the listener presents from now on.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Compare and switch: switch only if the given pin is still the announced next key's, and answer
        /// null without switching again where it is the current one already - a switch made meanwhile on
        /// the Local Controller's command line or by another request is not repeated.
        /// </para>
        /// <para>
        /// The request is the whole SetVariables request, with its network path (who sent it) and its
        /// signatures: the place where the right to change the server key is checked. A switch changes
        /// whom the stations trust, a trust decision like the certificate store, and is meant to need
        /// "certificates:edit" (white paper "Signed OCPP Messages and Signer Roles", Q12).
        /// </para>
        /// </remarks>
        /// <param name="NextKeyPin">The key pin of the announced next key.</param>
        /// <param name="Request">The SetVariables request that asks for the switch, where there is one.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <returns>Null when the key with the given pin is the current one now; why not otherwise.</returns>
        Task<StatusInfo?> SwitchToNextKeyAsync(KeyPin                NextKeyPin,
                                               SetVariablesRequest?  Request,
                                               CancellationToken     CancellationToken);

    }


    /// <summary>
    /// The EVQI controller of the white paper "Key Pins and TLS 1.3 Raw Public Keys for Local Controllers"
    /// (sections 5.5 and 7.4), the instance "TLSServer": the keys of a Local Controller's TLS listener as its
    /// CSMS sees them - the pins of the current and of the announced next key, when the current one was
    /// made, whether raw public keys are offered - and the switch to the next key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Rollover (section 5.5): the Local Controller makes its next key in advance and reports both pins in
    /// KeyPins; the CSMS adds the next pin to every station of the site; once all of them have it, the CSMS
    /// writes the next key's pin into ActiveKeyPin. That write is declarative and idempotent: the pin of the
    /// current key is accepted and changes nothing, so a CSMS that repeats it after a lost response does no
    /// harm, and any other pin is refused with the reason. A signed write cannot be turned into another
    /// switch than the one it names.
    /// </para>
    /// <para>
    /// ActiveKeyPin changes the server key, a trust decision like the certificate store: it is meant to
    /// need the right "certificates:edit" of the white paper "Signed OCPP Messages and Signer Roles" (Q12).
    /// That check is not built yet; the request reaches the <see cref="ITLSServerKeySwitch"/> with its
    /// sender and its signatures, where it will be.
    /// </para>
    /// </remarks>
    public class EVQICtrlr : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The instance of the TLS server keys.
        /// </summary>
        public const String TLSServer = "TLSServer";

        /// <summary>
        /// The variables of EVQICtrlr[TLSServer], in the order of the paper, and the switch.
        /// </summary>
        internal static readonly ControllerVariable[] TLSServerVariables = [

            new ("KeyPins",        DataTypes.String,      "Comma separated key pins of the listener's keys: the current key's first, then the announced next key's."),
            new ("NextKeyPin",     DataTypes.String,      "The key pin of the announced next key; empty while there is none."),
            new ("KeyCreated",     DataTypes.DateTime,    "When the current key was made."),
            new ("RawPublicKeys",  DataTypes.OptionList,  "Whether the listener offers its key raw (RFC 7250).", ValuesList: [ "Off", "On" ]),
            new ("ActiveKeyPin",   DataTypes.String,      "The key pin of the key the listener presents now. Writing the announced next key's pin switches to it; writing the current one changes nothing; any other pin is refused.", Writable: true)

        ];

        private readonly SemaphoreSlim switchLock = new (1, 1);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The instance "TLSServer".
        /// </summary>
        /// <param name="Source">Where the values come from; one that is an <see cref="ITLSServerKeySwitch"/> as well makes ActiveKeyPin writable.</param>
        public EVQICtrlr(IEVQICtrlrSource Source)

            : base(nameof(EVQICtrlr),
                   TLSServer,
                   I18NString.Create("The TLS server keys of this Local Controller, and the switch to its next key."),
                   null)

        {

            var keySwitch  = Source as ITLSServerKeySwitch;
            var answered   = new HashSet<String>(Source.TLSServerVariables, StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: a CSMS sees a next key made, and a switch done, by a Delta monitor.
            foreach (var variable in TLSServerVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.TLSServerValue(variable.Name),
                                                              variable.Name == "ActiveKeyPin" && keySwitch is not null,
                                                              Monitored: true));

            if (keySwitch is not null && answered.Contains("ActiveKeyPin"))
                SetVariablesSource = new Switch(this, Source, keySwitch);

        }

        #endregion


        #region (static) TryCheck(Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of EVQICtrlr[TLSServer] can be set to the given value by its type and
        /// syntax alone, or why not, as a SetVariables result says it.
        /// </summary>
        /// <remarks>
        /// Every variable but ActiveKeyPin is read-only. Whether an ActiveKeyPin is the current or the next
        /// key's is for the controller to say, with the source at hand.
        /// </remarks>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            var variable = TLSServerVariables.FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(EVQICtrlr)}[{TLSServer}]'!");
                return false;
            }

            var where = $"{nameof(EVQICtrlr)}[{TLSServer}].{variable.Name}";

            if (!variable.Writable)
            {
                Refusal = new StatusInfo("ReadOnly", $"{where} is read-only!");
                return false;
            }

            if (!variable.TryCheck(Value, where, out Refusal))
                return false;

            // ActiveKeyPin: one key pin, never none.
            if (!KeyPin.TryParse(Value, out _))
            {
                Refusal = new StatusInfo("InvalidValue", $"'{where}': \"{Value}\" is no key pin - 64 hexadecimal digits, with or without colons between the bytes!");
                return false;
            }

            return true;

        }

        #endregion

        #region (private) SwitchAsync(Value, Request, CancellationToken)

        /// <summary>
        /// Write ActiveKeyPin: accepted and nothing done for the current key's pin, the switch for the
        /// announced next key's, refused for any other.
        /// </summary>
        private async Task<StatusInfo?> SwitchAsync(IEVQICtrlrSource      Source,
                                                    ITLSServerKeySwitch   KeySwitch,
                                                    String?               Value,
                                                    SetVariablesRequest?  Request,
                                                    CancellationToken     CancellationToken)
        {

            if (!TryCheck("ActiveKeyPin", Value, out var refusal))
                return refusal;

            var wanted = KeyPin.Parse(Value!);

            // One switch at a time: the current and the next key are read and compared
            // with nothing switched in between by another request.
            await switchLock.WaitAsync(CancellationToken).ConfigureAwait(false);

            try
            {

                KeyPin.TryParse(Source.TLSServerValue("ActiveKeyPin"), out var current);
                KeyPin.TryParse(Source.TLSServerValue("NextKeyPin"),   out var next);

                if (wanted == current)
                    return null;

                if (next is null || wanted != next)
                    return new StatusInfo(
                               "InvalidValue",
                               $"'{nameof(EVQICtrlr)}[{TLSServer}].ActiveKeyPin': {wanted} is neither the key pin of the current key ({current?.ToString() ?? "none"}) " +
                               $"nor of the announced next one ({next?.ToString() ?? "none announced"})!"
                           );

                return await KeySwitch.SwitchToNextKeyAsync(next, Request, CancellationToken).ConfigureAwait(false);

            }
            finally
            {
                switchLock.Release();
            }

        }

        #endregion


        #region (private class) Switch

        /// <summary>
        /// Takes the writes of ActiveKeyPin from a SetVariables request.
        /// </summary>
        private sealed class Switch(EVQICtrlr            Controller,
                                    IEVQICtrlrSource     Source,
                                    ITLSServerKeySwitch  KeySwitch) : ISetVariablesSource
        {

            public async Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                                 DataConsistencyModel             DataConsistencyModel,
                                                                                 CancellationToken                CancellationToken)
            {

                var results = new List<SetVariableResult>(Entries.Count);

                foreach (var entry in Entries)
                {

                    var refusal = String.Equals(entry.Variable.Name, "ActiveKeyPin", StringComparison.OrdinalIgnoreCase)
                                      ? await Controller.SwitchAsync(Source, KeySwitch, entry.Data.AttributeValue, entry.Request, CancellationToken).ConfigureAwait(false)
                                      : new StatusInfo("ReadOnly", $"{nameof(EVQICtrlr)}[{TLSServer}].{entry.Variable.Name} is read-only!");

                    results.Add(new SetVariableResult(
                                    AttributeStatus:       refusal is null
                                                               ? SetVariableStatus.Accepted
                                                               : SetVariableStatus.Rejected,
                                    Component:             entry.Data.Component,
                                    Variable:              entry.Data.Variable,
                                    AttributeType:         entry.Variable.Attributes?.Type ?? AttributeTypes.Actual,
                                    AttributeStatusInfo:   refusal
                                ));

                }

                return results;

            }

        }

        #endregion

    }

}
