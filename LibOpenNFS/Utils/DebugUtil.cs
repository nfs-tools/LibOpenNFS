using System;
using System.Runtime.CompilerServices;

namespace LibOpenNFS.Utils
{
    /// <summary>
    /// Debugging utilities.
    /// </summary>
    public static class DebugUtil
    {
        /// <summary>
        /// Ensure that a condition is <code>true</code>, or throw an exception if it is <code>false</code>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="exceptionMessage"></param>
        /// <param name="callerName"></param>
        /// <exception cref="Exception"></exception>
        public static void EnsureCondition(bool condition, Func<string> exceptionMessage, [CallerMemberName] string callerName = "")
        {
            if (!condition)
            {
                throw new Exception($"[{callerName}]: {exceptionMessage()}");
            }
        }
        
        /// <summary>
        /// Ensure that a condition is <code>true</code>, or throw an exception if it is <code>false</code>.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="exceptionMessage"></param>
        /// <param name="callerName"></param>
        public static void EnsureCondition(Predicate<object> condition, Func<string> exceptionMessage, [CallerMemberName] string callerName = "")
        {
            EnsureCondition(condition.Invoke(new object()), exceptionMessage);
        }
    }
}