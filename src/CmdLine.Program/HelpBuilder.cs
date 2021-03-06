﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Program
{
    public abstract class HelpBuilder : NamedObject
    {
        protected HelpBuilder()
        {
        }

        protected HelpBuilder(IDictionary<string, bool> names)
            : base(names)
        {
        }

        /// <summary>
        ///     Override this method to display the help for the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> for which to display help.</param>
        public abstract void DisplayHelp(Command command);

        public virtual void VerifyHelp(Command command)
        {
        }
    }
}
