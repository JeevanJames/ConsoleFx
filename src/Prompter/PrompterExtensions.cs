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
using System.Collections.Generic;

using ConsoleFx.Prompter.Questions;

namespace ConsoleFx.Prompter
{
    public static class PrompterExtensions
    {
        public static Prompter Input(this Prompter prompter, string name, FunctionOrValue<string> message,
            Action<InputQuestion> setupQuestion = null)
        {
            var question = new InputQuestion(name, message);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static Prompter Password(this Prompter prompter, string name, FunctionOrValue<string> message,
            Action<PasswordQuestion> setupQuestion = null)
        {
            var question = new PasswordQuestion(name, message);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static Prompter Confirm(this Prompter prompter, string name, FunctionOrValue<string> message,
            bool @default = false, Action<ConfirmQuestion> setupQuestion = null)
        {
            var question = new ConfirmQuestion(name, message, @default);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static Prompter List(this Prompter prompter, string name, FunctionOrValue<string> message,
            IEnumerable<string> choices, Action<ListQuestion> setupQuestion = null)
        {
            var question = new ListQuestion(name, message, choices);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        //TODO: Enforce a converter parameter here
        public static Prompter List<TValue>(this Prompter prompter, string name, FunctionOrValue<string> message,
            IEnumerable<string> choices, Action<ListQuestion<TValue>> setupQuestion = null)
        {
            var question = new ListQuestion<TValue>(name, message, choices);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static Prompter Checkbox(this Prompter prompter, string name, FunctionOrValue<string> message,
            IEnumerable<string> choices, Action<CheckboxQuestion> setupQuestion = null)
        {
            var question = new CheckboxQuestion(name, message, choices);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static Prompter Text(this Prompter prompter, FunctionOrValue<string> text,
            Action<StaticText> setupStaticText = null)
        {
            var staticText = new StaticText(text);
            setupStaticText?.Invoke(staticText);
            prompter.Add(staticText);
            return prompter;
        }

        public static Prompter BlankLine(this Prompter prompter, Action<StaticText> setupStaticText = null)
        {
            var staticText = new StaticText(string.Empty);
            setupStaticText?.Invoke(staticText);
            prompter.Add(staticText);
            return prompter;
        }

        public static Prompter Separator(this Prompter prompter, char separator = '=',
            Action<StaticText> setupStaticText = null)
        {
            var staticText = new StaticText(new string(separator, Console.WindowWidth));
            setupStaticText?.Invoke(staticText);
            prompter.Add(staticText);
            return prompter;
        }
    }
}
