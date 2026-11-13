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

using System.Runtime.CompilerServices;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// When the response to a request is out: what drops the connection a pin set or a change of trust
    /// came over waits for it - dropped before, the connection takes the answer with it, and the CSMS
    /// never learns that its change was taken.
    /// </summary>
    /// <remarks>
    /// Whoever answers tells <see cref="Sent"/> from the event of the response sent - OnSetVariablesResponseSent
    /// of an OCPP 2.1 node, say. <see cref="Answered"/> is asked while the request is still being handled,
    /// before its response can be out: asked later, the response may be gone already, and what waits
    /// for it would wait the whole <see cref="AnswerWait"/>.
    /// </remarks>
    public sealed class ResponsesSent
    {

        #region Data

        /// <summary>
        /// How long what waits for the response to a request waits at most: a response that could not be
        /// sent at all says nothing of it.
        /// </summary>
        public static readonly TimeSpan AnswerWait = TimeSpan.FromSeconds(10);

        // The requests being answered, until their response is out.
        private readonly ConditionalWeakTable<Object, TaskCompletionSource> answering = new ();

        #endregion


        #region Answered(Request)

        /// <summary>
        /// Done once the response to the given request is out - sent, or found not to be sendable - and
        /// after <see cref="AnswerWait"/> at most; at once where there is no request.
        /// </summary>
        /// <param name="Request">The request, while it is being handled.</param>
        public Task Answered(Object? Request)
        {

            if (Request is null)
                return Task.CompletedTask;

            var answered = answering.GetValue(Request, _ => new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));

            return Task.WhenAny(answered.Task, Task.Delay(AnswerWait));

        }

        #endregion

        #region Sent(Request)

        /// <summary>
        /// The response to the given request is out.
        /// </summary>
        /// <param name="Request">The request.</param>
        public void Sent(Object Request)
        {
            if (answering.TryGetValue(Request, out var answered))
                answered.TrySetResult();
        }

        #endregion

    }

}
