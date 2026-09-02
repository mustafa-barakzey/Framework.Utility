using System.ComponentModel;
using brk.Framework.Utility.Helpers;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class AttributeHelperTests
{
    [Fact]
    public void AttributeQueries_FindDecoratedProperties() { Assert.True(AttributeHelper.HasThisAttribute<Model, DisplayNameAttribute>("name")); Assert.False(AttributeHelper.HasThisAttribute<Model, DisplayNameAttribute>("Missing")); Assert.Single(AttributeHelper.GetPropertiesByAttribute<Model, DisplayNameAttribute>()); }
    private sealed class Model { [DisplayName("Name")] public string Name { get; set; } = string.Empty; public string Value { get; set; } = string.Empty; }
}
