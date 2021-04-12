using System;
using UnityEngine;

namespace JPAssets.Unity.Examples.InterfaceProxy
{
    /// <summary>
    /// This class implements the <see cref="ITargetInterface"/> interface and thus can be assigned
    /// to a field with an <see cref="InterfaceProxyAttribute"/> attribute that is restricted to the
    /// <see cref="ITargetInterface"/> type.
    /// </summary>
    [CreateAssetMenu(fileName = "TestTargetScriptableObject", menuName = "JPAssets/Unity/Examples/InterfaceProxy/TestTargetScriptableObject")]
    public class TestTargetScriptableObject : ScriptableObject, ITargetInterface
    {
        // Blank class.
    }
}
