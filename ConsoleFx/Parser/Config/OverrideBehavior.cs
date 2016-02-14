namespace ConsoleFx.Parser.Config
{
    public enum OverrideBehavior
    {
        /// <summary>
        ///     Merge unique values from config and specified. In case of conflict, ignore the config values.
        /// </summary>
        MergePreferSpecified,

        /// <summary>
        ///     Merge unique values from config and specified. In case of conflict, ignore the specified values.
        /// </summary>
        MergePreferConfig,

        /// <summary>
        ///     If available, use only specified values and ignore config values.
        /// </summary>
        AlwaysSpecified,

        /// <summary>
        ///     If available, use only config values and ignore specified values.
        /// </summary>
        AlwaysConfig
    }
}