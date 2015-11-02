using System;

namespace DocumentDbDemo.Utils
{
    public static class PreRequisite
    {
        public static void NotNull<TObject>(TObject obj, string paramName = "", string message = "")
        {
            if (obj == null) throw new ArgumentNullException(paramName, message);
        }

        public static void NotNullOrEmpty(string value, string paramName = "", string message = "")
        {
            if (value == null) throw new ArgumentNullException(paramName, message);
            if (value == string.Empty) throw new ArgumentException(message, paramName);
        }

        public static void NotNullOrWhiteSpace(string value, string paramName = "", string message = "")
        {
            if (value == null) throw new ArgumentNullException(paramName, message);
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(message, paramName);
        }


    }
}
