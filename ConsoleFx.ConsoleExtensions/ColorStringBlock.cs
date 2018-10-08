#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
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

using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    /// Represents a colored text block in a <see cref="ColorString"/> instance.
    /// </summary>
    public sealed class ColorStringBlock
    {
        internal ColorStringBlock(string text, CColor? foreColor = null, CColor? backColor = null)
        {
            Text = text;
            ForeColor = foreColor;
            BackColor = backColor;
        }

        public string Text { get; }

        public CColor? ForeColor { get; }

        public CColor? BackColor { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (ForeColor.HasValue || BackColor.HasValue)
            {
                sb.Append("[");
                if (ForeColor.HasValue)
                    sb.Append(ForeColor.Value);
                if (ForeColor.HasValue && BackColor.HasValue)
                    sb.Append(".");
                if (BackColor.HasValue)
                    sb.Append(BackColor.Value);
                sb.Append("]");
            }
            if (!string.IsNullOrEmpty(Text))
                sb.Append(Text);
            return sb.ToString();
        }
    }
}
