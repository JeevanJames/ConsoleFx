﻿#region --- License & Copyright Notice ---
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
using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for any attribute that can assign metadata to an <see cref="Arg"/>.
    ///     <para/>
    ///     The framework locates such attributes decorated on args and automatically assigns them to
    ///     the metadata of the arg.
    /// </summary>
    /// <remarks>
    ///     An example of metadata attributes are the Help attributes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataAttribute : Attribute
    {
        /// <summary>
        ///     Override this method to return one or more metadata for the arg decorated by this
        ///     attribute.
        /// </summary>
        /// <returns>One or more metadata key-value pairs.</returns>
        public abstract IEnumerable<KeyValuePair<string, object>> GetMetadata();

        /// <summary>
        ///     Helper method to assign the metadata values from this attribute to the specified
        ///     <paramref name="arg"/>.
        /// </summary>
        /// <typeparam name="TArg">The type of arg.</typeparam>
        /// <param name="arg">The arg to assign the metadata to.</param>
        public void AssignMetadata<TArg>(TArg arg)
            where TArg : IMetadataObject
        {
            // Validate applicable args for this attribute.
            //TODO: Simplify code
            ApplicableArgTypesForMetadata applicableArgs = GetApplicableArgTypes();
            switch (arg)
            {
                case Argument _:
                    if ((applicableArgs & ApplicableArgTypesForMetadata.Argument) != ApplicableArgTypesForMetadata.Argument)
                        throw new InvalidOperationException($"Cannot apply the {GetType().Name} attribute to an arg of type {typeof(TArg).Name}.");
                    break;
                case Option _:
                    if ((applicableArgs & ApplicableArgTypesForMetadata.Option) != ApplicableArgTypesForMetadata.Option)
                        throw new InvalidOperationException($"Cannot apply the {GetType().Name} attribute to an arg of type {typeof(TArg).Name}.");
                    break;
                case Command _:
                    if ((applicableArgs & ApplicableArgTypesForMetadata.Command) != ApplicableArgTypesForMetadata.Command)
                        throw new InvalidOperationException($"Cannot apply the {GetType().Name} attribute to an arg of type {typeof(TArg).Name}.");
                    break;
            }

            IEnumerable<KeyValuePair<string, object>> metadata = GetMetadata();
            foreach (KeyValuePair<string, object> metadataItem in metadata)
                arg.Set(metadataItem.Key, metadataItem.Value);
        }

        /// <summary>
        ///     Specifies the types of args that this attribute can apply to.
        /// </summary>
        /// <returns>The applicable arg types.</returns>
        protected virtual ApplicableArgTypesForMetadata GetApplicableArgTypes()
        {
            return ApplicableArgTypesForMetadata.All;
        }
    }
}
