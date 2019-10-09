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

namespace ConsoleFx.Prompter.Questions
{
    public abstract class TextEntryQuestion<TValue> : Question<string, TValue>
    {
        protected TextEntryQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }

        public TextEntryQuestion<TValue> Required(bool allowWhitespaceOnly = false)
        {
            IsRequired = true;
            AllowWhitespaceOnly = allowWhitespaceOnly;
            return this;
        }

        internal bool IsRequired { get; set; }

        internal bool AllowWhitespaceOnly { get; set; }
    }
}
