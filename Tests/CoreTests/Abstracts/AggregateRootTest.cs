using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreTests.Abstracts;

public class AggregateRootTest
{
    private TestRoot root;

    public AggregateRootTest()
    {
        root = new() { Name = "Name1", Family = "Family1" };
        root.TestEntitites = new List<TestEntity>();
        root.TestEntitites.Add(new TestEntity() { Field1 = 1, Field2 = "Test Field 1" });
        root.TestEntitites.Add(new TestEntity() { Field1 = 2, Field2 = "Test Field 2" });
    }
    [Fact]
    public void GetSubEntitiesFromRoot_success()
    {
        var list = root.GetEntities<TestEntity>();
        Assert.NotNull(list);
        Assert.Equal(list.Count(), 2);
        Assert.Equal(list, root.TestEntitites);
    }
    
    [Fact]
    public void CopyProperties_success()
    {
        TestRoot root2= new();
        root2.UpdateValues(root);
        Assert.Equal(root.Name,root2.Name);
        Assert.Equal(root.Family,root2.Family);
        Assert.Equal(root.TestEntitites,root2.TestEntitites);
    }
}
