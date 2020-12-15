﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NQueen.Shared.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NQueen.Shared.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All Solutions.
        /// </summary>
        public static string AllSolutions {
            get {
                return ResourceManager.GetString("AllSolutions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} for {1} must not exceed {2}..
        /// </summary>
        public static string BoardSizeTooLargeAllCaseError {
            get {
                return ResourceManager.GetString("BoardSizeTooLargeAllCaseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} for {1} must not exceed {2}..
        /// </summary>
        public static string BoardSizeTooLargeSingleCaseError {
            get {
                return ResourceManager.GetString("BoardSizeTooLargeSingleCaseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} for {1} must not exceed {2}..
        /// </summary>
        public static string BoardSizeTooLargeUniqueCaseError {
            get {
                return ResourceManager.GetString("BoardSizeTooLargeUniqueCaseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must be greater than or equal to {1}..
        /// </summary>
        public static string BoardSizeTooSmallError {
            get {
                return ResourceManager.GetString("BoardSizeTooSmallError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must be a valid integer in the interval [0, 127]..
        /// </summary>
        public static string InvalidIntError {
            get {
                return ResourceManager.GetString("InvalidIntError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Solutions Found for this {0}. Try a larger value!.
        /// </summary>
        public static string NoSolutionMessage {
            get {
                return ResourceManager.GetString("NoSolutionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Results are Saved into: {0}..
        /// </summary>
        public static string SaveResultsMessage {
            get {
                return ResourceManager.GetString("SaveResultsMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Single Solution.
        /// </summary>
        public static string SingleSolution {
            get {
                return ResourceManager.GetString("SingleSolution", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Empty Solution List.
        /// </summary>
        public static string TitleNoSolutionMessage {
            get {
                return ResourceManager.GetString("TitleNoSolutionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save Results.
        /// </summary>
        public static string TitleSaveResultsMessage {
            get {
                return ResourceManager.GetString("TitleSaveResultsMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unique.
        /// </summary>
        public static string UniqueSolutions {
            get {
                return ResourceManager.GetString("UniqueSolutions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} can not be null, empty or exclusively contain white space..
        /// </summary>
        public static string ValueNullOrWhiteSpaceError {
            get {
                return ResourceManager.GetString("ValueNullOrWhiteSpaceError", resourceCulture);
            }
        }
    }
}
