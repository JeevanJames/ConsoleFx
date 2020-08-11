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

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class UriValidator : SingleMessageValidator
    {
        public UriValidator(UriKind uriKind = UriKind.RelativeOrAbsolute)
            : base(typeof(Uri), Messages.Uri)
        {
            UriKind = uriKind;
        }

        public UriKind UriKind { get; }

        protected override object ValidateAsString(string parameterValue)
        {
            if (!Uri.TryCreate(parameterValue, UriKind, out Uri uri))
                ValidationFailed(Message, parameterValue);
            return uri;
        }
    }

    public static class UriValidatorExtensions
    {
        public static Argument ValidateAsUri(this Argument argument, UriKind uriKind = UriKind.RelativeOrAbsolute,
            string message = null)
        {
            var validator = new UriValidator(uriKind);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsUri(this Option option, UriKind uriKind = UriKind.RelativeOrAbsolute,
            int parameterIndex = -1, string message = null)
        {
            var validator = new UriValidator(uriKind);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator).TypeAs<Uri>();
        }
    }
}
