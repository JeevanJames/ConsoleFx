﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleFx.CmdLine.Validators {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ConsoleFx.CmdLine.Validators.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expecting a true/false value.
        /// </summary>
        internal static string Boolean {
            get {
                return ResourceManager.GetString("Boolean", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Custom validation failed.
        /// </summary>
        internal static string Custom {
            get {
                return ResourceManager.GetString("Custom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory &apos;{0}&apos; does not exist or you do not have sufficient permissions to access it..
        /// </summary>
        internal static string Directory_Missing {
            get {
                return ResourceManager.GetString("Directory_Missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &apos;{0}&apos; is not a valid directory name..
        /// </summary>
        internal static string Directory_NameInvalid {
            get {
                return ResourceManager.GetString("Directory_NameInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory name you specified &apos;{0}&apos; exceeds the system-defined maximum length..
        /// </summary>
        internal static string Directory_PathTooLong {
            get {
                return ResourceManager.GetString("Directory_PathTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &quot;{0}&quot; does not match any of the allowed values.
        /// </summary>
        internal static string Enum {
            get {
                return ResourceManager.GetString("Enum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified file &apos;{0}&apos; has an invalid extension. Valid extensions are {1}..
        /// </summary>
        internal static string File_InvalidExtension {
            get {
                return ResourceManager.GetString("File_InvalidExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; does not exist or you do not have sufficient permissions to access it..
        /// </summary>
        internal static string File_Missing {
            get {
                return ResourceManager.GetString("File_Missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &apos;{0}&apos; is not a valid file name..
        /// </summary>
        internal static string File_NameInvalid {
            get {
                return ResourceManager.GetString("File_NameInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file name you specified &apos;{0}&apos; exceeds the system-defined maximum length..
        /// </summary>
        internal static string File_PathTooLong {
            get {
                return ResourceManager.GetString("File_PathTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &apos;{0}&apos; is not a valid GUID..
        /// </summary>
        internal static string Guid {
            get {
                return ResourceManager.GetString("Guid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is not a valid integer.
        /// </summary>
        internal static string Integer_NotAnInteger {
            get {
                return ResourceManager.GetString("Integer_NotAnInteger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} does not fall into the valid range of {1} to {2}.
        /// </summary>
        internal static string Integer_OutOfRange {
            get {
                return ResourceManager.GetString("Integer_OutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value you specified &apos;{0}&apos; should be a key value pair separated by an equal (=) symbol..
        /// </summary>
        internal static string KeyValue {
            get {
                return ResourceManager.GetString("KeyValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &quot;{0}&quot; does not match any of the allowed values: &quot;{1}&quot;.
        /// </summary>
        internal static string Lookup {
            get {
                return ResourceManager.GetString("Lookup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The path &apos;{0}&apos; does not exist.
        /// </summary>
        internal static string Path {
            get {
                return ResourceManager.GetString("Path", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameters you specified &quot;{0}&quot; does not match a valid value.
        /// </summary>
        internal static string Regex {
            get {
                return ResourceManager.GetString("Regex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &quot;{0}&quot; should not be more than {1} characters long.
        /// </summary>
        internal static string String_MaxLength {
            get {
                return ResourceManager.GetString("String_MaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &quot;{0}&quot; should be at least {1} characters long.
        /// </summary>
        internal static string String_MinLength {
            get {
                return ResourceManager.GetString("String_MinLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter you specified &apos;{0}&apos; is not a valid URI..
        /// </summary>
        internal static string Uri {
            get {
                return ResourceManager.GetString("Uri", resourceCulture);
            }
        }
    }
}