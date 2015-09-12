using ConsoleFx.Parsers;
using ConsoleFx.Parsers.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleFx.Parser
{
    /// <summary>
    /// Represents a commandline switch parameter.
    /// </summary>
    [DebuggerDisplay("Option: {Name}")]
    public sealed partial class Option
    {
        public string Name { get; }
        public string ShortName { get; set; }
        public bool CaseSensitive { get; set; }
        public int Order { get; set; }
        public OptionUsage Usage { get; } = new OptionUsage();
        public OptionParameterValidatorsCollection Validators { get; } = new OptionParameterValidatorsCollection();
        public OptionHandler Handler { get; set; }

        public Option(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public void AddToList<T>(Expression<Func<IList<T>>> expression, Converter<string, T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Handler = prms => {
                converter = GetConverterFor(converter);

                MemberInfo dataMember = GetMemberInfoFromExpression(expression, typeof(IList<T>));
                var list = GetDataMemberValue<IList<T>>(dataMember);
                foreach (string prm in prms)
                {
                    object value = converter != null ? (object)converter(prm) : prm;
                    list.Add((T)value);
                }
            };
        }

        public void AssignTo<T>(Expression<Func<T>> expression, Converter<string, T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            Handler = prms => {
                converter = GetConverterFor(converter);
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                object value = converter != null ? (object)converter(prms[0]) : prms[0];
                SetDataMemberValue(dataMember, value);
            };
        }


        public void Flag(Expression<Func<bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            Handler = prms => {
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                SetDataMemberValue(dataMember, true);
            };
        }

        public void HandledBy(OptionHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            Handler = handler;
        }

        public Option ValidateWith(params BaseValidator[] validators)
        {
            if (Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
                throw new ParserException(1000, $"Cannot add validators to option {Name} because it does not accept parameters.");
            if (Usage.ParameterType == OptionParameterType.Individual)
                throw new ParserException(1000, $"Cannot add an all-parameter validator for option {Name} because it's parameter type is individual. Use the ValidateWith overload that acceprs a parameter index.");
            foreach (BaseValidator validator in validators)
                Validators.Add(validator);
            return this;
        }

        public Option ValidateWith(int parameterIndex, params BaseValidator[] validators)
        {
            if (Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
                throw new ParserException(1000, $"Cannot add validators to option {Name} because it does not accept parameters.");
            if (Usage.ParameterType == OptionParameterType.Repeating)
                throw new ParserException(1000, $"Cannot add a specific parameter validator for option {Name} because it's parameter type is repeating. Use the ValidateWith overload that does not accept a parameter index.");
            if (parameterIndex >= Usage.MaxParameters)
                throw new ArgumentException($"Parameter index specified {parameterIndex} is greater than the number of parameters allowed for option {Name}.", nameof(parameterIndex));
            foreach (BaseValidator validator in validators)
                Validators.Add(parameterIndex, validator);
            return this;
        }

        /// <summary>
        /// Extracts the MemberInfo data from a member expression.
        /// If baseType is specified, then the type specified in the expression must be assignable
        /// to the baseType. Otherwise, it must be of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">Member expression to e</param>
        /// <param name="baseType">Optional base type that the type T must be assignable to.</param>
        /// <returns></returns>
        private static MemberInfo GetMemberInfoFromExpression<T>(Expression<Func<T>> expression, Type baseType = null)
        {
            var lambda = (LambdaExpression)expression;
            var memberExpression = lambda.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException($"The expression should return a data member (field or property) from the '{typeof(T).FullName}' type.", nameof(expression));
            if (baseType != null)
            {
                if (!baseType.IsAssignableFrom(memberExpression.Type))
                    throw new ArgumentException($"Expression data member '{memberExpression.Member.Name}' should be assignable to type '{baseType.FullName}'", nameof(expression));
            }
            else if (memberExpression.Type != typeof(T))
                throw new ArgumentException($"Expression data member '{memberExpression.Member.Name}' should be of type '{typeof(T).FullName}'", nameof(expression));
            return memberExpression.Member;
        }

        /// <summary>
        /// Given a data member, return its value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        private static T GetDataMemberValue<T>(MemberInfo member)
        {
            var property = member as PropertyInfo;
            if (property != null)
                return (T)property.GetValue(null, null);

            var field = member as FieldInfo;
            if (field != null)
                return (T)field.GetValue(null);

            string declaringTypeName = member.DeclaringType != null ? member.DeclaringType.FullName : "Unknown";
            throw new ParserException(1000, $"The member '{member.Name}' in type '{declaringTypeName}' should be either a property or a field to be specified in an argument/option handler");
        }

        /// <summary>
        /// Sets the value of the given data member.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        private static void SetDataMemberValue(MemberInfo member, object value)
        {
            var property = member as PropertyInfo;
            if (property != null)
            {
                property.SetValue(null, value, null);
                return;
            }

            var field = member as FieldInfo;
            if (field != null)
            {
                field.SetValue(null, value);
                return;
            }

            string declaringTypeName = member.DeclaringType != null ? member.DeclaringType.FullName : "Unknown";
            throw new ParserException(1000, $"The member '{member.Name}' in type '{declaringTypeName}' should be either a property or a field to be specified in an argument/option handler");
        }

        private static Converter<string, T> GetConverterFor<T>(Converter<string, T> converter)
        {
            Type type = typeof(T);

            //If the type is string, return whatever converter was passed, even if it is null (which
            //would mean that the converter just uses the parameter value as-is
            if (type == typeof(string) || converter != null)
                return converter;

            //Special handling for enums
            if (type.IsEnum)
                return str => (T)Enum.Parse(type, str, true);

            //Special handling for booleans
            if (type == typeof(bool))
                return str => {
                    bool boolValue;
                    if (bool.TryParse(str, out boolValue))
                        return (T)(object)boolValue;
                    if (str.Equals("yes", StringComparison.OrdinalIgnoreCase) || str.Equals("1", StringComparison.OrdinalIgnoreCase))
                        return (T)(object)true;
                    if (str.Equals("no", StringComparison.OrdinalIgnoreCase) || str.Equals("0", StringComparison.OrdinalIgnoreCase))
                        return (T)(object)false;

                    throw new FormatException($"Invalid boolean value specified - {str}.");
                };

            //For any basic type, return a pre-defined converter from the lookup dictionary
            Delegate @delegate;
            if (_converterLookup.TryGetValue(typeof(T), out @delegate))
                return (Converter<string, T>)@delegate;

            //If we cannot figure it out for the caller, then throw an exception saying that the caller
            //must explicitly specify the converter.
            throw new ParserException(1000, $"Cannot find default converter for type '{type.FullName}'. Please specify one manually.");
        }

        //Lookup of converters for all basic types
        private static readonly Dictionary<Type, Delegate> _converterLookup = new Dictionary<Type, Delegate> {
            { typeof(int), (Converter<string, int>)int.Parse },
            { typeof(uint), (Converter<string, uint>)uint.Parse },
            { typeof(sbyte), (Converter<string, sbyte>)sbyte.Parse },
            { typeof(byte), (Converter<string, byte>)byte.Parse },
            { typeof(char), (Converter<string, char>)char.Parse },
            { typeof(short), (Converter<string, short>)short.Parse },
            { typeof(ushort), (Converter<string, ushort>)ushort.Parse },
            { typeof(long), (Converter<string, long>)long.Parse },
            { typeof(ulong), (Converter<string, ulong>)ulong.Parse },
            { typeof(float), (Converter<string, float>)float.Parse },
            { typeof(double), (Converter<string, double>)double.Parse },
            { typeof(decimal), (Converter<string, decimal>)decimal.Parse },
        };
    }

    public sealed partial class Option
    {
        internal OptionRun Run { get; } = new OptionRun();

        internal void ClearRun()
        {
            Run.Occurences = 0;
            Run.Parameters.Clear();
        }

        internal sealed class OptionRun
        {
            internal int Occurences { get; set; }
            internal List<string> Parameters { get; } = new List<string>();
        }
    }

    public delegate void OptionHandler(string[] parameters);

    /// <summary>
    /// Represents a collection of options. Note: This is not a keyed collection because the key
    /// can be either the name or short name.
    /// </summary>
    public sealed class Options : Collection<Option>
    {
        public Option this[string name]
        {
            get
            {
                return this.FirstOrDefault(option => {
                    StringComparison comparison = option.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                    if (name.Equals(option.Name, comparison))
                        return true;
                    if (!string.IsNullOrEmpty(option.ShortName) && name.Equals(option.ShortName, comparison))
                        return true;
                    return false;
                });
            }
        }

        /// <summary>
        /// Prevents duplicate options from being inserted.
        /// </summary>
        /// <param name="index">Location to insert the new option.</param>
        /// <param name="item">Option to insert.</param>
        protected override void InsertItem(int index, Option item)
        {
            if (this.Any(DuplicateCheck(item)))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, item.Name), nameof(item));
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Prevents duplicate options from being set.
        /// </summary>
        /// <param name="index">Location to set the new option.</param>
        /// <param name="item">Option to set.</param>
        protected override void SetItem(int index, Option item)
        {
            if (this.Any(DuplicateCheck(item)))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, item.Name), nameof(item));
            base.SetItem(index, item);
        }
         
        /// <summary>
        /// Returns a delegate that can check whether the passed option is already available in the
        /// collection. Used whenever options are added or set in the collection.
        /// </summary>
        /// <param name="option">Option that is being set.</param>
        /// <returns>True if the option already exists in the collection. Otherwise false.</returns>
        private static Func<Option, bool> DuplicateCheck(Option option)
        {
            return opt => {
                StringComparison comparison = opt.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                if (option.Name.Equals(opt.Name, comparison))
                    return true;
                if (!string.IsNullOrEmpty(opt.ShortName) && option.ShortName.Equals(opt.ShortName, comparison))
                    return true;
                return false;
            };
        }
    }
}
