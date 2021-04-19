using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JakePerry.Unity
{
    [CustomPropertyDrawer(typeof(InterfaceProxyAttribute))]
    public sealed class InterfaceProxyPropertyDrawer : PropertyDrawer
    {
        private static MethodInfo s_getHelpIconMethod;
        private static GUIContent s_attributeNonInterfaceTypeErrorDialogContent;
        private static GUIContent s_attributeSealedTypeWarningDialogContent;
        private static GUIContent s_fieldNonObjectRefTypeWarningDialogContent;

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

        private static GUIContent FieldSealedTypeWarningDialogContent
        {
            get
            {
                if (s_attributeSealedTypeWarningDialogContent is null || s_attributeSealedTypeWarningDialogContent.image == null)
                {
                    s_attributeSealedTypeWarningDialogContent = new GUIContent(
                        image: GetHelpIcon(MessageType.Warning),
                        tooltip: $"The target field is of a sealed type."
                        );
                }

                return s_attributeSealedTypeWarningDialogContent;
            }
        }

        private static GUIContent FieldNonObjectRefTypeWarningDialogContent
        {
            get
            {
                if (s_fieldNonObjectRefTypeWarningDialogContent is null || s_fieldNonObjectRefTypeWarningDialogContent.image == null)
                {
                    s_fieldNonObjectRefTypeWarningDialogContent = new GUIContent(
                        image: GetHelpIcon(MessageType.Warning),
                        tooltip: $"The target field is not a reference to a unity object."
                        );
                }

                return s_fieldNonObjectRefTypeWarningDialogContent;
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

        private static bool IsUnsupportedType(UnityEngine.Object obj, Type interfaceType)
        {
            return obj != null && !interfaceType.IsAssignableFrom(obj.GetType());
        }

        private static void DisplayFieldWithAlert(Rect position, SerializedProperty property, GUIContent label, bool fieldEnabled, GUIContent alertContent)
        {
            var alertRect = GetAlertRect(ref position);

            EditorGUI.BeginDisabledGroup(!fieldEnabled);
            {
                EditorGUI.PropertyField(position, property, label);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.LabelField(alertRect, alertContent);
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
                        // If an object is assigned that does not match the interface type restriction, assign null and log an error
                        if (IsUnsupportedType(property.objectReferenceValue, interfaceType))
                        {
                            Debug.LogError($"The referenced object of type {property.objectReferenceValue.GetType()} does not implement the {interfaceType} interface type.");
                            property.objectReferenceValue = null;

                            EditorUtility.SetDirty(property.serializedObject.targetObject);
                        }

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
                            // If the newly assigned object does not match the interface type restriction, revert the change and log an error
                            if (IsUnsupportedType(property.objectReferenceValue, interfaceType))
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
                        DisplayFieldWithAlert(position, property, label, false, AttributeNonInterfaceTypeErrorDialogContent);
                    }
                }
                else
                {
                    DisplayFieldWithAlert(position, property, label, true, FieldSealedTypeWarningDialogContent);
                }
            }
            else
            {
                DisplayFieldWithAlert(position, property, label, true, FieldNonObjectRefTypeWarningDialogContent);
            }
        }
    }
}
