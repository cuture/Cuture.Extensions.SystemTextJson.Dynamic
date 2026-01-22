namespace System.Text.Json.Dynamic;

internal class DynamicJSONTestClass
{
    #region Public 属性

    public int MyProperty1 { get; set; } = 1;

    public double MyProperty2 { get; set; } = 2;

    public string MyProperty3 { get; set; } = nameof(MyProperty3);

    public object MyProperty4 { get; set; } = new
    {
        a = 1,
        b = 2,
        c = new
        {
            a = "3",
            b = "4"
        }
    };

    public string[] MyProperty6 { get; set; } = ["1", "2", "3", "4", "5"];

    public NestedClass[] MyProperty7 { get; set; } = [new() { Value1 = 1, Value2 = "1" }, new() { Value1 = 2, Value2 = "2" }, new() { Value1 = 3, Value2 = "3" }];

    public NestedClassCollection MyProperty8 { get; set; } = new();

    public int? NullableProperty { get; set; } = null;

    public object? NullProperty { get; set; } = null;

    #endregion Public 属性

    #region Public 方法

    public static void GetTestValue(out DynamicJSONTestClass origin, out dynamic json)
    {
        origin = new DynamicJSONTestClass();
        json = JSON.create(origin);
        origin.Check(json);
    }

    public void Check(dynamic json)
    {
        Assert.AreEqual(MyProperty1, json.MyProperty1);
        Assert.AreEqual(MyProperty2, json.MyProperty2);
        Assert.AreEqual(MyProperty3, json.MyProperty3);
        Assert.AreEqual(1, json.MyProperty4.a);
        Assert.AreEqual(2, json.MyProperty4.b);
        Assert.AreEqual("3", json.MyProperty4.c.a);
        Assert.AreEqual("4", json.MyProperty4.c.b);

        if (MyProperty6?.Length > 0)
        {
            for (int i = 0; i < MyProperty6.Length; i++)
            {
                Assert.AreEqual(MyProperty6[i], json.MyProperty6[i]);
            }
        }

        if (MyProperty6?.Length > 0)
        {
            for (int i = 0; i < MyProperty6.Length; i++)
            {
                Assert.AreEqual(MyProperty6[i], json.MyProperty6[i]);
            }
        }
    }

    #endregion Public 方法

    #region Public 类

    public class NestedClass
    {
        #region Public 属性

        public int Value1 { get; set; }

        public string Value2 { get; set; }

        #endregion Public 属性
    }

    public class NestedClassCollection
    {
        #region Public 属性

        public NestedClass Property1 { get; set; } = new() { Value1 = 1, Value2 = "1" };

        public NestedClass Property2 { get; set; } = new() { Value1 = 2, Value2 = "3" };

        public NestedClass Property3 { get; set; } = new() { Value1 = 2, Value2 = "3" };

        #endregion Public 属性
    }

    #endregion Public 类
}
