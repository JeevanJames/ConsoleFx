// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter.Questions
{
    public class InputQuestion<TValue> : TextEntryQuestion<TValue>
    {
        internal InputQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }

        /// <inheritdoc />
        internal override object Ask(dynamic answers)
        {
            bool Validator(string str)
            {
                if (string.IsNullOrEmpty(str) && DefaultValue.IsAssigned)
                    str = DefaultValue.Resolve(answers);
                bool valid = (RawValueValidator is null) || RawValueValidator(str, answers).Valid;
                if (valid && IsRequired)
                    return AllowWhitespaceOnly ? !string.IsNullOrEmpty(str) : !string.IsNullOrWhiteSpace(str);

                return valid;
            }

            return ConsoleEx.Prompt(new ColorString(Message.Resolve(answers),
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor).ToString(),
                Validator);
        }
    }

    public sealed class InputQuestion : InputQuestion<string>
    {
        public InputQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }
    }
}
