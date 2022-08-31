using GenericServices.Core.Abstracts;
using Xunit;
namespace CoreTests.Abstracts;

public class AggregateEntityTest
{
    private class TestEntity : AggregateEntity<Guid>
    {

    }

    [Fact]
    public void NewEntity_success()
    {
        var entity = new TestEntity(); 
        Assert.IsType<Guid>(entity.Id);
        Assert.NotNull(entity.CreatedAt);
        Assert.NotNull(entity.UpdatedAt);
    }
}
