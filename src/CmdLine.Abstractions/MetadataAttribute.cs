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

using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataAttribute : Attribute
    {
        public abstract IEnumerable<KeyValuePair<string, object>> GetMetadata();

        /// <summary>
        ///     Helper method to assign the metadata values from this attribute to the specified
        ///     <paramref name="arg"/>.
        /// </summary>
        /// <typeparam name="TArg">The type of arg.</typeparam>
        /// <param name="arg">The arg to assign the metadata to.</param>
        public void AssignMetadata<TArg>(TArg arg)
            where TArg : Arg
        {
            IEnumerable<KeyValuePair<string, object>> metadata = GetMetadata();
            foreach (KeyValuePair<string, object> metadataItem in metadata)
                arg.Set(metadataItem.Key, metadataItem.Value);
        }
    }
}
