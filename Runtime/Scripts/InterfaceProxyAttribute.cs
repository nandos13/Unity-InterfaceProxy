using System;
using UnityEngine;

namespace JPAssets.Unity
{
    /// <summary>
    /// Use this attribute on an object-reference field (eg. a serialized <see cref="MonoBehaviour"/>
    /// or <see cref="ScriptableObject"/> field) to restrict the type of object which can be assigned
    /// to the field via the inspector.
    /// 
    /// </summary>
    public sealed class InterfaceProxyAttribute : PropertyAttribute
    {
        private readonly Type m_interfaceType;

        public Type InterfaceType => m_interfaceType;

        /// <summary>
        /// Restricts the type of object that can be assigned to the decorated field.
        /// </summary>
        /// <param name="interfaceType">
        /// The interface type that may be assigned to the decorated field.
        /// </param>
        public InterfaceProxyAttribute(Type interfaceType)
        {
            m_interfaceType = interfaceType;
        }
    }
}
