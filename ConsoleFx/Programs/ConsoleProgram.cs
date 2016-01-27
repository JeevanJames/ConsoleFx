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
using System.Linq;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Utilities;

namespace ConsoleFx.Programs
{
    public sealed class ConsoleProgram<TStyle> : ProgramParser<TStyle>
        where TStyle : ParserStyle, new()
    {
        private readonly ExecuteHandler _handler;
        private readonly Dictionary<Type, Delegate> _errorHandlers = new Dictionary<Type, Delegate>();

        public ConsoleProgram(ExecuteHandler handler, object scope = null)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            _handler = handler;
            Behaviors = new Behaviors(InternalBehaviors) { Scope = scope };
        }

        public Behaviors Behaviors { get; }

        public int Run()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                Parse(args.Skip(1));
                return _handler();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        //This method is public because it should also be called from a try-catch block in the program.
        //The built-in error handling code is only available from within the Run() method, which does
        //the bulk of the work, but the programmer still has to handle exceptions from the remaining
        //code (i.e. the code that sets up the command-line parsing parameters).
        public int HandleError(Exception ex)
        {
            FireBeforeError(ex);
            try
            {
                //Find the corresponding error handler and execute it (or use the default error handler).
                Delegate handler;
                int exitCode;
                if (_errorHandlers.TryGetValue(ex.GetType(), out handler))
                    exitCode = (int)handler.DynamicInvoke(ex);
                else
                    exitCode = DefaultErrorHandler(ex);

                //Display usage, if needed
                //if (Behaviors.DisplayUsageOnError)
                //    DisplayUsage();

                return exitCode;
            }
            finally
            {
                FireAfterError(ex);
            }
        }

        /// <summary>
        ///     Assigns a new error handler for a specified exception types. The handler is called whenever
        ///     an exception of the specified type or it's derived types is thrown.
        /// </summary>
        /// <typeparam name="TException">The type of exception to handle</typeparam>
        /// <param name="handler">The method that handles the specified exception type</param>
        public void SetErrorHandler<TException>(ErrorHandler<TException> handler)
            where TException : Exception
        {
            _errorHandlers.Add(typeof(TException), handler);
        }

        /// <summary>
        ///     Occurs before an error is handled an error handler. Allows you to specify pre-error actions
        ///     that are common to all error handlers (like setting the console foreground color to red,
        ///     for example).
        /// </summary>
        public event EventHandler<ErrorEventArgs> BeforeError;

        /// <summary>
        ///     Occurs after an error has been handled by an error handler. Allows you to specify post-error
        ///     actions that are common to all error handlers (like resetting the console foreground to
        ///     default, for example).
        /// </summary>
        public event EventHandler<ErrorEventArgs> AfterError;

        /// <summary>
        ///     The default error handler, in case the framework cannot find a specific handler for an exception
        /// </summary>
        /// <param name="ex">Exception to handle.</param>
        private static int DefaultErrorHandler(Exception ex)
        {
            var cfxException = ex as ConsoleFxException;

            //If the exception derives from ArgumentException or it derives from ConsoleFxException
            //and has a negative error code, treat it as an internal exception.
            if (ex is ArgumentException || (cfxException != null && cfxException.ErrorCode < 0))
                Console.WriteLine(Messages.InternalError, ex.Message);
            else
                Console.WriteLine(ex.Message);
            ConsoleEx.WriteBlankLine();

            return cfxException?.ErrorCode ?? -1;
        }

        private void FireBeforeError(Exception exception)
        {
            EventHandler<ErrorEventArgs> beforeError = BeforeError;
            beforeError?.Invoke(this, new ErrorEventArgs(exception));
        }

        private void FireAfterError(Exception exception)
        {
            EventHandler<ErrorEventArgs> afterError = AfterError;
            afterError?.Invoke(this, new ErrorEventArgs(exception));
        }
    }

    /// <summary>
    ///     Provides exception details for the BeforeError and AfterError events of ConsoleProgram classes
    /// </summary>
    public sealed class ErrorEventArgs : EventArgs
    {
        internal ErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     The exception that is being handled
        /// </summary>
        public Exception Exception { get; }
    }

    public delegate int ExecuteHandler();

    public delegate int ErrorHandler<in TException>(TException exception) where TException : Exception;
}