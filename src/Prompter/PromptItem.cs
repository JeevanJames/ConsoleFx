#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Base class for any item that can be displayed by the prompter.
    ///     <para/>
    ///     This includes questions (<see cref="Question"/>) and static text (<see cref="StaticText"/>).
    /// </summary>
    public abstract class PromptItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PromptItem"/> class.
        /// </summary>
        /// <param name="message">The <paramref name="message"/> to display to the user.</param>
        protected PromptItem(FunctionOrColorString message)
        {
            Message = message;
        }

        public PromptItem When(AnswersFunc<bool> canAskFn)
        {
            CanAskFn = canAskFn;
            return this;
        }

        internal FunctionOrColorString Message { get; }

        internal AnswersFunc<bool> CanAskFn { get; set; }

        internal abstract AskerFn AskerFn { get; }

        internal bool CanAsk(dynamic answers)
        {
            return (CanAskFn is null) || (bool)CanAskFn(answers);
        }
    }
}
