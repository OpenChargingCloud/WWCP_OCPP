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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The standard clock controller, ClockCtrlr: what time the charging
    /// station has, and where it comes from. It has no instance.
    /// </summary>
    /// <remarks>
    /// Its variables are those of the standard that the given source can
    /// answer, read from it at every read. A variable the source cannot answer
    /// is left out, so that a CSMS asking for it learns it is unknown rather
    /// than reading an empty value as an answer.
    ///
    /// NtpServerUri has the instances "1" to "9", the priority of the server,
    /// 1 for the highest; TimeOffset has none, for the offset now, and
    /// "NextTransition", for the offset after the next transition.
    /// </remarks>
    public class ClockController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The name of the component, as the standard spells it.
        /// </summary>
        public const String ComponentName = "ClockCtrlr";

        /// <summary>
        /// The variables of the standard, in its order.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("DateTime",                          DataTypes.DateTime,      "The current date and time of the charging station."),
            new ("NextTimeOffsetTransitionDateTime",  DataTypes.DateTime,      "When the local time offset changes next.",                                                                     Writable: true, MayBeEmpty: true),
            new ("NtpServerUri",                      DataTypes.String,        "The address of an NTP server. The instance is its priority, 1 for the highest; an empty one names no server.",  Writable: true, MayBeEmpty: true),
            new ("NtpSource",                         DataTypes.OptionList,    "Whether the NTP servers come from DHCP or are configured manually.",                                          ValuesList: [ "DHCP", "manual" ],  Writable: true),
            new ("TimeAdjustmentReportingThreshold",  DataTypes.Integer,       "A time adjustment larger than this is reported as the security event SettingSystemTime.",                    InSeconds: true,                   Writable: true, MayBeEmpty: true),
            new ("TimeOffset",                        DataTypes.String,        "The local time offset, e.g. \"+01:00\"; in the instance NextTransition the one after the next transition.",  Writable: true, MayBeEmpty: true),
            new ("TimeSource",                        DataTypes.SequenceList,  "Where the time comes from, in the order the sources are tried.",
                                                                               ValuesList: [ "Heartbeat", "NTP", "GPS", "RealTimeClock", "MobileNetwork", "RadioTimeTransmitter" ],                    Writable: true),
            new ("TimeZone",                          DataTypes.String,        "The local time zone, e.g. \"Europe/Oslo\".",                                                                    Writable: true, MayBeEmpty: true)

        ];

        /// <summary>
        /// The instances of NtpServerUri: one per priority, 1 for the highest.
        /// </summary>
        public static readonly IReadOnlyList<String> NtpServerUriInstances = [ "1", "2", "3", "4", "5", "6", "7", "8", "9" ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create the clock controller of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ClockController(IClockControllerSource  Source,
                               I18NString?             Description   = null,
                               CustomData?             CustomData    = null)

            : base(ComponentName,
                   null,
                   Description ?? I18NString.Create("What time this charging station has, and where it comes from."),
                   CustomData)

        {

            // Written only by a source that takes a change: every write goes to
            // it with the others of its request, since a server at one priority
            // depends on those at the others.
            var setter   = Source as ISetVariablesSource;
            var answered = new HashSet<String>(Source.Variables, StringComparer.OrdinalIgnoreCase);
            var written  = new HashSet<String>(setter is not null ? Source.WritableVariables : [], StringComparer.OrdinalIgnoreCase);

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                foreach (var instance in InstancesOf(variable.Name))
                    variableConfigs.Add(
                        variable.ToVariableConfig(() => Source.Value(variable.Name, instance),
                                                  Writes:    written.Contains(variable.Name),
                                                  Instance:  instance)
                    );

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) TryCheck(Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of the standard can be set to the given
        /// value by its type, its limits and its values alone - or why not, as
        /// a SetVariables result says it. Whether the value fits the rest of the
        /// configuration is for the source to ask.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)

            => ControllerVariable.TryCheck(Variables, ComponentName, Variable, Value, out Refusal);

        #endregion


        #region (private static) InstancesOf(Variable)

        private static IEnumerable<String?> InstancesOf(String Variable)

            => Variable switch {
                   "NtpServerUri"  => (IEnumerable<String?>) NtpServerUriInstances,
                   "TimeOffset"    => [ null, "NextTransition" ],
                   _               => [ null ]
               };

        #endregion

    }

}
