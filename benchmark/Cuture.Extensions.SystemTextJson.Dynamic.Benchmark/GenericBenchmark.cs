using System.Text.Json;
using System.Text.Json.Dynamic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Cuture.Extensions.SystemTextJson.Dynamic;

[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class GenericBenchmark
{
    private readonly string _json;
    private readonly User _user;

    public GenericBenchmark()
    {
        _user = new User()
        {
            Name = "name",
            Age = 11,
            Address = new Address()
            {
                Name = "address",
                Code = new()
                {
                    Code = 122,
                    Name = "addressCode",
                }
            }
        };

        _json = JsonSerializer.Serialize(_user);
    }

    [Benchmark]
    public void DynamicJsonAccess()
    {
        var user = JSON.parse(_json);
        if (user!.Address!.Code!.Code != _user.Address!.Code!.Code)
        {
            throw new InvalidOperationException("Deserialize failed.");
        }
    }

    [Benchmark(Baseline = true)]
    public void JsonSerializerAccess()
    {
        var user = JsonSerializer.Deserialize<User>(_json);
        if (user!.Address!.Code!.Code != _user.Address!.Code!.Code)
        {
            throw new InvalidOperationException("Deserialize failed.");
        }
    }
}

internal class Address
{
    public AddressCode? Code { get; set; }
    public string Name { get; set; }
}

internal class User
{
    public Address? Address { get; set; }
    public int? Age { get; set; }
    public string? Name { get; set; }
}

internal class AddressCode
{
    public int Code { get; set; }
    public string Name { get; set; }
}
