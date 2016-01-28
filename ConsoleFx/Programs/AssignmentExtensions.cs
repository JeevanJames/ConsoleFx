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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using ConsoleFx.Parser;

namespace ConsoleFx.Programs
{
    public static class AssignmentExtensions
    {
        public static void HandleWith(this Argument argument, ArgumentHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            argument.Handler = handler;
        }

        public static void HandleWith(this Option option, OptionHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            option.Handler = handler;
        }

        public static void AssignTo<T>(this Argument argument, Expression<Func<T>> expression,
            Converter<T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            argument.Handler = (arg, scope) => {
                converter = GetConverterFor(converter);
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                object value = converter != null ? (object)converter(arg) : arg;
                SetDataMemberValue(dataMember, value, scope);
            };
        }

        public static void AssignTo<T>(this Option option, Expression<Func<T>> expression, Converter<T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            option.Handler = (prms, scope) => {
                converter = GetConverterFor(converter);
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                object value = converter != null ? (object)converter(prms[0]) : prms[0];
                SetDataMemberValue(dataMember, value, scope);
            };
        }

        public static void AssignTo<TScope, T>(this Option option, Expression<Func<TScope, T>> expression,
            Converter<T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            option.Handler = (prms, scope) => {
                converter = GetConverterFor(converter);
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                object value = converter != null ? (object)converter(prms[0]) : prms[0];
                SetDataMemberValue(dataMember, value, scope);
            };
        }

        public static void AddToList<T>(this Argument argument, Expression<Func<IList<T>>> expression,
            Converter<T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            argument.Handler = (arg, scope) => {
                converter = GetConverterFor(converter);
                MemberInfo dataMember = GetMemberInfoFromExpression(expression, typeof(IList<T>));
                IList<T> list = GetDataMemberValue<IList<T>>(dataMember, scope);
                object value = converter != null ? (object)converter(arg) : arg;
                list.Add((T)value);
            };
        }

        public static void AddToList<T>(this Option option, Expression<Func<IList<T>>> expression,
            Converter<T> converter = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            option.Handler = (prms, scope) => {
                converter = GetConverterFor(converter);

                MemberInfo dataMember = GetMemberInfoFromExpression(expression, typeof(IList<T>));
                IList<T> list = GetDataMemberValue<IList<T>>(dataMember, scope);
                foreach (string prm in prms)
                {
                    object value = converter != null ? (object)converter(prm) : prm;
                    list.Add((T)value);
                }
            };
        }

        /// <summary>
        ///     Shortcut to a option handler that sets a boolean value to true if the option is specified
        ///     on the command line.
        ///     The prerequisite to use this method is that the option should not occur more than once
        ///     and should not accept any parameters.
        /// </summary>
        /// <param name="option">The option to handle.</param>
        /// <param name="expression">The expression pointing to the boolean field to set.</param>
        public static void Flag(this Option option, Expression<Func<bool>> expression)
        {
            if (option.Usage.MaxOccurences > 1)
            {
                throw new Exception(
                    $"Cannot use the Flag method to handle the {option.Name} option because it can be specified more than one time.");
            }
            if (option.Usage.ParameterRequirement != OptionParameterRequirement.NotAllowed)
            {
                throw new Exception(
                    $"Cannot use the Flag method to handle the {option.Name} option because it requires parameters.");
            }
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            option.Handler = (prms, scope) => {
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                SetDataMemberValue(dataMember, true, scope);
            };
        }

        //TODO: Extract common code from these 2 overloads
        public static void Flag<TScope>(this Option option, Expression<Func<TScope, bool>> expression)
        {
            if (option.Usage.MaxOccurences > 1)
            {
                throw new Exception(
                    $"Cannot use the Flag method to handle the {option.Name} option because it can be specified more than one time.");
            }
            if (option.Usage.ParameterRequirement != OptionParameterRequirement.NotAllowed)
            {
                throw new Exception(
                    $"Cannot use the Flag method to handle the {option.Name} option because it requires parameters.");
            }
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            option.Handler = (prms, scope) => {
                MemberInfo dataMember = GetMemberInfoFromExpression(expression);
                SetDataMemberValue(dataMember, true, scope);
            };
        }

        /// <summary>
        ///     Extracts the MemberInfo data from a member expression.
        ///     If baseType is specified, then the type specified in the expression must be assignable
        ///     to the baseType. Otherwise, it must be of type T.
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
            {
                throw new ArgumentException(
                    $"The expression should return a data member (field or property) from the '{typeof(T).FullName}' type.",
                    nameof(expression));
            }
            if (baseType != null)
            {
                if (!baseType.IsAssignableFrom(memberExpression.Type))
                {
                    throw new ArgumentException(
                        $"Expression data member '{memberExpression.Member.Name}' should be assignable to type '{baseType.FullName}'",
                        nameof(expression));
                }
            } else if (memberExpression.Type != typeof(T))
            {
                throw new ArgumentException(
                    $"Expression data member '{memberExpression.Member.Name}' should be of type '{typeof(T).FullName}'",
                    nameof(expression));
            }
            return memberExpression.Member;
        }

