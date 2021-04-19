using System;

namespace JakePerry.Unity.Examples.InterfaceProxy
{
    /// <summary>
    /// This class is not a Unity reference (eg. it's not a MonoBehaviour, ScriptableObject, etc).
    /// </summary>
    [Serializable]
    public sealed class ExampleNonUnityClass
    {
        public float field1;
        public bool field2;
    }
}
