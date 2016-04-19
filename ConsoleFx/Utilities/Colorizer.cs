#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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
using System.Linq;

namespace ConsoleFx.Utilities
{
    public sealed class Colorizer
    {
        public Colorizer Out(string text, ConsoleColor foreColor, ConsoleColor backColor) =>
            Out(this, text, foreColor, backColor);

        public Colorizer Out(string text, ConsoleColor foreColor) => Out(this, text, foreColor, null);

        public Colorizer Out(string text, string colorName)
        {
            if (colorName == null)
                throw new ArgumentNullException(nameof(colorName));
            PaletteEntry color = Palette.GetColor(text);
            if (color == null)
                throw new ArgumentException($"Could not find palette color named {colorName}. Ensure you add it to the palette using the Palette.AddXXXX methods.", nameof(colorName));
            return this;
        }

        private static Colorizer Out(Colorizer colorizer, string text, ConsoleColor? foreColor, ConsoleColor? backColor)
        {
            if (foreColor.HasValue)
                Console.ForegroundColor = foreColor.Value;
            if (backColor.HasValue)
                Console.BackgroundColor = backColor.Value;
            Console.Write(text ?? string.Empty);
            Console.ResetColor();
            return colorizer ?? new Colorizer();
        }
    }

    public static class Palette
    {
        private static readonly List<PaletteEntry> _colors = new List<PaletteEntry>();

        public static void Add(string name, ConsoleColor foreColor, ConsoleColor backColor)
        {
            _colors.Add(new PaletteEntry(name, foreColor, backColor));
        }

        public static void AddForeColor(string name, ConsoleColor foreColor)
        {
            _colors.Add(new PaletteEntry(name, foreColor, null));
        }

        public static void AddBackColor(string name, ConsoleColor backColor)
        {
            _colors.Add(new PaletteEntry(name, null, backColor));
        }

        internal static PaletteEntry GetColor(string name) =>
            _colors.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    internal sealed class PaletteEntry
    {
        internal PaletteEntry(string name, ConsoleColor? foreColor, ConsoleColor? backColor)
        {
            Name = name;
            ForeColor = foreColor;
            BackColor = backColor;
        }

        internal string Name { get; }

        internal ConsoleColor? ForeColor { get; }

        internal ConsoleColor? BackColor { get; }
    }
}