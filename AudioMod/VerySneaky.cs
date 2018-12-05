using System;
using System.Reflection;

namespace AudioMod
{
    public static class VerySneaky
    {
        /// <summary>
        /// Gets a field from an object using reflection
        /// </summary>
        /// <typeparam name="T">The type of the field</typeparam>
        /// <param name="obj">The object to get field from</param>
        /// <param name="fieldName">The name of the field</param>
        /// <returns>The field</returns>
        public static T GetField<T>(this object obj, string fieldName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if(string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));
           return (T) obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(obj);
        }

        public static void SetField<T>(this object obj, string fieldName, T value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));
            var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}
