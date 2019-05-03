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

using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    public readonly struct Clr
    {
        public static InstanceClr Black => new InstanceClr(CColor.Black, null);

        public static InstanceClr Blue => new InstanceClr(CColor.Blue, null);

        public static InstanceClr Cyan => new InstanceClr(CColor.Cyan, null);

        public static InstanceClr DkBlue => new InstanceClr(CColor.DkBlue, null);

        public static InstanceClr DkCyan => new InstanceClr(CColor.DkCyan, null);

        public static InstanceClr DkGray => new InstanceClr(CColor.DkGray, null);

        public static InstanceClr DkGreen => new InstanceClr(CColor.DkGreen, null);

        public static InstanceClr DkMagenta => new InstanceClr(CColor.DkMagenta, null);

        public static InstanceClr DkRed => new InstanceClr(CColor.DkRed, null);

        public static InstanceClr DkYellow => new InstanceClr(CColor.DkYellow, null);

        public static InstanceClr Gray => new InstanceClr(CColor.DkGray, null);

        public static InstanceClr Green => new InstanceClr(CColor.Green, null);

        public static InstanceClr Magenta => new InstanceClr(CColor.Magenta, null);

        public static InstanceClr Red => new InstanceClr(CColor.Red, null);

        public static InstanceClr White => new InstanceClr(CColor.White, null);

        public static InstanceClr Yellow => new InstanceClr(CColor.Yellow, null);

        public static InstanceClr Reset => new InstanceClr(CColor.Reset, null);

        public static InstanceClr BgBlack => new InstanceClr(null, CColor.BgBlack);

        public static InstanceClr BgBlue => new InstanceClr(null, CColor.BgBlue);

        public static InstanceClr BgCyan => new InstanceClr(null, CColor.BgCyan);

        public static InstanceClr BgDkBlue => new InstanceClr(null, CColor.BgDkBlue);

        public static InstanceClr BgDkCyan => new InstanceClr(null, CColor.BgDkCyan);

        public static InstanceClr BgDkGray => new InstanceClr(null, CColor.BgDkGray);

        public static InstanceClr BgDkGreen => new InstanceClr(null, CColor.BgDkGreen);

        public static InstanceClr BgDkMagenta => new InstanceClr(null, CColor.BgDkMagenta);

        public static InstanceClr BgDkRed => new InstanceClr(null, CColor.BgDkRed);

        public static InstanceClr BgDkYellow => new InstanceClr(null, CColor.BgDkYellow);

        public static InstanceClr BgGray => new InstanceClr(null, CColor.BgGray);

        public static InstanceClr BgGreen => new InstanceClr(null, CColor.BgGreen);

        public static InstanceClr BgMagenta => new InstanceClr(null, CColor.BgMagenta);

        public static InstanceClr BgRed => new InstanceClr(null, CColor.BgRed);

        public static InstanceClr BgWhite => new InstanceClr(null, CColor.BgWhite);

        public static InstanceClr BgYellow => new InstanceClr(null, CColor.BgYellow);

        public static InstanceClr BgReset => new InstanceClr(null, CColor.BgReset);
    }

    public readonly struct InstanceClr
    {
        private readonly CColor?[] _colors;

        internal InstanceClr(CColor? foregroundColor, CColor? backgroundColor)
        {
            _colors = new CColor?[2] { foregroundColor, backgroundColor };
        }

        internal InstanceClr(InstanceClr clr, CColor? foregroundColor, CColor? backgroundColor)
        {
            _colors = new CColor?[2]
            {
                foregroundColor ?? clr._colors[0],
                backgroundColor ?? clr._colors[1],
            };
        }

        public override string ToString()
        {
            if (!_colors[0].HasValue && !_colors[1].HasValue)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (CColor? color in _colors)
            {
                if (!color.HasValue)
                    continue;
                if (sb.Length > 0)
                    sb.Append(".");
                sb.Append(color.Value.ToString());
            }

            sb.Insert(0, "[").Append("]");
            return sb.ToString();
        }

        public InstanceClr Black => new InstanceClr(this, CColor.Black, null);

        public InstanceClr Blue => new InstanceClr(this, CColor.Blue, null);

        public InstanceClr Cyan => new InstanceClr(this, CColor.Cyan, null);

        public InstanceClr DkBlue => new InstanceClr(this, CColor.DkBlue, null);

        public InstanceClr DkCyan => new InstanceClr(this, CColor.DkCyan, null);

        public InstanceClr DkGray => new InstanceClr(this, CColor.DkGray, null);

        public InstanceClr DkGreen => new InstanceClr(this, CColor.DkGreen, null);

        public InstanceClr DkMagenta => new InstanceClr(this, CColor.DkMagenta, null);

        public InstanceClr DkRed => new InstanceClr(this, CColor.DkRed, null);

        public InstanceClr DkYellow => new InstanceClr(this, CColor.DkYellow, null);

        public InstanceClr Gray => new InstanceClr(this, CColor.DkGray, null);

        public InstanceClr Green => new InstanceClr(this, CColor.Green, null);

        public InstanceClr Magenta => new InstanceClr(this, CColor.Magenta, null);

        public InstanceClr Red => new InstanceClr(this, CColor.Red, null);

        public InstanceClr White => new InstanceClr(this, CColor.White, null);

        public InstanceClr Yellow => new InstanceClr(this, CColor.Yellow, null);

        public InstanceClr Reset => new InstanceClr(this, CColor.Reset, null);

        public InstanceClr BgBlack => new InstanceClr(this, null, CColor.BgBlack);

        public InstanceClr BgBlue => new InstanceClr(this, null, CColor.BgBlue);

        public InstanceClr BgCyan => new InstanceClr(this, null, CColor.BgCyan);

        public InstanceClr BgDkBlue => new InstanceClr(this, null, CColor.BgDkBlue);

        public InstanceClr BgDkCyan => new InstanceClr(this, null, CColor.BgDkCyan);

        public InstanceClr BgDkGray => new InstanceClr(this, null, CColor.BgDkGray);

        public InstanceClr BgDkGreen => new InstanceClr(this, null, CColor.BgDkGreen);

        public InstanceClr BgDkMagenta => new InstanceClr(this, null, CColor.BgDkMagenta);

        public InstanceClr BgDkRed => new InstanceClr(this, null, CColor.BgDkRed);

        public InstanceClr BgDkYellow => new InstanceClr(this, null, CColor.BgDkYellow);

        public InstanceClr BgGray => new InstanceClr(this, null, CColor.BgGray);

        public InstanceClr BgGreen => new InstanceClr(this, null, CColor.BgGreen);

        public InstanceClr BgMagenta => new InstanceClr(this, null, CColor.BgMagenta);

        public InstanceClr BgRed => new InstanceClr(this, null, CColor.BgRed);

        public InstanceClr BgWhite => new InstanceClr(this, null, CColor.BgWhite);

        public InstanceClr BgYellow => new InstanceClr(this, null, CColor.BgYellow);

        public InstanceClr BgReset => new InstanceClr(this, null, CColor.BgReset);
    }
}
