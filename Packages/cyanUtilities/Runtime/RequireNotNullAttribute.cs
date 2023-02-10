/*
Copyright (c) 2004 cyanseraph - This code is part of the CyanUtilities private Unity package
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyan
{
    /// <summary>
    /// Disallows play mode if this reference is not set, also highlights it in red in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequireNotNullAttribute : PropertyAttribute {

        /// <summary>
        /// Add some prefix text to the serialized property
        /// </summary>
        public readonly string message;

        public RequireNotNullAttribute()
        {
            message = "Required";
        }

        /// <summary>
        /// Add some prefix text to the serialized property
        /// </summary>
        public RequireNotNullAttribute(string message)
        {
            this.message = message;
        }
    }
}