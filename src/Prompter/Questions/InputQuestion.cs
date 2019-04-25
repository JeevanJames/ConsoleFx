#region --- License & Copyright Notice ---
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

using System;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public sealed class InputQuestion : TextEntryQuestion
    {
        private readonly AskerFn _askerFn;

        internal InputQuestion(string name, FunctionOrValue<string> message)
            : base(name, message)
        {
            _askerFn = (q, ans) =>
            {
                bool validator(string str)
                {
                    if (string.IsNullOrEmpty(str))
                        str = q.DefaultValue.Resolve(ans);
                    bool valid = (q.Validator == null) || q.Validator(str, ans).Valid;
                    var teq = (TextEntryQuestion)q;
                    if (valid && teq.IsRequired)
                    {
                        return teq.AllowWhitespaceOnly
                            ? !string.IsNullOrEmpty(str)
                            : !string.IsNullOrWhiteSpace(str);
                    }

                    return valid;
                }

                return ConsoleEx.Prompt(new ColorString().Cyan(q.Message.Resolve(ans)), (Func<string, bool>)validator);
            };
        }

        internal override AskerFn AskerFn => _askerFn;
    }
}
