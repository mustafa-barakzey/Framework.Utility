using brk.Framework.Utility.Validators;
using Xunit;

namespace brk.Framework.Utility.Tests;

public sealed class NationalIdValidatorTests
{
    [Fact]
    public void IranianValidators_ValidateChecksums() { Assert.True(NationalIdValidator.IsIranNationalId("0123456789")); Assert.True(NationalIdValidator.IsIranNationalId2("0123456789")); Assert.False(NationalIdValidator.IsIranNationalId("1111111111")); }

    [Fact]
    public void AfghanistanValidator_ValidatesStructuralFormat() { Assert.True(NationalIdValidator.IsAfghanistanNationalId("1400-0901-33104")); Assert.False(NationalIdValidator.IsAfghanistanNationalId("1111111111")); }
}
