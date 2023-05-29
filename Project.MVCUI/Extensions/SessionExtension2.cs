using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Project.MVCUI.Extensions
{
    public static class SessionExtension2
    {
        public static void SetSession(this ISession session, string key, object value)
        {
            string serializedObject = JsonConvert.SerializeObject(value);
            session.SetString(key, serializedObject);
        }

        public static T? GetSession<T>(this ISession session, string key) where T : class
        {
            string? stringSession = session.GetString(key);
            if (string.IsNullOrEmpty(stringSession)) return null;

            return JsonConvert.DeserializeObject<T>(stringSession);
        }
    }
}
