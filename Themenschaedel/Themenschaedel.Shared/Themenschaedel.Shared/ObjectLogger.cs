using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Themenschaedel.Shared
{
    public static class ObjectLogger
    {
        public static string Dump(object obj) => JsonSerializer.Serialize(obj);
    }
}
