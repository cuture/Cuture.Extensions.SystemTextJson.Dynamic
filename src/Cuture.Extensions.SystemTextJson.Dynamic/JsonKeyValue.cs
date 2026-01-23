#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning disable IDE0161

#if NETSTANDARD2_0

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}

#endif

namespace System.Text.Json.Dynamic
{
    /// <summary>
    /// Json字段的Key - Value结构
    /// </summary>
    /// <param name="Key">key</param>
    /// <param name="Value">值</param>
    public readonly record struct JsonKeyValuePair(string Key, dynamic? Value)
    {
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator JsonKeyValuePair(KeyValuePair<string, dynamic?> value)
        {
            return new(value.Key, value.Value);
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator KeyValuePair<string, object?>(JsonKeyValuePair value)
        {
            return new(value.Key, value.Value as object);
        }
    }
}
