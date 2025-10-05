
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AdvancedTurrets.Editor
{
    // This is a bit exploratory and still a work in progress and under development for better QOL/support on runtime objects.
    public class AdvancedEditor<T> : UnityEditor.Editor where T : MonoBehaviour
    {
        protected IEnumerable<T> TTargets => targets.Select(t => (T)t);

        protected virtual void OnEnable()
        {
            _serializedPropertyCache.Clear();
            _onChangedEvents.Clear();
        }

        // Override default GUI to inject our gui function that can be extended.
        public sealed override void OnInspectorGUI()
        {
            var beforeValues = GetBeforeValues();
            OnAdvancedGUI();
            MeasureAfterValues(beforeValues);
        }

        // By default the extension just calls the default - allows for derived types to customize this.
        protected virtual void OnAdvancedGUI()
        {
            base.OnInspectorGUI();
        }

        # region SerializedProperty Caching

        readonly Dictionary<string, SerializedProperty> _serializedPropertyCache = new();

        public bool DrawFieldProperty<V>(Expression<Func<T, V>> getFieldOrProperty)
        {
            var serializedProperty = GetFieldProperty(getFieldOrProperty);
            return EditorGUILayout.PropertyField(serializedProperty);
        }

        public SerializedProperty GetFieldProperty<V>(Expression<Func<T, V>> getFieldOrProperty)
        {
            var memberInfo = AdvancedEditorUtil.GetMemberInfo((T)(object)serializedObject.targetObject, getFieldOrProperty);

            if (!_serializedPropertyCache.TryGetValue(memberInfo.Name, out var result))
            {
                if (memberInfo is FieldInfo fieldInfo)
                {
                    result = serializedObject.FindProperty(fieldInfo.Name);
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    result = serializedObject.FindProperty($"<{propertyInfo.Name}>k__BackingField");
                    if (result == default)
                    {
                        Debug.LogError($"Property {propertyInfo.Name} is not a serializable property");
                    }
                }
                _serializedPropertyCache[memberInfo.Name] = result;
            }

            return result;
        }

        #endregion

        # region ChangeEvents

        public delegate void OnChanged<V>(V from, V to);

        readonly Dictionary<SerializedProperty, Action<object, object>> _onChangedEvents = new();

        // Invoke onchanged whenever the specified field or property changes
        public void AddListener<V>(Expression<Func<T, V>> getFieldOrProperty, OnChanged<V> onChanged)
        {
            if (target == default)
            {
                return;
            }

            var serializedProperty = GetFieldProperty(getFieldOrProperty);
            if (!_onChangedEvents.ContainsKey(serializedProperty))
            {
                _onChangedEvents[serializedProperty] = OnValueChanged;
                void OnValueChanged(object from, object to)
                {
                    onChanged.Invoke((V)from, (V)to);
                }
            }
        }

        public Dictionary<SerializedProperty, object> GetBeforeValues()
        {
            var beforeValues = new Dictionary<SerializedProperty, object>();
            foreach (var serializedProperty in _onChangedEvents.Keys)
            {
                beforeValues[serializedProperty] = GetValue(serializedProperty);
            }
            return beforeValues;
        }

        public void MeasureAfterValues(Dictionary<SerializedProperty, object> beforeValues)
        {
            foreach (var beforeKvp in beforeValues)
            {
                var serialiezedProperty = beforeKvp.Key;
                var from = beforeKvp.Value;
                var to = GetValue(serialiezedProperty);

                if (from == default)
                {
                    if (to != default)
                    {
                        _onChangedEvents[serialiezedProperty].Invoke(from, to);
                    }
                }
                else if (!from.Equals(to))
                {
                    _onChangedEvents[serialiezedProperty].Invoke(from, to);
                }
            }
        }

        public static object GetValue(SerializedProperty serializedProperty)
        {
            var parentType = serializedProperty.serializedObject.targetObject.GetType();
            var fieldInfo = parentType.GetField(serializedProperty.propertyPath);
            return fieldInfo.GetValue(serializedProperty.serializedObject.targetObject);
        }

        #endregion
    }
}