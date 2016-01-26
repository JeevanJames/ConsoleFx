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

namespace ConsoleFx.Parser
{
    //Class is not sealed so that derived classes can define their own behavior properties.
    public class Behaviors
    {
        protected Behaviors SyncBehaviors { get; }

        private CommandGrouping _grouping;
        private object _scope;

        public Behaviors()
        {
        }

        public Behaviors(Behaviors syncBehaviors)
        {
            SyncBehaviors = syncBehaviors;
        }

        //TODO: Remove DisplayUsageOnError property from here as it is a UI-specific behavior and does not belong in the parser.

        public CommandGrouping Grouping
        {
            get { return SyncBehaviors?.Grouping ?? _grouping; }
            set
            {
                if (SyncBehaviors != null)
                    SyncBehaviors.Grouping = value;
                else
                    _grouping = value;
            }
        }

        /// <summary>
        ///     The object instance to write argument and option values when parsing the command-line args.
        ///     If null, then these values are written to static properties.
        /// </summary>
        public object Scope
        {
            get { return SyncBehaviors != null ? SyncBehaviors.Scope : _scope; }
            set
            {
                if (SyncBehaviors != null)
                    SyncBehaviors.Scope = value;
                else
                    _scope = value;
            }
        }
    }
}