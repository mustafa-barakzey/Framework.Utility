using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using brk.Framework.Utility.Extensions;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class EnumExtensionsTests
{
    [Fact]
    public void FlagConversionsAndCount_Work() { var flags = new[] { TestFlags.One, TestFlags.Two }.ToFlag(); Assert.Equal(TestFlags.One | TestFlags.Two, flags); Assert.Equal(2, flags.Count()); Assert.Equal([TestFlags.One, TestFlags.Two], flags.ToList()); Assert.Equal(default, ((IEnumerable<TestFlags>?)null).ToFlag()); }

    [Fact]
    public void MetadataAndDictionary_AreResolved() { Assert.Equal("description", TestFlags.One.GetDescription()); Assert.Equal("display", TestFlags.Two.GetDisplayName()); Assert.Equal("Three", TestFlags.Three.GetDisplayName()); Assert.Equal("display", EnumExtensions.GetItemsAsDictionary<TestFlags>()[TestFlags.Two]); }

    [Flags] private enum TestFlags { [Description("description")] One = 1, [Display(Name = "display")] Two = 2, Three = 4 }
}
