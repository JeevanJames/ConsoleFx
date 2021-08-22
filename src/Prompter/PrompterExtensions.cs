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

using System;
using System.Collections.Generic;

using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Prompter.Questions;

namespace ConsoleFx.Prompter
{
    public static class PrompterExtensions
    {
        public static PrompterFlow Input(this PrompterFlow prompter, string name, FunctionOrColorString message,
            Action<InputQuestion> setupQuestion = null)
        {
            var question = new InputQuestion(name, message);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static PrompterFlow Password(this PrompterFlow prompter, string name, FunctionOrColorString message,
            Action<PasswordQuestion> setupQuestion = null)
        {
            var question = new PasswordQuestion(name, message);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static PrompterFlow Confirm(this PrompterFlow prompter, string name, FunctionOrColorString message,
            bool @default = false, Action<ConfirmQuestion> setupQuestion = null)
        {
            var question = new ConfirmQuestion(name, message, @default);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static PrompterFlow List(this PrompterFlow prompter, string name, FunctionOrColorString message,
            IEnumerable<string> choices, Action<ListQuestion> setupQuestion = null)
        {
            var question = new ListQuestion(name, message, choices);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        //TODO: Enforce a converter parameter here
        public static PrompterFlow List<TValue>(this PrompterFlow prompter, string name, FunctionOrColorString message,
            IEnumerable<string> choices, Func<int, TValue> converter, Action<ListQuestion<TValue>> setupQuestion = null)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));
            var question = new ListQuestion<TValue>(name, message, choices);
            question.Transform(converter);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static PrompterFlow Checkbox(this PrompterFlow prompter, string name, FunctionOrColorString message,
            IEnumerable<string> choices, Action<CheckboxQuestion> setupQuestion = null)
        {
            var question = new CheckboxQuestion(name, message, choices);
            setupQuestion?.Invoke(question);
            prompter.Add(question);
            return prompter;
        }

        public static PrompterFlow Text(this PrompterFlow prompter, FunctionOrColorString text,
            Action<StaticText> setupStaticText = null)
        {
            var staticText = new StaticText(text);
            setupStaticText?.Invoke(staticText);
            prompter.Add(staticText);
            return prompter;
        }

        public static PrompterFlow BlankLine(this PrompterFlow prompter, Action<StaticText> setupStaticText = null)
        {
            var staticText = new StaticText(ColorString.Empty);
            setupStaticText?.Invoke(staticText);
            prompter.Add(staticText);
            return prompter;
        }

        public static PrompterFlow Separator(this PrompterFlow prompter, char separator = '=',
            Action<StaticText> setupStaticText = null)
        {
            var staticText = new StaticText(new ColorString(new string(separator, Console.WindowWidth)));
            setupStaticText?.Invoke(staticText);
            prompter.Add(staticText);
            return prompter;
        }

        public static PrompterFlow AsyncUpdateFlow(this PrompterFlow prompter, AsyncUpdateFlowAction updateFlowAction)
        {
            prompter.Add(new AsyncUpdateFlowItem(updateFlowAction));
            return prompter;
        }

        public static PrompterFlow UpdateFlow(this PrompterFlow prompter, UpdateFlowAction updateFlowAction)
        {
            prompter.Add(new UpdateFlowItem(updateFlowAction));
            return prompter;
        }
    }
}
