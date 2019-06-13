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

using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Prompter.Style;

namespace ConsoleFx.Prompter
{
    public sealed partial class Styling
    {
        private QuestionStyle _question;
        private InstructionStyle _instructions;

        public QuestionStyle Question
        {
            get => _question ?? (_question = new QuestionStyle());
            set => _question = value;
        }

        public InstructionStyle Instructions
        {
            get => _instructions ?? (_instructions = new InstructionStyle());
            set => _instructions = value;
        }
    }

    // Themes
    public sealed partial class Styling
    {
        public static readonly Styling NoTheme = new Styling();

        public static readonly Styling Terminal = new Styling
        {
            Question = { ForeColor = CColor.Green, },
            Instructions = { ForeColor = CColor.DkGreen, },
        };

        public static readonly Styling Ruby = new Styling
        {
            Question = { ForeColor = CColor.Magenta, },
        };

        public static readonly Styling Colorful = new Styling
        {
            Question =
            {
                ForeColor = CColor.Yellow,
                BackColor = CColor.DkMagenta,
            },
            Instructions =
            {
                ForeColor = CColor.White,
                BackColor = CColor.DkBlue,
            },
        };
    }
}
