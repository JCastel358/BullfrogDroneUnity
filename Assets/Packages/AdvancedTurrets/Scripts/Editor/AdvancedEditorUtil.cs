using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AdvancedTurrets.Editor
{
    public static class AdvancedEditorUtil
    {
        // Validates that the field or property from the lambda exists on the instance type T
        public static MemberInfo GetMemberInfo<T, E>(T instance, Expression<Func<T, E>> getFieldOrProperty)
        {
            if (getFieldOrProperty.Body is not MemberExpression memberExpression)
            {
                throw new Exception($"Lambda {getFieldOrProperty} must be a field or property!");
            }

            // Validate member is on specified type and/or a subclass thereof
            var memberInfo = memberExpression.Member;
            if (!ValidateMemberInfo<T>(memberInfo))
            {
                throw new ArgumentException($"Expression '{getFieldOrProperty}' refers to a field or property that is not found on the target type {typeof(T).Name}");
            }

            return memberInfo;
        }

        // Validates that the MemberInfo is available on the target type
        static bool ValidateMemberInfo<T>(MemberInfo memberInfo)
        {
            var tType = typeof(T);
            if (memberInfo.ReflectedType == default)
            {
                // No type found on the member info
                return false;
            }

            if (memberInfo.ReflectedType != tType)
            {
                // The reflected type differs from the specified type
                if (!tType.IsSubclassOf(memberInfo.ReflectedType))
                {
                    // The type isn't a subclass of reflected type
                    return false;
                }
            }

            return true;
        }
    }
}