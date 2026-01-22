namespace System.Text.Json.Dynamic;

[TestClass]
public class JSONisArrayTest
{
    #region Public 方法

    [TestMethod]
    public void Should_Check_Success()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.IsFalse(JSON.isArray(json.MyProperty1));
        Assert.IsFalse(JSON.isArray(json.MyProperty2));
        Assert.IsFalse(JSON.isArray(json.MyProperty3));
        Assert.IsFalse(JSON.isArray(json.MyProperty4));
        Assert.IsFalse(JSON.isArray(json.MyProperty5));

        Assert.IsTrue(JSON.isArray(json.MyProperty6));
        Assert.IsTrue(JSON.isArray(origin.MyProperty6));
        Assert.IsTrue(JSON.isArray(json.MyProperty7));
        Assert.IsTrue(JSON.isArray(origin.MyProperty7));
    }

    #endregion Public 方法
}
