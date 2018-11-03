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
using System.Collections.Generic;

using ConsoleFx.Prompter.Questions;

namespace ConsoleFx.Prompter
{
    public static class PrompterExtensions
    {
        public static InputQuestion Input(this Prompter prompter, string name, FunctionOrValue<string> message)
        {
            var question = new InputQuestion(name, message);
            prompter.AddQuestion(question);
            return question;
        }

        public static PasswordQuestion Password(this Prompter prompter, string name, FunctionOrValue<string> message)
        {
            var question = new PasswordQuestion(name, message);
            prompter.AddQuestion(question);
            return question;
        }

        public static ConfirmQuestion Confirm(this Prompter prompter, string name, FunctionOrValue<string> message, bool @default = false)
        {
            var question = new ConfirmQuestion(name, message, @default);
            prompter.AddQuestion(question);
            return question;
        }

        public static ListQuestion List(this Prompter prompter, string name, FunctionOrValue<string> message, IEnumerable<string> choices)
        {
            var question = new ListQuestion(name, message, choices);
            prompter.AddQuestion(question);
            return question;
        }

        public static CheckboxQuestion Checkbox(this Prompter prompter, string name, FunctionOrValue<string> message, IEnumerable<string> choices)
        {
            var question = new CheckboxQuestion(name, message, choices);
            prompter.AddQuestion(question);
            return question;
        }

        public static StaticText Text(this Prompter prompter, FunctionOrValue<string> text)
        {
            var staticText = new StaticText(text);
            prompter.AddQuestion(staticText);
            return staticText;
        }

        public static StaticText BlankLine(this Prompter prompter)
        {
            var staticText = new StaticText(string.Empty);
            prompter.AddQuestion(staticText);
            return staticText;
        }

        public static StaticText Separator(this Prompter prompter, char separator = '=')
        {
            var staticText = new StaticText(new string(separator, Console.WindowWidth));
            prompter.AddQuestion(staticText);
            return staticText;
        }
    }
}