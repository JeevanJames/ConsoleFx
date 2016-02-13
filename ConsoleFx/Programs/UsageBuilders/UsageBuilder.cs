using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleFx.Programs.UsageBuilders
{
    public abstract class UsageBuilder
    {
        private IReadOnlyList<Attribute> _assemblyAttributes;

        public abstract void Display(Parser.Parser parser);

        protected string AssemblyTitle => GetAssemblyAttribute<AssemblyTitleAttribute>(attr => attr.Title);

        protected string AssemblyDescription => GetAssemblyAttribute<AssemblyDescriptionAttribute>(attr => attr.Description);

        protected string AssemblyCompany => GetAssemblyAttribute<AssemblyCompanyAttribute>(attr => attr.Company);

        protected string AssemblyProduct => GetAssemblyAttribute<AssemblyProductAttribute>(attr => attr.Product);

        protected string AssemblyCopyright => GetAssemblyAttribute<AssemblyCopyrightAttribute>(attr => attr.Copyright);

        protected string GetAssemblyAttribute<TAttr>(Func<TAttr, string> valueExtractor)
            where TAttr : Attribute
        {
            Attribute attribute = AssemblyAttributes.OfType<TAttr>().FirstOrDefault();
            return attribute != null ? valueExtractor((TAttr)attribute) : null;
        }

        private IReadOnlyList<Attribute> AssemblyAttributes =>
            _assemblyAttributes ??
                (_assemblyAttributes = Assembly.GetEntryAssembly().GetCustomAttributes().ToList());
    }
}