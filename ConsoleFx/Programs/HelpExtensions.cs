using ConsoleFx.Parser;

namespace ConsoleFx.Programs
{
    public static class HelpExtensions
    {
        public static Command Description(this Command command, string description)
        {
            command["Description"] = description;
            return command;
        }

        public static Option Description(this Option option, string description)
        {
            option["Description"] = description;
            return option;
        }

        public static Argument Description(this Argument argument, string name, string description)
        {
            argument["Name"] = name;
            argument["Description"] = description;
            return argument;
        }
    }
}