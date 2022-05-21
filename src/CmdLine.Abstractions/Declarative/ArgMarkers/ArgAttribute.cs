using System;
using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Common base class for all arg attributes, which has common properties for help.
    /// </summary>
    public abstract class ArgAttribute : MetadataAttribute
    {
        public string HelpText { get; set; }

        public Type HelpTextResourceType { get; set; }

        public string HelpTextResourceName { get; set; }

        public int HelpOrder { get; set; }

        public bool HideHelp { get; set; }

        public override IEnumerable<ArgMetadata> GetMetadata()
        {
            yield return new ArgMetadata(HelpMetadataKey.Description,
                ResolveResourceString(HelpText, HelpTextResourceType, HelpTextResourceName, required: false));
            yield return new ArgMetadata(HelpMetadataKey.Order, HelpOrder);
            yield return new ArgMetadata(HelpMetadataKey.Hide, HideHelp);
        }

        protected static IReadOnlyList<string> ConstructNames(string name, string[] additionalNames)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException("The name cannot be empty or whitespaced.", nameof(name));

            if (additionalNames is null)
                throw new ArgumentNullException(nameof(additionalNames));

            for (int i = 0; i < additionalNames.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(additionalNames[i]))
                {
                    throw new ArgumentException(
                        $"Additional names for arg '{name}' has an null, empty or whitespaced value at index {i}.",
                        nameof(additionalNames));
                }
            }

            string[] names = new string[additionalNames.Length + 1];
            names[0] = name;
            if (additionalNames.Length > 0)
                additionalNames.CopyTo(names, index: 1);

            return names;
        }
    }
}
