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

using System;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        ///     Displays a message and waits for user input.
        /// </summary>
        /// <param name="message">A string or color string representing the message to be displayed</param>
        /// <returns>The input entered by the user</returns>
        public static string Prompt(ColorString message)
        {
            WriteColor(message);
            return Console.ReadLine();
        }
    }
}
