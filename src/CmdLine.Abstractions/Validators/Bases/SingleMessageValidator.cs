﻿#region --- License & Copyright Notice ---
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

using System;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    /// <summary>
    ///     Base class for validators that only have one possible type of validation failure. In this
    ///     case, the class provides a Message property with a default value that can be changed.
    /// </summary>
    /// <typeparam name="T">The actual type of the value being validated.</typeparam>
    public abstract class SingleMessageValidator<T> : Validator<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleMessageValidator{T}"/> class.
        /// </summary>
        /// <param name="message">The validation failure message.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="message"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="message"/> is empty or only white spaces.</exception>
        protected SingleMessageValidator(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            if (message.Trim().Length == 0)
                throw new ArgumentException("Specify a valid error message.", nameof(message));
            Message = message;
        }

        /// <summary>
        ///     Gets or sets the error message to be displayed if the validation fails.
        /// </summary>
        public string Message { get; set; }
    }
}
