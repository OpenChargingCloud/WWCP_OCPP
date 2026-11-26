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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The Device Model of this node as its OCPP Device Model page shows it:
    /// every component of OCPP 2.1 with its variables and what they say now,
    /// and every configuration key of OCPP 1.6.
    /// </summary>
    public abstract partial class OCPPServingNode
    {

        #region DeviceModelJSON(ShowWriteOnly)

        /// <summary>
        /// The Device Model of this node, as a tree to browse:
        /// <c>{ "ocpp2.1": { component: { variable: { value, ... } } },
        ///      "ocpp1.6": { key: { value, ... } } }</c> - each version only
        /// where the node has an OCPP node of it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// What each variable says is read now, as GetVariables reads it, and a
        /// getter that throws says so in place of its value instead of taking
        /// the whole tree down. A component is named as OCPP names it - its
        /// name, its instance in brackets, its EVSE and connector -, sorted as
        /// GetBaseReport sorts them.
        /// </para>
        /// <para>
        /// A value OCPP keeps WriteOnly - a password, a key - is left out and
        /// marked as hidden, unless <paramref name="ShowWriteOnly"/> is set:
        /// then it is there, as for everything else (Achim 2026-11-26: a
        /// systemadmin is hidden nothing). The configuration keys of OCPP 1.6
        /// are as their sources hand them out, which leave out the value of a
        /// write-only key; none of them has one today.
        /// </para>
        /// </remarks>
        /// <param name="ShowWriteOnly">Whether the values OCPP keeps WriteOnly are shown too.</param>
        public JObject DeviceModelJSON(Boolean ShowWriteOnly)
        {

            var json = new JObject();

            if (Node21 is IDeviceModelNode deviceModel)
                json["ocpp2.1"] = ComponentsJSON(deviceModel.AllComponentConfigs, ShowWriteOnly);

            if (Node16 is OCPPv1_6.CP.IConfigurationKeysNode configurationKeys)
                json["ocpp1.6"] = ConfigurationKeysJSON(configurationKeys.ConfigurationKeySources);

            return json;

        }

        #endregion


        #region (static) NameOf(Component)

        /// <summary>
        /// A component as the tree names it: "Name", "Name[instance]",
        /// "Name @ EVSE 1" or "Name[instance] @ EVSE 1/2".
        /// </summary>
        public static String NameOf(ComponentConfig Component)

            => Component.Name +
               (Component.Instance is not null ? $"[{Component.Instance}]" : "") +
               (Component.EVSE     is not null
                    ? $" @ EVSE {Component.EVSE.Id}{(Component.EVSE.ConnectorId.HasValue ? $"/{Component.EVSE.ConnectorId}" : "")}"
                    : "");

        #endregion

        #region (static) NameOf(Variable)

        /// <summary>
        /// A variable as the tree names it: "Name" or "Name[instance]".
        /// </summary>
        public static String NameOf(VariableConfig Variable)

            => Variable.Name +
               (Variable.Instance is not null ? $"[{Variable.Instance}]" : "");

        #endregion

        #region (static) ComponentsJSON(Components, ShowWriteOnly)

        /// <summary>
        /// The given components, each with its variables and what they say now.
        /// </summary>
        /// <param name="Components">The components of an OCPP 2.1 node.</param>
        /// <param name="ShowWriteOnly">Whether the values OCPP keeps WriteOnly are shown too.</param>
        public static JObject ComponentsJSON(IEnumerable<ComponentConfig>  Components,
                                             Boolean                       ShowWriteOnly)
        {

            var json = new JObject();

            foreach (var component in Components.OrderBy(component => component.Name,                            StringComparer.OrdinalIgnoreCase).
                                                 ThenBy (component => component.EVSE?.Id.Value          ?? 0).
                                                 ThenBy (component => component.EVSE?.ConnectorId?.Value ?? 0).
                                                 ThenBy (component => component.Instance ?? "",                  StringComparer.OrdinalIgnoreCase))
            {

                var variables = new JObject();

                foreach (var variable in component.VariableConfigs.OrderBy(variable => variable.Name,             StringComparer.OrdinalIgnoreCase).
                                                                   ThenBy (variable => variable.Instance ?? "",   StringComparer.OrdinalIgnoreCase))
                    variables[Unique(variables, NameOf(variable))] = VariableJSON(variable, ShowWriteOnly);

                var name = Unique(json, NameOf(component));

                if (component.Description is not null && component.Description.IsNotNullOrEmpty())
                    variables.AddFirst(new JProperty("@description", component.Description.FirstText()));

                json[name] = variables;

            }

            return json;

        }

        #endregion

        #region (static) VariableJSON(Variable, ShowWriteOnly)

        /// <summary>
        /// A variable: what it says now - or that it is hidden -, its attribute
        /// and its characteristics.
        /// </summary>
        /// <param name="Variable">A variable of a component.</param>
        /// <param name="ShowWriteOnly">Whether its value is shown where OCPP keeps it WriteOnly.</param>
        public static JObject VariableJSON(VariableConfig  Variable,
                                           Boolean         ShowWriteOnly)
        {

            var mutability  = Variable.Attributes?.Mutability ?? MutabilityTypes.ReadWrite;
            var json        = new JObject();

            if (mutability == MutabilityTypes.WriteOnly && !ShowWriteOnly)
                json["hidden"]  = true;

            else
            {
                try
                {
                    json["value"] = Variable.Value is String value
                                        ? new JValue(value)
                                        : JValue.CreateNull();
                }
                catch (Exception e)
                {
                    json["value"] = JValue.CreateNull();
                    json["error"] = e.Message;
                }
            }

            json["type"]        = (Variable.Attributes?.Type ?? AttributeTypes.Actual).AsText();
            json["mutability"]  = mutability.AsText();

            if (Variable.Attributes?.Persistent == true)
                json["persistent"]  = true;

            if (Variable.Attributes?.Constant   == true)
                json["constant"]    = true;

            if (Variable.IsSecret)
                json["secret"]      = true;

            if (Variable.Characteristics is not null)
                json["characteristics"] = Variable.Characteristics.ToJSON();

            if (Variable.Description is not null && Variable.Description.IsNotNullOrEmpty())
                json["description"] = Variable.Description.FirstText();

            return json;

        }

        #endregion

        #region (static) ConfigurationKeysJSON(Sources)

        /// <summary>
        /// The configuration keys of OCPP 1.6 the given sources have now, by
        /// name, each with what it says, whether it is read-only and of which
        /// type and unit its value is.
        /// </summary>
        /// <param name="Sources">The sources of configuration keys of an OCPP 1.6 node.</param>
        public static JObject ConfigurationKeysJSON(IEnumerable<OCPPv1_6.CP.IConfigurationKeySource> Sources)
        {

            var keys = new List<OCPPv1_6.ConfigurationKey>();

            foreach (var source in Sources)
            {
                foreach (var name in source.Keys)
                {
                    try
                    {
                        if (source.Get(name) is OCPPv1_6.ConfigurationKey key)
                            keys.Add(key);
                    }
                    catch
                    { }
                }
            }

            var json = new JObject();

            foreach (var key in keys.OrderBy(key => key.Key, StringComparer.OrdinalIgnoreCase))
            {

                var entry = new JObject();

                if (key.AccessRights == OCPPv1_6.AccessRights.WriteOnly && key.Value is null)
                    entry["hidden"] = true;

                else
                    entry["value"]  = key.Value is String value
                                          ? new JValue(value)
                                          : JValue.CreateNull();

                entry["accessRights"]  = key.AccessRights.ToString();
                entry["readonly"]      = key.AccessRights == OCPPv1_6.AccessRights.ReadOnly;
                entry["valueType"]     = key.ValueType.   ToString();
                entry["unit"]          = key.ValueUnit.   ToString();

                json[Unique(json, key.Key)] = entry;

            }

            return json;

        }

        #endregion

        #region (private static) Unique(Object, Name)

        /// <summary>
        /// The name, or - where the object has it already - the name with
        /// " (2)", " (3)" ... : no entry of the tree hides another.
        /// </summary>
        private static String Unique(JObject Object, String Name)
        {

            if (!Object.ContainsKey(Name))
                return Name;

            for (var number = 2; ; number++)
            {
                var name = $"{Name} ({number})";
                if (!Object.ContainsKey(name))
                    return name;
            }

        }

        #endregion

    }

}
