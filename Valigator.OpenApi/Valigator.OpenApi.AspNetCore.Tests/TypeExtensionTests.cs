using Functional;
using FluentAssertions;
using System;
using Valigator.OpenApi.AspNetCore.Extensions;
using Xunit;
using Valigator.OpenApi.AspNetCore.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;

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
			public abstract class CreateInstanceAbstractClass1 { }

			[Theory]
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
			[InlineData(typeof(CreateInstanceAbstractClass1))]
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
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			public void ShouldReturnNoneOption(Type type)
				=> type
					.CreateInstance()
					.Should()
					.BeSuccessful()
					.AndSuccessValue
					.Should()
					.NotHaveValue();
		}

		public class IsValigatorDataTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(Data<>))]
			[InlineData(typeof(Data<int>))]
			[InlineData(typeof(Data<object>))]
			[InlineData(typeof(Data<decimal>))]
			[InlineData(typeof(Data<TestClass1>))]
			public void ShouldBeTrue(Type type)
				=> type.IsValigatorData().Should().BeTrue();

			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(object))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(TestClass1))]
			public void ShouldBeFalse(Type type)
				=> type.IsValigatorData().Should().BeFalse();
		}

		public class IsTaskGenericTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(Task<>))]
			[InlineData(typeof(Task<int>))]
			[InlineData(typeof(Task<decimal>))]
			[InlineData(typeof(Task<object>))]
			[InlineData(typeof(Task<TestClass1>))]
			[InlineData(typeof(Task<IInterface>))]
			public void ShouldBeTrue(Type type)
				=> type.IsTaskGeneric().Should().BeTrue();


			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(object))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeFalse(Type type)
				=> type.IsTaskGeneric().Should().BeFalse();
		}

		public class IsDictionaryTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(Dictionary<,>))]
			[InlineData(typeof(Dictionary<int, int>))]
			[InlineData(typeof(Dictionary<object, object>))]
			[InlineData(typeof(Dictionary<IInterface, IInterface>))]
			[InlineData(typeof(Dictionary<TestClass1, TestClass1>))]
			[InlineData(typeof(IDictionary<,>))]
			[InlineData(typeof(IDictionary<int, int>))]
			[InlineData(typeof(IDictionary<object, object>))]
			[InlineData(typeof(IDictionary<IInterface, IInterface>))]
			[InlineData(typeof(IDictionary<TestClass1, TestClass1>))]
			public void ShouldBeTrue(Type type)
				=> type.IsDictionary().Should().BeTrue();


			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(object))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeFalse(Type type)
				=> type.IsDictionary().Should().BeFalse();
		}

		public class IsEnumerableTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(IEnumerable<>))]
			[InlineData(typeof(int[]))]
			[InlineData(typeof(IEnumerable<int>))]
			[InlineData(typeof(IDictionary<,>))]
			[InlineData(typeof(Dictionary<,>))]
			[InlineData(typeof(ICollection<>))]
			[InlineData(typeof(IList<>))]
			[InlineData(typeof(List<>))]
			public void ShouldBeTrue(Type type)
				=> type.IsEnumerable().Should().BeTrue();


			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(object))]
			[InlineData(typeof(string))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeFalse(Type type)
				=> type.IsEnumerable().Should().BeFalse();
		}

		public class IsResultTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(Result<,>))]
			[InlineData(typeof(Result<object, object>))]
			[InlineData(typeof(Result<int, int>))]
			public void ShouldBeTrue(Type type)
				=> type.IsResult().Should().BeTrue();


			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(object))]
			[InlineData(typeof(string))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeFalse(Type type)
				=> type.IsResult().Should().BeFalse();
		}

		public class IsNullableTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?))]
			[InlineData(typeof(Nullable<>))]
			[InlineData(typeof(object))]
			public void ShouldBeTrue(Type type)
				=> type.IsNullable().Should().BeTrue();


			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(string))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeFalse(Type type)
				=> type.IsNullable().Should().BeFalse();
		}

		public class GetOpenApiTypeNameTests
		{
			private interface IInterface { }
			private class TestClass1 { public class InnerClass { } }

			[Theory]
			[InlineData(typeof(int?), "int")]
			[InlineData(typeof(object), "object")]
			[InlineData(typeof(int), "int")]
			[InlineData(typeof(decimal), "decimal")]
			[InlineData(typeof(string), "string")]
			[InlineData(typeof(TestClass1), "TestClass1")]
			[InlineData(typeof(IInterface), "IInterface")]
			[InlineData(typeof(TestClass1.InnerClass), "TestClass1+InnerClass")]
			[InlineData(typeof(int[]), "int")]
			public void ShouldBeTrue(Type type, string name)
				=> type.GetOpenApiTypeName().Should().Be(name);
		}

		public class IsComplexObjectTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(Nullable<>))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeTrue(Type type)
				=> type.IsComplexObject().Should().BeTrue();


			[Theory]
			[InlineData(typeof(object))]
			[InlineData(typeof(int))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(string))]
			[InlineData(typeof(Guid))]
			[InlineData(typeof(DateTime))]
			public void ShouldBeFalse(Type type)
				=> type.IsComplexObject().Should().BeFalse();
		}

		public class IsNumericTypeTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int))]
			[InlineData(typeof(byte))]
			[InlineData(typeof(sbyte))]
			[InlineData(typeof(UInt16))]
			[InlineData(typeof(UInt32))]
			[InlineData(typeof(UInt64))]
			[InlineData(typeof(Int16))]
			[InlineData(typeof(Int32))]
			[InlineData(typeof(Int64))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(Decimal))]
			[InlineData(typeof(double))]
			[InlineData(typeof(Double))]
			[InlineData(typeof(float))]
			[InlineData(typeof(Single))]
			public void ShouldBeTrue(Type type)
				=> type.IsNumericType().Should().BeTrue();


			[Theory]
			[InlineData(typeof(object))]
			[InlineData(typeof(string))]
			[InlineData(typeof(Guid))]
			[InlineData(typeof(DateTime))]
			[InlineData(typeof(Nullable<>))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			public void ShouldBeFalse(Type type)
				=> type.IsNumericType().Should().BeFalse();
		}

		public class IsFunctionalOptionTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(Option<>))]
			[InlineData(typeof(Option<int>))]
			[InlineData(typeof(Option<double>))]
			[InlineData(typeof(Option<object>))]
			[InlineData(typeof(Option<IInterface>))]
			[InlineData(typeof(Option<TestClass1>))]
			public void ShouldBeTrue(Type type)
				=> type.IsFunctionalOption().Should().BeTrue();


			[Theory]
			[InlineData(typeof(object))]
			[InlineData(typeof(string))]
			[InlineData(typeof(Guid))]
			[InlineData(typeof(DateTime))]
			[InlineData(typeof(Nullable<>))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(IInterface))]
			[InlineData(typeof(int))]
			[InlineData(typeof(byte))]
			[InlineData(typeof(sbyte))]
			[InlineData(typeof(UInt16))]
			[InlineData(typeof(UInt32))]
			[InlineData(typeof(UInt64))]
			[InlineData(typeof(Int16))]
			[InlineData(typeof(Int32))]
			[InlineData(typeof(Int64))]
			[InlineData(typeof(decimal))]
			[InlineData(typeof(Decimal))]
			[InlineData(typeof(double))]
			[InlineData(typeof(Double))]
			[InlineData(typeof(float))]
			[InlineData(typeof(Single))]
			public void ShouldBeFalse(Type type)
				=> type.IsFunctionalOption().Should().BeFalse();
		}

		public class UnwrapNullableTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?), typeof(int))]
			[InlineData(typeof(Nullable<int>), typeof(int))]
			[InlineData(typeof(object), typeof(object))]
			[InlineData(typeof(int), typeof(int))]
			[InlineData(typeof(IInterface), typeof(IInterface))]
			[InlineData(typeof(TestClass1), typeof(TestClass1))]
			public void ShouldHaveProperType(Type inputType, Type outputType)
				=> inputType.UnwrapNullable().Should().Be(outputType);
		}

		public class UnwrapDefaultTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?), typeof(int))]
			[InlineData(typeof(Nullable<int>), typeof(int))]
			[InlineData(typeof(object), typeof(object))]
			[InlineData(typeof(int), typeof(int))]
			[InlineData(typeof(IInterface), typeof(IInterface))]
			[InlineData(typeof(TestClass1), typeof(TestClass1))]
			[InlineData(typeof(Task<object>), typeof(object))]
			[InlineData(typeof(Task<Data<object>>), typeof(object))]
			[InlineData(typeof(Task<Option<Data<object>>>), typeof(object))]
			[InlineData(typeof(Task<Task<Task<object>>>), typeof(object))]
			public void ShouldHaveSingleOutputType(Type inputType, Type outputType1)
				=> inputType.UnwrapDefault().Should().BeEquivalentTo(outputType1);

			[Theory]
			[InlineData(typeof(Result<object, object>), typeof(object), typeof(object))]
			[InlineData(typeof(Result<Task<object>, Task<object>>), typeof(object), typeof(object))]
			[InlineData(typeof(Result<Task<Option<object>>, Task<Option<object>>>), typeof(object), typeof(object))]
			[InlineData(typeof(Result<Task<Option<object[]>>, Task<Option<object[]>>>), typeof(object), typeof(object))]
			[InlineData(typeof(Result<Task<Option<Data<object[]>>>, Task<Option<Data<object>[]>>>), typeof(object), typeof(object))]
			public void ShouldHaveTwoOutputTypes(Type inputType, Type outputType1, Type outputType2)
				=> inputType.UnwrapDefault().Should().BeEquivalentTo(outputType1, outputType2);
		}

		public class UnwrapTypeTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?))]
			[InlineData(typeof(Nullable<int>))]
			[InlineData(typeof(object))]
			[InlineData(typeof(int))]
			[InlineData(typeof(IInterface))]
			[InlineData(typeof(TestClass1))]
			[InlineData(typeof(Task<object>))]
			[InlineData(typeof(Task<Data<object>>))]
			[InlineData(typeof(Task<Option<Data<object>>>))]
			[InlineData(typeof(Task<Task<Task<object>>>))]
			[InlineData(typeof(Result<object, object>))]
			[InlineData(typeof(Result<Task<object>, Task<object>>))]
			[InlineData(typeof(Result<Task<Option<object>>, Task<Option<object>>>))]
			[InlineData(typeof(Result<Task<Option<object[]>>, Task<Option<object[]>>>))]
			[InlineData(typeof(Result<Task<Option<Data<object[]>>>, Task<Option<Data<object>[]>>>))]
			public void ShouldHaveSingleOutputType(Type inputType)
				=> inputType.UnwrapType(type => type).Should().Be(inputType);
		}

		public class UnwrapValigatorDataTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?), typeof(int))]
			[InlineData(typeof(Nullable<int>), typeof(int))]
			[InlineData(typeof(object), typeof(object))]
			[InlineData(typeof(int), typeof(int))]
			[InlineData(typeof(IInterface), typeof(IInterface))]
			[InlineData(typeof(TestClass1), typeof(TestClass1))]
			[InlineData(typeof(Data<object>), typeof(object))]
			[InlineData(typeof(Data<Data<object>>), typeof(object))]
			[InlineData(typeof(Data<Data<Data<object>>>), typeof(object))]
			[InlineData(typeof(Data<Data<Data<IEnumerable<object>>>>), typeof(IEnumerable<object>))]
			public void ShouldHaveSingleOutputType(Type inputType, Type outputType1)
				=> inputType.UnwrapValigatorData().Should().Be(outputType1);
		}

		public class UnwrapOptionTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?), typeof(int))]
			[InlineData(typeof(Nullable<int>), typeof(int))]
			[InlineData(typeof(object), typeof(object))]
			[InlineData(typeof(int), typeof(int))]
			[InlineData(typeof(IInterface), typeof(IInterface))]
			[InlineData(typeof(TestClass1), typeof(TestClass1))]
			[InlineData(typeof(Option<object>), typeof(object))]
			[InlineData(typeof(Option<Option<object>>), typeof(object))]
			[InlineData(typeof(Option<Option<Option<object>>>), typeof(object))]
			[InlineData(typeof(Option<Option<Option<IEnumerable<object>>>>), typeof(IEnumerable<object>))]
			public void ShouldHaveSingleOutputType(Type inputType, Type outputType1)
				=> inputType.UnwrapOption().Should().Be(outputType1);
		}

		public class UnwrapResultTests
		{
			private interface IInterface { }
			private class TestClass1 { }
			[Theory]
			[InlineData(typeof(Result<object, object>), typeof(object), typeof(object))]
			[InlineData(typeof(Result<Data<object[]>, Task<object>[]>), typeof(Data<object[]>), typeof(Task<object>[]))]
			public void ShouldHaveTwoOutputTypes(Type inputType, Type outputType1, Type outputType2)
				=> inputType.UnwrapDefault().Should().BeEquivalentTo(outputType1, outputType2);
		}

		public class UnwrapTaskTests
		{
			private interface IInterface { }
			private class TestClass1 { }

			[Theory]
			[InlineData(typeof(int?), typeof(int))]
			[InlineData(typeof(Nullable<int>), typeof(int))]
			[InlineData(typeof(object), typeof(object))]
			[InlineData(typeof(int), typeof(int))]
			[InlineData(typeof(IInterface), typeof(IInterface))]
			[InlineData(typeof(TestClass1), typeof(TestClass1))]
			[InlineData(typeof(Task<object>), typeof(object))]
			[InlineData(typeof(Task<Task<object>>), typeof(object))]
			[InlineData(typeof(Task<Task<Task<object>>>), typeof(object))]
			[InlineData(typeof(Task<Task<Task<IEnumerable<object>>>>), typeof(IEnumerable<object>))]
			public void ShouldHaveSingleOutputType(Type inputType, Type outputType1)
				=> inputType.UnwrapOption().Should().Be(outputType1);
		}

		public class GetSuccessTypeTests
		{
			private interface IInterface { }
			private class TestClass1 { }
			[Theory]
			[InlineData(typeof(Result<object, int>), typeof(object))]
			[InlineData(typeof(Result<Data<object[]>, Task<object>[]>), typeof(Data<object[]>))]
			public void ShouldHaveOneOutputTypes(Type inputType, Type outputType1)
				=> inputType.GetSuccessType().Should().Be(outputType1);
		}

		public class GetFailureTypeTests
		{
			private interface IInterface { }
			private class TestClass1 { }
			[Theory]
			[InlineData(typeof(Result<object, int>), typeof(int))]
			[InlineData(typeof(Result<Data<object[]>, Task<object>[]>), typeof(Task<object>[]))]
			public void ShouldHaveOneOutputTypes(Type inputType, Type outputType1)
				=> inputType.GetFailureType().Should().Be(outputType1);
		}
	}
}
