using System;
using UnityEngine;

namespace JPAssets.Unity.Examples.InterfaceProxy
{
    /// <summary>
    /// This class implements the <see cref="ICloneable"/> interface and thus can be assigned
    /// to a field with an <see cref="InterfaceProxyAttribute"/> attribute that is restricted to the
    /// <see cref="ICloneable"/> type.
    /// </summary>
    [CreateAssetMenu(fileName = "TestCloneableScriptableObject", menuName = "JPAssets/Unity/Examples/InterfaceProxy/TestCloneableScriptableObject")]
    public class TestCloneableScriptableObject : ScriptableObject, ICloneable
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
