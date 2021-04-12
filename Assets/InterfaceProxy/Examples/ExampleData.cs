using System;
using UnityEngine;

namespace JPAssets.Unity.Examples.InterfaceProxy
{
    public class ExampleData : MonoBehaviour
    {
        /// <summary>
        /// This field is decorated with an attribute which restricts it to only accept objects
        /// which implement the <see cref="ITargetInterface"/> interface.
        /// Attempting to assign an object in the inspector which does not implement the interface
        /// will log an error and fail.
        /// </summary>
        [InterfaceProxy(typeof(ITargetInterface))]
        public UnityEngine.Object m_targetObjectField;

        /// <summary>
        /// This attribute uses a non-interface type. As such, it is displayed as an error in the inspector.
        /// </summary>
        [InterfaceProxy(typeof(GameObject))]
        public UnityEngine.Object m_badAttributeType;

        /// <summary>
        /// This field is of type GameObject, which is sealed. Only one type can be assigned to the field,
        /// thus there is no benefit to declaring the InterfaceProxyAttribute here. A warning is displayed
        /// in the inspector explaining this.
        /// </summary>
        [InterfaceProxy(typeof(ITargetInterface))]
        public GameObject m_badFieldType;

        public void Start()
        {
            if (m_targetObjectField != null)
            {
                Debug.Assert(InterfaceProxyUtilities.TryCast<ITargetInterface>(m_targetObjectField, out ITargetInterface result));
                Debug.Log($"Successfully cast to ITargetInterface object.");
            }
        }
    }
}
