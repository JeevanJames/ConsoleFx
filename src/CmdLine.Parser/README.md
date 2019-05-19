## Enhancements

[ ] Create a group concept for args, to group mutually-exclusive args
    Args under a command can specify a group name. The default group name is null.
	Only args under a specific group can be mentioned together.
	Args with the same name can be declared multiple times under a command, but with different group names.
	Arg identity will now include group names in addition to arg name.
[X] Support case sensitive short names, independent of the case sensitivity of the name
[ ] Consider arguments having names so they can be referenced by the name and not just position.
[X]		Update `Argument` objects to derive from `MetadataObject`
[ ]		Update corresponding members in `ParseResult`
[ ] Replace all hardcoded strings with resources
[ ] Remove any unused resources
[ ] Add developer notes
[ ]		Validations
[X] Add support for alternative names for options and commands. Generalize the concept, instead of having a ShortName concept.
[ ] Implement default and order for options

[ ] Allow a configuration file which can specify some pre-defined args as custom commands.
	Roughly, lets take a git command like git status -s -b.
	The config file can specify this as a custom usage under an alias, say "my-status"
	Now, running the command can be done with "git my-status"
	Additional command-line args can also be specified with the aliases
	For example: git my-status -x -y -z