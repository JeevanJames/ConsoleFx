using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.Parser.Styles;

namespace ConsoleFx.Programs
{
    public abstract class SingleCommandProgram : BaseCommand
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ConsoleFx.Programs.SingleCommandProgram" /> class.
        /// </summary>
        protected SingleCommandProgram(ParserStyle parserStyle) : base(parserStyle)
        {
        }

        public int Run()
        {
            IEnumerable<string> args = Environment.GetCommandLineArgs().Skip(1);
            return CoreRun(args);
        }
    }
}
