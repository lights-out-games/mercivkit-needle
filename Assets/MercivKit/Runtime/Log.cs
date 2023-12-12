using UnityEngine;

namespace MercivKit
{
    public class Log
    {
        [System.Diagnostics.Conditional("MERCIV_LOG_VERBOSE")]
        public static void Verbose(string message)
        {
            Debug.Log("C#: " + message);
        }

        public static void Info(string message)
        {
            Debug.Log("C#: " + message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning("C#: " + message);
        }

        public static void Error(string message)
        {
            Debug.LogError("C#: " + message);
        }

        public static void Exception(System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}