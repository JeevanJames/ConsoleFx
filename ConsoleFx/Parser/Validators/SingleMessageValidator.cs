﻿#region --- License & Copyright Notice ---
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

namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    ///     Base class for validators that only have one possible type of validation failure. In this case, the class provides
    ///     an OOTB Message property.
    /// </summary>
    /// <typeparam name="T">The actual type of the value being validated.</typeparam>
    public abstract class SingleMessageValidator<T> : Validator<T>
    {
        protected SingleMessageValidator(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The error message to be displayed if the validation fails.
        /// </summary>
        public string Message { get; set; }
    }
}