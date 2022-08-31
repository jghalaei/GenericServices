using GenericServices.Core.Abstracts;

namespace CoreTests.Abstracts;

public class TestRoot : AggregateRoot<TestRoot,Guid>
{
public string Name { get; set; }
public string Family { get; set; }
public List<TestEntity> TestEntitites { get; set; }
}
