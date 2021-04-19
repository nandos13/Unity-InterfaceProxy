using UnityEngine;

namespace JakePerry.Unity.Examples.InterfaceProxy
{
    public class ExampleData : MonoBehaviour
    {
        [Header("Correct usage")]

        /// <summary>
        /// This field is decorated with an attribute which restricts it to only accept objects
        /// which implement the <see cref="ITargetInterface"/> interface.
        /// Attempting to assign an object in the inspector which does not implement the interface
        /// will log an error and fail.
        /// 
        /// Try dragging a <see cref="TargetScriptableObject"/> or <see cref="TargetMonobehaviour"/> instance
        /// into this field in the inspector.
        /// Dragging any other objects in will not work. For example, try the <see cref="EmptyScriptableObject"/>
        /// instance in the Examples folder.
        /// </summary>
        [SerializeField]
        [InterfaceProxy(typeof(ITargetInterface))]
        private UnityEngine.Object m_targetObjectField;

        [Header("Error cases")]

        #region Incorrect usage examples

        /// <summary>
        /// This attribute uses a non-interface type. As such, it is displayed as an error in the inspector.
        /// </summary>
        [SerializeField]
        [InterfaceProxy(typeof(GameObject))]
        private UnityEngine.Object m_badAttributeType;

        /// <summary>
        /// This field is of type <see cref="GameObject"/>, which is sealed. Only one type can be assigned to the field,
        /// thus there is no benefit to declaring the <see cref="InterfaceProxyAttribute"/> here. A warning is displayed
        /// in the inspector explaining this.
        /// </summary>
        [SerializeField]
        [InterfaceProxy(typeof(ITargetInterface))]
        private GameObject m_badFieldTypeSealed;

        /// <summary>
        /// This field is not a serializable reference type and cannot be used with <see cref="InterfaceProxyAttribute"/>.
        /// </summary>
        [SerializeField]
        [InterfaceProxy(typeof(ITargetInterface))]
        private float m_badFieldTypeValueType;

        /// <summary>
        /// This field is not an internal unity reference type and cannot be used with <see cref="InterfaceProxyAttribute"/>.
        /// </summary>
        [SerializeField]
        [InterfaceProxy(typeof(ITargetInterface))]
        ExampleNonUnityClass m_badFieldTypeNonUnityReference;

        #endregion

        /// <summary>
        /// This is one example on how to nicely cast the serialized object to the target interface.
        /// </summary>
        public ITargetInterface GetTarget => m_targetObjectField is ITargetInterface result ? result : null;

        public void Start()
        {
            // Cast the serialized object
            var target = GetTarget;

            if (target != null)
            {
                Debug.Log("Successfully cast to ITargetInterface object.");
            }
            else
            {
                Debug.Log("Failed to cast to ITargetInterface. Is the serialized object reference null?");
            }
        }
    }
}
