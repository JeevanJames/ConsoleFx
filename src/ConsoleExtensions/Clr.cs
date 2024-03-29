﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    ///     Structure that represents a foreground and background color.
    /// </summary>
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

        public static InstanceClr BgBlack => new InstanceClr(null, CColor.Black);

        public static InstanceClr BgBlue => new InstanceClr(null, CColor.Blue);

        public static InstanceClr BgCyan => new InstanceClr(null, CColor.Cyan);

        public static InstanceClr BgDkBlue => new InstanceClr(null, CColor.DkBlue);

        public static InstanceClr BgDkCyan => new InstanceClr(null, CColor.DkCyan);

        public static InstanceClr BgDkGray => new InstanceClr(null, CColor.DkGray);

        public static InstanceClr BgDkGreen => new InstanceClr(null, CColor.DkGreen);

        public static InstanceClr BgDkMagenta => new InstanceClr(null, CColor.DkMagenta);

        public static InstanceClr BgDkRed => new InstanceClr(null, CColor.DkRed);

        public static InstanceClr BgDkYellow => new InstanceClr(null, CColor.DkYellow);

        public static InstanceClr BgGray => new InstanceClr(null, CColor.Gray);

        public static InstanceClr BgGreen => new InstanceClr(null, CColor.Green);

        public static InstanceClr BgMagenta => new InstanceClr(null, CColor.Magenta);

        public static InstanceClr BgRed => new InstanceClr(null, CColor.Red);

        public static InstanceClr BgWhite => new InstanceClr(null, CColor.White);

        public static InstanceClr BgYellow => new InstanceClr(null, CColor.Yellow);

        public static InstanceClr BgReset => new InstanceClr(null, CColor.Reset);
    }

    public readonly struct InstanceClr
    {
        private readonly CColor?[] _colors;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InstanceClr"/> struct with the specified
        ///     <paramref name="foregroundColor"/> and <paramref name="backgroundColor"/>.
        /// </summary>
        /// <param name="foregroundColor">Optional foreground color.</param>
        /// <param name="backgroundColor">Optional background color.</param>
        internal InstanceClr(CColor? foregroundColor, CColor? backgroundColor)
        {
            _colors = new CColor?[2] { foregroundColor, backgroundColor };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InstanceClr"/> struct with the specified
        ///     <paramref name="foregroundColor"/> and <paramref name="backgroundColor"/>.
        ///     <para />
        ///     If either of the colors are not specified, they are initialized from the specified
        ///     <paramref name="clr"/> value.
        /// </summary>
        /// <param name="clr">
        ///     The <see cref="InstanceClr"/> structure to initialize this instance from, if either <paramref name="foregroundColor"/> or <paramref name="backgroundColor"/> are not specified.
        /// </param>
        /// <param name="foregroundColor">Optional foreground color.</param>
        /// <param name="backgroundColor">Optional background color.</param>
        /// <remarks>
        ///     This constructor can only be called from an <see cref="InstanceClr"/> instance only,
        ///     hence it is private.
        /// </remarks>
        private InstanceClr(InstanceClr clr, CColor? foregroundColor, CColor? backgroundColor)
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
            if (_colors[0].HasValue)
                sb.Append(_colors[0].Value.ToString());
            if (_colors[1].HasValue)
            {
                if (sb.Length > 0)
                    sb.Append(".");
                sb.Append("Bg").Append(_colors[1].Value.ToString());
            }

            if (sb.Length > 0)
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

        public InstanceClr BgBlack => new InstanceClr(this, null, CColor.Black);

        public InstanceClr BgBlue => new InstanceClr(this, null, CColor.Blue);

        public InstanceClr BgCyan => new InstanceClr(this, null, CColor.Cyan);

        public InstanceClr BgDkBlue => new InstanceClr(this, null, CColor.DkBlue);

        public InstanceClr BgDkCyan => new InstanceClr(this, null, CColor.DkCyan);

        public InstanceClr BgDkGray => new InstanceClr(this, null, CColor.DkGray);

        public InstanceClr BgDkGreen => new InstanceClr(this, null, CColor.DkGreen);

        public InstanceClr BgDkMagenta => new InstanceClr(this, null, CColor.DkMagenta);

        public InstanceClr BgDkRed => new InstanceClr(this, null, CColor.DkRed);

        public InstanceClr BgDkYellow => new InstanceClr(this, null, CColor.DkYellow);

        public InstanceClr BgGray => new InstanceClr(this, null, CColor.Gray);

        public InstanceClr BgGreen => new InstanceClr(this, null, CColor.Green);

        public InstanceClr BgMagenta => new InstanceClr(this, null, CColor.Magenta);

        public InstanceClr BgRed => new InstanceClr(this, null, CColor.Red);

        public InstanceClr BgWhite => new InstanceClr(this, null, CColor.White);

        public InstanceClr BgYellow => new InstanceClr(this, null, CColor.Yellow);

        public InstanceClr BgReset => new InstanceClr(this, null, CColor.Reset);
    }
}
