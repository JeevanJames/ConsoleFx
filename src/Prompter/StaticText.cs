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

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public sealed class StaticText : PromptItem
    {
        private readonly AskerFn _askerFn;

        internal StaticText(FunctionOrColorString message)
            : base(message)
        {
            _askerFn = (q, ans) =>
            {
                ColorString staticText = q.Message.Resolve(ans);
                if (staticText != null)
                    ConsoleEx.PrintLine(staticText);
                return null;
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }
}
