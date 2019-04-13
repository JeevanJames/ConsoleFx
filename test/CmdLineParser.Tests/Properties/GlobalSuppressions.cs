using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules",
    "SA1005:Single line comments must begin with single space",
    Justification = "This would be nice but it raises errors with commented out code.")]

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules",
    "SA1101:Prefix local calls with this",
    Justification = "Not needed as there can be no naming clashes between various types of members.")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules",
    "SA1116:Split parameters must start on line after declaration",
    Justification = "TBD")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules",
    "SA1117:Parameters must be on same line or separate lines",
    Justification = "TBD")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules",
    "SA1118:Parameter must not span multiple lines",
    Justification = "TBD")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules",
    "SA1124:Do not use regions",
    Justification = "Nice to have, but preventing the region around the file header.")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules",
    "SA1133:Do not combine attributes",
    Justification = "TBD")]

[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules",
    "SA1201:Elements must appear in the correct order",
    Justification = "TBD")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules",
    "SA1202:Elements must be ordered by access",
    Justification = "Allow members to be mixed so that related members can be together.")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules",
    "SA1204:Static elements must appear before instance elements",
    Justification = "Prefer to keep static and instance in separate partial classes, even files.")]

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules",
    "SA1309:Field names must not begin with underscore",
    Justification = "Require underscore prefix for all private fields.")]

[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single class",
    Justification = "Primary classes should be in separate files, but supporting classes should be allowed to be next to their primary classes.")]

[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules",
    "SA1503:Braces must not be omitted",
    Justification = "No braces needed for single line blocks.")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules",
    "SA1520:Use braces consistently",
    Justification = "Brace usage justified according to block size.")]

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules",
    "SA1633:File must have header",
    Justification = "Nice to have, but this rule does not allow file headers in a region.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules",
    "SA1652:Enable XML documentation output",
    Justification = "Temporary exception. Will be removed once documentation is completed.")]

[assembly: SuppressMessage("Major Code Smell",
    "S125:Sections of code should not be commented out",
    Justification = "Temporary exception to allow quick pace of development. Should be removed later.")]
[assembly: SuppressMessage("Major Code Smell",
    "S1135:Track uses of TODO tags",
    Justification = "TODO tags should be allowed to be committed to version control, as they indicate pending work.")]
