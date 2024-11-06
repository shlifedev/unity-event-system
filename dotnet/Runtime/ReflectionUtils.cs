using System;

namespace LD.Framework
{
    public static class EventBusReflectionUtility
    {
        /// <summary>
        /// Determine if a specific type inherits from genericType
        /// </summary>
        /// <param name="genericType">ClassName<> 등의 제네릭 타입을 인자로넣음</param>
        /// <param name="checkType">확인 대상 </param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(this Type checkType, Type genericType)
        {
            while (checkType != null && checkType != typeof(object))
            {
                var cur = checkType.IsGenericType ? checkType.GetGenericTypeDefinition() : checkType;
                if (genericType == cur)
                {
                    return true;
                }
                checkType = checkType.BaseType;
            }
            return false;
        }

    }
}