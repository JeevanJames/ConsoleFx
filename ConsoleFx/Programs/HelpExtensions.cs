using ConsoleFx.Parser;

namespace ConsoleFx.Programs
{
    public static class HelpExtensions
    {
        public static Command Description(this Command command, string description)
        {
            command.Metadata.Set("Description", description);
            return command;
        }

        public static Option Description(this Option option, string description)
        {
            option.Metadata.Set("Description", description);
            return option;
        }

        public static Argument Description(this Argument argument, string name, string description)
        {
            argument.Metadata.Set("Name", name);
            argument.Metadata.Set("Description", description);
            return argument;
        }
    }
}