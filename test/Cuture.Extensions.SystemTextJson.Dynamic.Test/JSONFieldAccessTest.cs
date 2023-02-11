using System.Runtime.CompilerServices;

namespace System.Text.Json.Dynamic;

[TestClass]
public class JSONFieldAccessTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldReturnUndefinedForErrorField()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.IsTrue(json.notexistfield == null);
        Assert.IsTrue(json.notexistfield == JSON.Undefined);
        Assert.IsTrue(JSON.isUndefined(json.notexistfield));
    }

    #endregion Public 方法
}
