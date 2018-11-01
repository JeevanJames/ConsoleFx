#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

namespace ConsoleFx.Prompter.Questions
{
    public abstract class TextEntryQuestion : Question
    {
        protected TextEntryQuestion(string name, FunctionOrValue<string> message) : base(name, message)
        {
        }

        public TextEntryQuestion Validate(Func<string, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (str, _) => validator(str);
            return this;
        }

        public TextEntryQuestion Validate(Func<string, dynamic, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            RawValueValidator = (str, ans) => validator(str, ans);
            return this;
        }

        public TextEntryQuestion Required(bool allowWhitespaceOnly = false)
        {
            IsRequired = true;
            AllowWhitespaceOnly = allowWhitespaceOnly;
            return this;
        }

        internal bool IsRequired { get; set; }

        internal bool AllowWhitespaceOnly { get; set; }
    }
}