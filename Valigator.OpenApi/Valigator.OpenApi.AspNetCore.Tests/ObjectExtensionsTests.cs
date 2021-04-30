using Functional;
using FluentAssertions;
using Valigator.OpenApi.AspNetCore.Extensions;
using Xunit;

namespace Valigator.OpenApi.AspNetCore.Tests
{
	public class ObjectExtensionsTests
	{
		public class AsTests
		{
			[Fact]
			public void HasProperValue()
				=> ObjectExtensions.As<int>((object)1).Should().HaveValue().AndValue.Should().Be(1);

			[Fact]
			public void HasOptionNone()
				=> ObjectExtensions.As<int>(new object()).Should().NotHaveValue();
		}
	}
}
