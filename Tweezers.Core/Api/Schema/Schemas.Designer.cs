﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tweezers.Api.Schema {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Schemas {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Schemas() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Tweezers.Api.Schema.Schemas", typeof(Schemas).Assembly);
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
        ///   Looks up a localized string similar to {
        ///  &quot;collectionName&quot;: &quot;tweezers-schema&quot;,
        ///  &quot;displayNames&quot;: {
        ///    &quot;singularName&quot;: &quot;Tweezers Object&quot;,
        ///    &quot;pluralName&quot;: &quot;Tweezers Objects&quot;
        ///  },
        ///  &quot;internal&quot;: true,
        ///  &quot;icon&quot;: &quot;grid_on&quot;,
        ///  &quot;fields&quot;: {
        ///    &quot;collectionName&quot;: {
        ///      &quot;fieldProperties&quot;: {
        ///        &quot;name&quot;: &quot;collectionName&quot;,
        ///        &quot;displayName&quot;: &quot;Collection&quot;,
        ///        &quot;fieldType&quot;: &quot;String&quot;,
        ///        &quot;min&quot;: 1,
        ///        &quot;max&quot;: 40,
        ///        &quot;required&quot;: true,
        ///        &quot;idField&quot;: true
        ///      }
        ///    },
        ///    &quot;singularName&quot;: {
        ///      &quot;fieldProperties&quot;: {
        ///        &quot;na [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SchemaMetaJson {
            get {
                return ResourceManager.GetString("SchemaMetaJson", resourceCulture);
            }
        }
    }
}
