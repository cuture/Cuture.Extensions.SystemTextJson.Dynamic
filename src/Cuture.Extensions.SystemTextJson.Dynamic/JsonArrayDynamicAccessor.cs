using System.Collections;
using System.Dynamic;
using System.Text.Json.Nodes;

namespace System.Text.Json.Dynamic;

internal class JsonArrayDynamicAccessor
    : JsonDynamicAccessor
    , IEnumerable
    , IDynamicEnumerable
{
    #region Private 字段

    private readonly JsonArray _jsonArray;

    #endregion Private 字段

    #region Public 属性

    public int Length => _jsonArray.Count;

    #endregion Public 属性

    #region Public 构造函数

    public JsonArrayDynamicAccessor(JsonArray jsonArray) : base(jsonArray)
    {
        _jsonArray = jsonArray ?? throw new ArgumentNullException(nameof(jsonArray));
    }

    #endregion Public 构造函数

    #region Public 方法

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        yield return "Length";
    }

    public IEnumerator GetEnumerator()
    {
        return new JsonArrayEnumerator(_jsonArray);
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.ReturnType == typeof(JsonArray))
        {
            result = _jsonArray;
            return true;
        }
        return base.TryConvert(binder, out result);
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        //TODO 多维数组
        if (indexes.Length > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(indexes));
        }

        var index = indexes[0];

        if (index is int intIndex)
        {
            result = JsonNodeUtil.GetNodeAccessValue(_jsonArray[intIndex]);
            return true;
        }

#if NET6_0_OR_GREATER

        //低版本可以靠手动定义 System.Index 和 System.Range 类型进行兼容，但会污染命名空间，容易出现冲突
        else if (index is Index systemIndex)
        {
            result = JsonNodeUtil.GetNodeAccessValue(_jsonArray[systemIndex.GetOffset(_jsonArray.Count)]);
            return true;
        }
        else if (index is Range systemRange)
        {
            var (offset, length) = systemRange.GetOffsetAndLength(_jsonArray.Count);

            //创建一个新的 Json 对象进行操作
            //TODO 封装一个针对 JsonNode[] 的包装类，以不用创建新的 Json 对象
            using var memoryStream = new MemoryStream();
            using var writer = new Utf8JsonWriter(memoryStream, JSON.s_defaultJsonWriterOptions);

            writer.WriteStartArray();

            foreach (var item in _jsonArray.AsEnumerable().Skip(offset).Take(length))
            {
                if (item is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    item.WriteTo(writer, JSON.s_defaultJsonSerializerOptions);
                }
            }

            writer.WriteEndArray();

            writer.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);

            result = JsonNodeUtil.GetNodeAccessValue(JsonNode.Parse(memoryStream, JSON.s_defaultJsonNodeOptions, JSON.s_defaultJsonDocumentOptions));

            return true;
        }

#endif
        throw new ArgumentException($"not support for index {index}.");
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (string.Equals(binder.Name, "Length", StringComparison.OrdinalIgnoreCase))
        {
            result = _jsonArray.Count;
            return true;
        }
        return base.TryGetMember(binder, out result);
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        switch (binder.Name.ToLowerInvariant())
        {
            case "insert":
                {
                    if (args?.Length != 2
                        || args[0] is not int index)
                    {
                        throw new InvalidOperationException($"Method \"{binder.Name}\" for array must has 2 argument with 'index' and 'value'.");
                    }

                    if (index < _jsonArray.Count)
                    {
                        var node = JsonNode.Parse(JSON.stringify(args[1]));
                        _jsonArray.Insert(index, node);

                        result = JsonNodeUtil.GetNodeAccessValue(node);
                    }
                    else
                    {
                        result = SetValueAtIndex(index, args[1]);
                    }

                    return true;
                }

            case "append":
            case "add":
            case "push":
                {
                    if (args?.Length > 0 != true)
                    {
                        throw new InvalidOperationException($"Method \"{binder.Name}\" for array must has argument.");
                    }

                    result = null;

                    foreach (var item in args)
                    {
                        result = SetValueAtIndex(_jsonArray.Count, item);
                    }
                    return true;
                }

            case "pop":
                {
                    var index = _jsonArray.Count - 1;
                    if (index >= 0)
                    {
                        result = JsonNodeUtil.GetNodeAccessValue(_jsonArray[index]);
                        _jsonArray.RemoveAt(index);
                    }
                    else
                    {
                        result = null;
                    }
                    return true;
                }

            case "removeat":
                {
                    if (args?.Length != 1
                        || args[0] is not int index)
                    {
                        throw new InvalidOperationException($"Method \"{binder.Name}\" for array must has 1 argument with 'index'.");
                    }
                    var node = _jsonArray[index];
                    _jsonArray.RemoveAt(index);
                    result = JsonNodeUtil.GetNodeAccessValue(node);
                    return true;
                }

            case "clear":
            case "removeall":
                {
                    while (_jsonArray.Count > 0)
                    {
                        _jsonArray.RemoveAt(0);
                    }
                    result = null;
                    return true;
                }

            case "first":
            case "firstordefault":
                {
                    result = _jsonArray.Count > 0
                             ? JsonNodeUtil.GetNodeAccessValue(_jsonArray[0])
                             : null;
                    return true;
                }

            case "last":
            case "lastordefault":
                {
                    result = _jsonArray.Count > 0
                             ? JsonNodeUtil.GetNodeAccessValue(_jsonArray[_jsonArray.Count - 1])
                             : null;
                    return true;
                }
        }
        return base.TryInvokeMember(binder, args, out result);
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        if (indexes.Length > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(indexes));
        }

        var index = indexes[0];

        if (index is int intIndex)
        {
            SetValueAtIndex(intIndex, value);
            return true;
        }
#if NET6_0_OR_GREATER

        //低版本可以靠手动定义 System.Index 和 System.Range 类型进行兼容，但会污染命名空间，容易出现冲突
        else if (index is Index systemIndex)
        {
            _jsonArray[systemIndex.GetOffset(_jsonArray.Count)] = JsonNode.Parse(JSON.stringify(value));
            return true;
        }
#endif

        throw new ArgumentException($"not support for index {index}.");
    }

    #region IDynamicEnumerable

    public IEnumerable<dynamic?> AsEnumerable()
    {
        var enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    #endregion IDynamicEnumerable

    #endregion Public 方法

    #region Private 方法

    private object? SetValueAtIndex(int index, object? value)
    {
        while (index >= _jsonArray.Count)
        {
            _jsonArray.Add(null);
        }
        var node = JsonNode.Parse(JSON.stringify(value));
        _jsonArray[index] = node;

        return JsonNodeUtil.GetNodeAccessValue(node);
    }

    #endregion Private 方法

    #region Private 类

    private class JsonArrayEnumerator : IEnumerator
    {
        #region Private 字段

        private readonly JsonArray _jsonArray;

        private int _index = -1;

        #endregion Private 字段

        #region Public 属性

        public object Current => JsonNodeUtil.GetNodeAccessValue(_jsonArray[_index])!;

        #endregion Public 属性

        #region Public 构造函数

        public JsonArrayEnumerator(JsonArray jsonArray)
        {
            _jsonArray = jsonArray;
        }

        #endregion Public 构造函数

        #region Public 方法

        public bool MoveNext()
        {
            if (_index < _jsonArray.Count - 1)
            {
                _index++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
