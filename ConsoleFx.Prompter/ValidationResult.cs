#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
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


namespace ConsoleFx.Prompter
{
    public readonly struct ValidationResult
    {
        internal bool Valid { get; }

        internal string ErrorMessage { get; }

        internal ValidationResult(bool valid)
        {
            Valid = valid;
            ErrorMessage = null;
        }

        internal ValidationResult(string errorMessage)
        {
            Valid = errorMessage == null;
            ErrorMessage = errorMessage;
        }

        public static implicit operator ValidationResult(bool valid) =>
            new ValidationResult(valid);

        public static implicit operator ValidationResult(string errorMessage) =>
            new ValidationResult(errorMessage);
    }
}