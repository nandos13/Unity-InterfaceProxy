using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JPAssets.Unity
{
    [CustomPropertyDrawer(typeof(InterfaceProxyAttribute))]
    public sealed class InterfaceProxyPropertyDrawer : PropertyDrawer
    {
        private static MethodInfo s_defaultPropertyFieldMethod;
        private static MethodInfo s_getHelpIconMethod;
        private static GUIContent s_attributeNonInterfaceTypeErrorDialogContent;
        private static GUIContent s_attributeSealedTypeWarningDialogContent;

        private static MethodInfo DefaultPropertyFieldMethod
        {
            get
            {
                if (s_defaultPropertyFieldMethod is null)
                    s_defaultPropertyFieldMethod = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.NonPublic);

                return s_defaultPropertyFieldMethod;
            }
        }

        private static Texture2D GetHelpIcon(MessageType messageType)
        {
            if (s_getHelpIconMethod is null)
                s_getHelpIconMethod = typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.Static | BindingFlags.NonPublic);

            return (Texture2D)s_getHelpIconMethod.Invoke(null, parameters: new object[] { messageType });
        }

        private static GUIContent AttributeNonInterfaceTypeErrorDialogContent
        {
            get
            {
                if (s_attributeNonInterfaceTypeErrorDialogContent == null || s_attributeNonInterfaceTypeErrorDialogContent.image == null)
                {
                    s_attributeNonInterfaceTypeErrorDialogContent = new GUIContent(
                        image: GetHelpIcon(MessageType.Error),
                        tooltip: $"The {nameof(InterfaceProxyAttribute)} attribute on this field has a non-interface type specified as the type argument."
                        );
                }

                return s_attributeNonInterfaceTypeErrorDialogContent;
            }
        }

        private static GUIContent AttributeSealedTypeWarningDialogContent
        {
            get
            {
                if (s_attributeSealedTypeWarningDialogContent is null || s_attributeSealedTypeWarningDialogContent.image == null)
                {
                    s_attributeSealedTypeWarningDialogContent = new GUIContent(
                        image: GetHelpIcon(MessageType.Warning),
                        tooltip: $"The target field is of a sealed type. There may be no benefit to declaring an {nameof(InterfaceProxyAttribute)} attribute here."
                        );
                }

                return s_attributeSealedTypeWarningDialogContent;
            }
        }

        private static Rect GetAlertRect(ref Rect totalRect)
        {
            const float spacing = 8f;
            float iconSize = Mathf.Min(totalRect.height, EditorGUIUtility.singleLineHeight);
            var alertRect = new Rect(totalRect.x + totalRect.width - iconSize, totalRect.y, iconSize, totalRect.height);

            totalRect.width -= iconSize + spacing;

            return alertRect;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Custom property drawer logic should only execute for object reference fields that can be
            // assigned more than one type of object.
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                // Sealed-type fields like GameObject, for example, can only be assigned a single object type
                // and thus do not benefit from being restricted to an arbitrary interface type.
                // Conversely, extendable types - such as Component and ScriptableObject can be inherited
                // to implement any desired interface.
                if (!this.fieldInfo.FieldType.IsSealed)
                {
                    var interfaceProxy = attribute as InterfaceProxyAttribute;
                    var interfaceType = interfaceProxy.InterfaceType;

                    bool isInterfaceType = interfaceType?.IsInterface ?? false;

                    if (isInterfaceType)
                    {
                        // Store the previous object reference in case we want to revert a change
                        var previousObjRef = property.objectReferenceValue;

                        // Draw a property field for the serialized backing field and detect when it's changed by the user
                        EditorGUI.BeginChangeCheck();
                        {
                            EditorGUI.PropertyField(position, property, label);
                        }
                        bool didChange = EditorGUI.EndChangeCheck();

                        if (didChange)
                        {
                            var newObj = property.objectReferenceValue;

                            // If the newly assigned object does not match the interface type restriction, revert the change and log an error
                            if (newObj != null && interfaceType.IsAssignableFrom(newObj.GetType()))
                            {
                                Debug.LogError($"Unable to assign object of type {property.objectReferenceValue.GetType()} as it does not implement the {interfaceType} interface type.");
                                property.objectReferenceValue = previousObjRef;
                            }

                            EditorUtility.SetDirty(property.serializedObject.targetObject);
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        var errorRect = GetAlertRect(ref position);

                        // Draw a disabled property field
                        EditorGUI.BeginDisabledGroup(true);
                        {
                            EditorGUI.PropertyField(position, property, label);
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.LabelField(errorRect, AttributeNonInterfaceTypeErrorDialogContent);
                    }
                }
                else
                {
                    // The field type is sealed. Display a warning next to the property field.
                    var warningRect = GetAlertRect(ref position);

                    EditorGUI.PropertyField(position, property, label);

                    EditorGUI.LabelField(warningRect, AttributeSealedTypeWarningDialogContent);
                }
            }
            else
            {
                DefaultPropertyFieldMethod.Invoke(null, new object[3] { position, property, label });
            }
        }
    }
}
