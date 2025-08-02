namespace System.Text.Json.Dynamic;

[TestClass]
public class JSONArrayMethodTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldGetFirstSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);
        Assert.AreEqual(origin.MyProperty6.First(), json.MyProperty6.first());

        json.MyProperty6.clear();

        Assert.AreEqual<object>(null, json.MyProperty6.first());
    }

    [TestMethod]
    public void ShouldGetLastSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);
        Assert.AreEqual(origin.MyProperty6.Last(), json.MyProperty6.last());

        json.MyProperty6.clear();

        Assert.AreEqual<object>(null, json.MyProperty6.last());
    }

    [TestMethod]
    public void ShouldInsertOverRangeSuccessful()
    {
        const int Count = 500;
        var array = JSON.parse("[]");

        Assert.AreEqual(0, array!.length);

        array[Count] = 1;

        Assert.AreEqual(Count + 1, array.length);

        for (int i = 0; i < Count; i++)
        {
            Assert.AreEqual<object>(null, array[i]);
        }

        Assert.AreEqual(1, array[Count]);
    }

    [TestMethod]
    public void ShouldInsertSuccessful()
    {
        const int Count = 500;
        var array = JSON.parse("[]");

        Assert.AreEqual(0, array!.length);

        for (int i = 0; i < Count; i++)
        {
            Assert.AreEqual(i, array.insert(0, i));
        }

        Assert.AreEqual(Count, array.length);

        for (int i = 0; i < Count; i++)
        {
            Assert.AreEqual(Count - i - 1, array[i]);
        }
    }

    [TestMethod]
    public void ShouldPopSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.AreEqual(origin.MyProperty6.Length, json.MyProperty6.length);

        for (var i = origin.MyProperty6.Length - 1; i >= 0; i--)
        {
            Assert.AreEqual(origin.MyProperty6[i], json.MyProperty6.pop());
        }

        Assert.AreEqual(0, json.MyProperty6.length);

        Assert.AreEqual<object>(null, json.MyProperty6.pop());
    }

    [TestMethod]
    public void ShouldPushSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.AreEqual(origin.MyProperty6.Length, json.MyProperty6.length);

        for (var i = 1; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[i], json.MyProperty6[i]);
        }

        const int PushValue = 500;
        json.MyProperty6.push(PushValue);

        for (var i = 1; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[i], json.MyProperty6[i]);
        }

        Assert.AreEqual(origin.MyProperty6.Length + 1, json.MyProperty6.length);

        Assert.AreEqual(PushValue, json.MyProperty6[origin.MyProperty6.Length]);
    }

    [TestMethod]
    public void ShouldRemoveAllSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);
        Assert.AreEqual(origin.MyProperty6.Length, json.MyProperty6.length);

        json.MyProperty6.clear();

        Assert.AreEqual(0, json.MyProperty6.length);

        DynamicJSONTestClass.GetTestValue(out origin, out json);
        Assert.AreEqual(origin.MyProperty6.Length, json.MyProperty6.length);

        json.MyProperty6.removeall();

        Assert.AreEqual(0, json.MyProperty6.length);
    }

    [TestMethod]
    public void ShouldRemoveAtSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.AreEqual(origin.MyProperty6.Length, json.MyProperty6.length);

        var index = 0;
        while (json.MyProperty6.length > 0)
        {
            Assert.AreEqual(origin.MyProperty6[index++], json.MyProperty6.removeat(0));
        }

        Assert.AreEqual(0, json.MyProperty6.length);
    }

    #endregion Public 方法
}
