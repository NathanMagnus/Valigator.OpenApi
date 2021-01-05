using Functional;
using FluentAssertions;
using System;
using Valigator.OpenApi.AspNetCore.Extensions;
using Xunit;
using Valigator.OpenApi.AspNetCore.Exceptions;

namespace Valigator.OpenApi.AspNetCore.Tests
{
	public class TypeExtensionTests
	{
		public class CreateInstanceTests
		{
			private class CreateInstanceTestClass1
			{
				public Data<int> I { get; private set; }
			}

			public interface ICreateInstanceTestInterface1 { }
			public abstract class CreateInstanceAbstractClass { }

			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(Authorization.Authorization))]
			[InlineData(typeof(CreateInstanceTestClass1))]
			public void ShouldReturnInstanceSuccessfully(Type type)
				=> type
					.CreateInstance()
					.Should()
					.BeSuccessful()
					.AndSuccessValue
					.Should()
					.HaveValue()
					.AndValue
					.Should()
					.BeOfType(type);

			[Theory]
			[InlineData(typeof(ICreateInstanceTestInterface1))]
			[InlineData(typeof(CreateInstanceAbstractClass))]
			public void ShouldReturnInvalidTypeException(Type type)
				=> type
					.CreateInstance()
					.Should()
					.BeFaulted()
					.AndFailureValue
					.Should()
					.BeOfType<InvalidTypeException>();

			[Theory]
			[InlineData(typeof(string))]
			public void ShouldReturnNoneOption(Type type)
				=> type
					.CreateInstance()
					.Should()
					.BeSuccessful()
					.AndSuccessValue
					.Should()
					.NotHaveValue();
		}
	}
}