        //TODO: Extract common code from these two overloads
        private static MemberInfo GetMemberInfoFromExpression<T, TScope>(Expression<Func<TScope, T>> expression,
            Type baseType = null)
        {
            var lambda = (LambdaExpression)expression;
            var memberExpression = lambda.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException(
                    $"The expression should return a data member (field or property) from the '{typeof(T).FullName}' type.",
                    nameof(expression));
            }
            if (baseType != null)
            {
                if (!baseType.IsAssignableFrom(memberExpression.Type))
                {
                    throw new ArgumentException(
                        $"Expression data member '{memberExpression.Member.Name}' should be assignable to type '{baseType.FullName}'",
                        nameof(expression));
                }
            }
            else if (memberExpression.Type != typeof(T))
            {
                throw new ArgumentException(
                    $"Expression data member '{memberExpression.Member.Name}' should be of type '{typeof(T).FullName}'",
                    nameof(expression));
            }
            return memberExpression.Member;
        }

        /// <summary>
        ///     Given a data member, return its value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <param name="scope">Instance on which to set data member value. Use null for static member.</param>
        /// <returns></returns>
        private static T GetDataMemberValue<T>(MemberInfo member, object scope)
        {
            var property = member as PropertyInfo;
            if (property != null)
                return (T)property.GetValue(scope, null);

            var field = member as FieldInfo;
            if (field != null)
                return (T)field.GetValue(scope);

            string declaringTypeName = member.DeclaringType != null ? member.DeclaringType.FullName : "Unknown";
            throw new ParserException(1000,
                $"The member '{member.Name}' in type '{declaringTypeName}' should be either a property or a field to be specified in an argument/option handler");
        }

        /// <summary>
        ///     Sets the value of the given data member.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <param name="scope">Instance on which to set data member value. Use null for static member.</param>
        private static void SetDataMemberValue(MemberInfo member, object value, object scope)
        {
            var property = member as PropertyInfo;
            if (property != null)
            {
                property.SetValue(scope, value, null);
                return;
            }

            var field = member as FieldInfo;
            if (field != null)
            {
                field.SetValue(scope, value);
                return;
            }

            string declaringTypeName = member.DeclaringType != null ? member.DeclaringType.FullName : "Unknown";
            throw new ParserException(1000,
                $"The member '{member.Name}' in type '{declaringTypeName}' should be either a property or a field to be specified in an argument/option handler");
        }

        private static Converter<T> GetConverterFor<T>(Converter<T> converter)
        {
            Type type = typeof(T);

            //If the type is string, return whatever converter was passed, even if it is null (which
            //would mean that the converter just uses the parameter value as-is
            if (type == typeof(string) || converter != null)
                return converter;

            //Special handling for enums
            if (type.GetTypeInfo().IsEnum)
                return str => (T)Enum.Parse(type, str, true);

            //Special handling for booleans
            if (type == typeof(bool))
            {
                return str => {
                    bool boolValue;
                    if (bool.TryParse(str, out boolValue))
                        return (T)(object)boolValue;
                    if (str.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                        str.Equals("1", StringComparison.OrdinalIgnoreCase))
                        return (T)(object)true;
                    if (str.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                        str.Equals("0", StringComparison.OrdinalIgnoreCase))
                        return (T)(object)false;

                    throw new FormatException($"Invalid boolean value specified - {str}.");
                };
            }

            //For any basic type, return a pre-defined converter from the lookup dictionary
            Delegate @delegate;
            if (_converterLookup.TryGetValue(typeof(T), out @delegate))
                return (Converter<T>)@delegate;

            //If we cannot figure it out for the caller, then throw an exception saying that the caller
            //must explicitly specify the converter.
            throw new ParserException(1000,
                $"Cannot find default converter for type '{type.FullName}'. Please specify one manually.");
        }

        //Lookup of converters for all basic types
        private static readonly Dictionary<Type, Delegate> _converterLookup = new Dictionary<Type, Delegate> {
            { typeof(int), (Converter<int>)int.Parse },
            { typeof(uint), (Converter<uint>)uint.Parse },
            { typeof(sbyte), (Converter<sbyte>)sbyte.Parse },
            { typeof(byte), (Converter<byte>)byte.Parse }, {
                typeof(char), (Converter<char>)(str => {
                    char ch;
                    return char.TryParse(str, out ch) ? ch : '\0';
                })
            },
            { typeof(short), (Converter<short>)short.Parse },
            { typeof(ushort), (Converter<ushort>)ushort.Parse },
            { typeof(long), (Converter<long>)long.Parse },
            { typeof(ulong), (Converter<ulong>)ulong.Parse },
            { typeof(float), (Converter<float>)float.Parse },
            { typeof(double), (Converter<double>)double.Parse },
            { typeof(decimal), (Converter<decimal>)decimal.Parse }
        };
    }

    public delegate T Converter<out T>(string input);
}
