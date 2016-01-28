using System;
using System.Collections.Generic;

using ConsoleFx.Utilities;

namespace ConsoleFx.Programs
{
    public abstract class ConsoleProgram
    {
        //TODO: Prevent duplicates
        //TODO: Insert in correct order of exception dependence.
        private readonly Dictionary<Type, Delegate> _errorHandlers = new Dictionary<Type, Delegate>();

        /// <summary>
        ///     Executes the console program and returns the resultant error code.
        /// </summary>
        /// <returns>The error code to return.</returns>
        public int Run()
        {
            try
            {
                return InternalRun();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        ///     Override this method to provide the actual execution logic for the console program.
        /// </summary>
        /// <returns>The error code to return from the console program.</returns>
        protected abstract int InternalRun();

        /// <summary>
        ///     Generic error handler for the entire console program.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        /// <returns>The error code to return from the console program.</returns>
        public int HandleError(Exception ex)
        {
            BeforeError?.Invoke(this, new ErrorEventArgs(ex));
            try
            {
                //Find the corresponding error handler and execute it (or use the default error handler).
                Delegate handler;
                int exitCode;
                if (_errorHandlers.TryGetValue(ex.GetType(), out handler))
                    exitCode = (int)handler.DynamicInvoke(ex);
                else
                    exitCode = DefaultErrorHandler(ex);

                //TODO: Handle displaying of usage on exception
                //Display usage, if needed
                //if (Behaviors.DisplayUsageOnError)
                //    DisplayUsage();

                return exitCode;
            }
            finally
            {
                AfterError?.Invoke(this, new ErrorEventArgs(ex));
            }
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
        ///     The default error handler, in case the framework cannot find a specific handler for an exception
        /// </summary>
        /// <param name="ex">Exception to handle.</param>
        private static int DefaultErrorHandler(Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex);
#endif

            var cfxException = ex as ConsoleFxException;

#if !DEBUG
    //If the exception derives from ArgumentException or it derives from ConsoleFxException
    //and has a negative error code, treat it as an internal exception.
            if (ex is ArgumentException || (cfxException != null && cfxException.ErrorCode < 0))
                Console.WriteLine(Messages.InternalError, ex.Message);
            else
                Console.WriteLine(ex.Message);
#endif

            ConsoleEx.WriteBlankLine();

            return cfxException?.ErrorCode ?? -1;
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

    public delegate int ErrorHandler<in TException>(TException exception) where TException : Exception;
}