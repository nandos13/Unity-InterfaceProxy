using UnityEngine;

namespace JakePerry.Unity.Examples.InterfaceProxy
{
    /// <summary>
    /// This class implements the <see cref="ITargetInterface"/> interface and thus can be assigned
    /// to a field with an <see cref="InterfaceProxyAttribute"/> attribute that is restricted to the
    /// <see cref="ITargetInterface"/> type.
    /// </summary>
    [CreateAssetMenu(fileName = "TargetScriptableObject", menuName = "JakePerry/Unity/Examples/InterfaceProxy/TargetScriptableObject")]
    public class TargetScriptableObject : ScriptableObject, ITargetInterface
    {
        // Blank class.
    }
}
