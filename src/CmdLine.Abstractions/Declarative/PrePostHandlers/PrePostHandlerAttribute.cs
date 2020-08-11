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

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for attributes that can run code before or after a <see cref="Command"/> handler
    ///     is executed, or when an exception occurs during the execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PrePostHandlerAttribute : Attribute
    {
        /// <summary>
        ///     Override this method to run code before the <see cref="Command"/> handler is executed.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to be executed.</param>
        public virtual void BeforeHandler(Command command)
        {
        }

        /// <summary>
        ///     Override this method to run code whenever the <see cref="Command"/> handler throws
        ///     an unhandled exception.
        /// </summary>
        /// <param name="ex">The thrown exception.</param>
        /// <param name="command">The <see cref="Command"/> that was executed.</param>
        /// <returns>Integer exit code that should be returned from the application.</returns>
        public virtual int? OnException(Exception ex, Command command)
        {
            return null;
        }

        /// <summary>
        ///     Override this method to run code after the <see cref="Command"/> handler is executed.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> that was executed.</param>
        public virtual void AfterHandler(Command command)
        {
        }
    }
}
