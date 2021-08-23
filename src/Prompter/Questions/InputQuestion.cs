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
            AskerFn = (q, ans) =>
            {
                bool Validator(string str)
                {
                    var question = (InputQuestion<TValue>)q;
                    if (string.IsNullOrEmpty(str) && question.DefaultValue.IsAssigned)
                        str = question.DefaultValue.Resolve(ans);
                    bool valid = (question.RawValueValidator is null) || question.RawValueValidator(str, ans).Valid;
                    var teq = (TextEntryQuestion<TValue>)q;
                    if (valid && teq.IsRequired)
                    {
                        return teq.AllowWhitespaceOnly
                            ? !string.IsNullOrEmpty(str)
                            : !string.IsNullOrWhiteSpace(str);
                    }

                    return valid;
                }

                return ConsoleEx.Prompt(new ColorString(q.Message.Resolve(ans),
                    PrompterFlow.Style.Question.ForeColor, PrompterFlow.Style.Question.BackColor).ToString(),
                    Validator);
            };
        }

        internal override AskerFn AskerFn { get; }
    }

    public sealed class InputQuestion : InputQuestion<string>
    {
        public InputQuestion(string name, FunctionOrColorString message)
            : base(name, message)
        {
        }
    }
}
