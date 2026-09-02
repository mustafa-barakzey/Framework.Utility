using brk.Framework.Utility.Helpers;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class CultureHelperTests
{
    [Fact]
    public void CultureFactories_ReturnConfiguredCultures() { Assert.Equal("fa-IR", CultureHelper.GetIranCulture().Name); Assert.Equal("prs-AF", CultureHelper.GetAfghanCulture().Name); Assert.Equal("ps-AF", CultureHelper.GetPashtoCulture().Name); }
}
