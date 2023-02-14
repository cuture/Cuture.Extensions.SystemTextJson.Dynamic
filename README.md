# Cuture.Extensions.SystemTextJson.Dynamic

Extension of `System.Text.Json` to support dynamic access using `dynamic`

对 `System.Text.Json` 的拓展，以支持使用 `dynamic` 进行动态访问

 - 使用 `dynamic` 访问，无需预定义实体类型
 - 支持修改/添加属性

## 创建 `JSON` 对象

### 从 `Json字符串` 创建 `JSON` 对象
```C#
using System.Text.Json.Dynamic;

var rawJson = 
    """
    {
    "a":1,
    "b":2.0,
    "c":{
        "a":"1",
        "b":"2",
        },
    }
    """;

var json = JSON.parse(rawJson);
```

### 从 `对象` 创建 `JSON` 对象
```C#
using System.Text.Json.Dynamic;

var json = JSON.create(new object());
```

## 访问 `JSON` 对象

```C#
//访问属性
var a = json.a;
var c = json.a.b.c;

//访问数组
var v2 = json.x[1];

//访问属性并转换为基础数据类型
bool b1 = json.b;
string s1 = json.s;

//局部反序列化
MyClass obj = json.a.b; //将 json.a.b 反序列化到类型 MyClass

//获取内部的原始 System.Text.Json.Nodes.* 对象
JsonArray array = json.ArrayProperty;
JsonObject obj = json.ObjectProperty;
JsonNode node = json.NodeProperty;
```

## 修改 `JSON` 对象

```C#
//设置值（不可多级设置未定义字段）
json.a = new {};
json.a.b = 1;

json.c = new
{
    d = new
    {
        e = 2
    }
};

int value = json.a.b;
```

## 检查 `JSON` 对象

```C#
//判断是否为未定义字段
var b1 = json.notexistfield == null;
var b2 = json.notexistfield == JSON.Undefined;
var b3 = JSON.isUndefined(json.notexistfield);

//多级属性判断是否为未定义字段
var b4 = JSON.isUndefined(() => json.a.b.c.e.f.g.h.i.j.k);
```

## IEnumerable

使用`IEnumerable`以支持使用Linq, (C#不支持实现dynamic的接口，所以需要额外的转换)

```C#
//遍历Array
//显式赋值类型
IEnumerable<dynamic> enumerable = json.Array;
//通过IDynamicEnumerable
var enumerable = ((IDynamicEnumerable)json.Array).AsEnumerable();

//遍历属性
//显式赋值类型
IEnumerable<KeyValuePair<string, dynamic?>> enumerable = json;
//通过IDynamicKeyValueEnumerable
var enumerable = ((IDynamicKeyValueEnumerable)json).AsEnumerable();
```
