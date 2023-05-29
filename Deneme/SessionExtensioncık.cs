using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deneme
{
    public static class SessionExtensioncık
    {
        public static void SetSession(this ISession session, string key, object value)
        {
            session.SetString(key, value.ToString());
        }
    }
}
