namespace System.Text.Json.Dynamic;

[TestClass]
public class DynamicEnumerableTest
{
    #region Public 方法

    [TestMethod]
    public void Should_Array_EqualOrigin()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        var index = 0;
        foreach (var item in json.MyProperty6)
        {
            Check(origin.MyProperty6[index++], item);
        }

        index = 0;

        foreach (dynamic item in json.MyProperty7)
        {
            Check(origin.MyProperty7[index++], item);
        }

        static void Check(object originValue, dynamic value)
        {
            //直接比较有点麻烦。。直接转json字符串比较
            Assert.AreEqual(JsonSerializer.Serialize(originValue), JSON.stringify(value));
        }
    }

    [TestMethod]
    public void ShouldCastFail()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.ThrowsExactly<InvalidCastException>(() => (IEnumerable<dynamic>)json.MyProperty6);
        Assert.ThrowsExactly<InvalidCastException>(() => (IEnumerable<KeyValuePair<string, dynamic?>>)json.MyProperty6);
        Assert.ThrowsExactly<InvalidCastException>(() => (IDynamicKeyValueEnumerable)json.MyProperty6);

        Assert.ThrowsExactly<InvalidCastException>(() => (IEnumerable<dynamic>)json);
        Assert.ThrowsExactly<InvalidCastException>(() => (IEnumerable<KeyValuePair<string, dynamic?>>)json);
        Assert.ThrowsExactly<InvalidCastException>(() => (IDynamicEnumerable)json);
    }

    [TestMethod]
    public void ShouldCastSuccess()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        #region IDynamicEnumerable

        {
            IDynamicEnumerable dynamicEnumerable = json.MyProperty6;
            Assert.IsNotNull(dynamicEnumerable);

            dynamicEnumerable = (IDynamicEnumerable)json.MyProperty6;
            Assert.IsNotNull(dynamicEnumerable);

            dynamicEnumerable = json.MyProperty6;
            Assert.IsNotNull(dynamicEnumerable);
        }

        #endregion IDynamicEnumerable

        #region IDynamicKeyValueEnumerable

        {
            IDynamicKeyValueEnumerable dynamicKeyValueEnumerable = json;
            Assert.IsNotNull(dynamicKeyValueEnumerable);

            dynamicKeyValueEnumerable = (IDynamicKeyValueEnumerable)json;
            Assert.IsNotNull(dynamicKeyValueEnumerable);

            dynamicKeyValueEnumerable = json;
            Assert.IsNotNull(dynamicKeyValueEnumerable);
        }

        #endregion IDynamicKeyValueEnumerable

        #region IEnumerable<dynamic>

        {
            IEnumerable<dynamic> enumerable = json.MyProperty6;
            Assert.IsNotNull(enumerable);

            enumerable = json;
            Assert.IsNotNull(enumerable);
        }

        #endregion IEnumerable<dynamic>

        #region IEnumerable<dynamic>

        {
            IEnumerable<KeyValuePair<string, dynamic?>> enumerable = json;
            Assert.IsNotNull(enumerable);
        }

        #endregion IEnumerable<dynamic>
    }

    [TestMethod]
    public void ShouldEqualLinqOnOrigin()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        var enumerable = ((IDynamicEnumerable)json.MyProperty6).AsEnumerable();

        Assert.AreEqual(origin.MyProperty6.Length, enumerable.Count());
        Assert.AreEqual(origin.MyProperty6.First(), enumerable.First());
        Assert.AreEqual(origin.MyProperty6.Skip(1).First(), enumerable.Skip(1).First());

        var filtered = origin.MyProperty6.Where(m => m[0] > '1').ToArray();
        var dynamicFiltered = enumerable.Where(m => m[0] > '1').ToArray();

        Assert.HasCount(filtered.Length, dynamicFiltered);

        for (int i = 0; i < filtered.Length; i++)
        {
            Assert.AreEqual(filtered[i], dynamicFiltered[i]);
        }
    }

    [TestMethod]
    public void ShouldEqualOrigin()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        var enumerable = ((IDynamicEnumerable)json.MyProperty6).AsEnumerable();

        var dynamicToArray = enumerable.ToArray();

        Assert.HasCount(origin.MyProperty6.Length, dynamicToArray);

        for (int i = 0; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[i], dynamicToArray[i]);
        }
    }

    [TestMethod]
    public void ShouldEqualOriginProperty()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        //IDynamicKeyValueEnumerable
        {
            var enumerable = ((IDynamicKeyValueEnumerable)json).AsEnumerable();

            foreach (var item in enumerable)
            {
                Check(origin, item.Key, item.Value);
            }
        }

        //foreach
        foreach (var item in json)
        {
            Check(origin, item.Key, item.Value);
        }

        //typed foreach
        foreach (JsonKeyValuePair item in json)
        {
            Check(origin, item.Key, item.Value);
        }

        //IEnumerable<dynamic>
        {
            IEnumerable<dynamic> dynamicEnumerable = json;

            foreach (var item in dynamicEnumerable)
            {
                string key = item.Key;
                Assert.IsNotNull(key);
                dynamic value = item.Value;

                Check(origin, key, value);
            }
        }

        static void Check(object origin, string key, dynamic value)
        {
            var type = origin.GetType();
            var originProperty = type.GetProperty(key);
            Assert.IsNotNull(originProperty);

            var originValue = originProperty.GetValue(origin);

            //直接比较有点麻烦。。直接转json字符串比较
            Assert.AreEqual(JsonSerializer.Serialize(originValue), JSON.stringify(value));
        }
    }

    [TestMethod]
    public void ShouldModifyPropertySuccess()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);
        DynamicJSONTestClass.GetTestValue(out var origin2, out var json2);

        //IDynamicEnumerable
        {
            var originEnumerable = ((IDynamicKeyValueEnumerable)json2.MyProperty8).AsEnumerable();

            foreach (var item in ((IDynamicKeyValueEnumerable)json.MyProperty8).AsEnumerable())
            {
                item.Value.Value1 = item.Value.Value1 + 1;
            }

            foreach (var item in ((IDynamicKeyValueEnumerable)json.MyProperty8).AsEnumerable())
            {
                Check(origin2.MyProperty8, item.Key, item.Value);
            }
        }

        //IDynamicEnumerable
        {
            var originEnumerable = ((IDynamicEnumerable)json2.MyProperty7).AsEnumerable();

            foreach (var item in ((IDynamicEnumerable)json.MyProperty7).AsEnumerable())
            {
                item.Value1 = item.Value1 + 1;
            }

            int index = 0;
            foreach (var item in ((IDynamicEnumerable)json.MyProperty7).AsEnumerable())
            {
                AreNotEqual(originEnumerable.Skip(index++).First(), item);
            }
        }

        static void Check(object origin, string key, dynamic value)
        {
            var type = origin.GetType();
            var originProperty = type.GetProperty(key);
            Assert.IsNotNull(originProperty);

            var originValue = originProperty.GetValue(origin);

            AreNotEqual(originValue, value);
        }

        static void AreNotEqual(object originValue, dynamic value)
        {
            //直接比较有点麻烦。。直接转json字符串比较
            Assert.AreNotEqual(JsonSerializer.Serialize(originValue), JSON.stringify(value));
        }
    }

    #endregion Public 方法
}
