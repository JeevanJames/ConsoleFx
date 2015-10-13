#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

namespace ConsoleFx.Parser.Validators
{
    public class UriValidator : AdditionalChecksValidator<Uri>
    {
        public UriKind UriKind { get; }

        public UriValidator(UriKind uriKind)
        {
            UriKind = uriKind;
        }

        protected override Uri PrimaryChecks(string parameterValue)
        {
            Uri uri;
            if (!Uri.TryCreate(parameterValue, UriKind, out uri))
                ValidationFailed(parameterValue, UriKindMessage);
            return uri;
        }

        public string UriKindMessage { get; set; } = Messages.Uri;
    }

    /// <summary>
    /// Multiple message validator that allows derived classes to perform additional checks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AdditionalChecksValidator<T> : MultipleMessageValidator
    {
        public sealed override void Validate(string parameterValue)
        {
            T value = PrimaryChecks(parameterValue);
            AdditionalChecks(value);
        }

        protected abstract T PrimaryChecks(string parameterValue);

        protected virtual void AdditionalChecks(T value)
        {
        }
    }
}
