using brk.Framework.Utility.Extensions;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class NumberExtensionsTests
{
    [Fact]
    public void NullAndPositiveChecks_WorkForSupportedTypes() { string? missing = null; Assert.True(missing.IsNull()); Assert.True("value".IsNotNull()); Assert.True(1.IsGreaterThanZero()); Assert.True(((int?)1).IsGreaterThanZero()); Assert.True(1m.IsGreaterThanZero()); Assert.True(((decimal?)1m).IsGreaterThanZero()); Assert.Equal(12.5m, 12.5m.Normalize()); }
}
