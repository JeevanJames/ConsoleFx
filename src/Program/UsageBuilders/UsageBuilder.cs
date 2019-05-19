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
using System.Linq;
using System.Reflection;

using ConsoleFx.CmdLine.Parser;

namespace ConsoleFx.CmdLine.Program.UsageBuilders
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
