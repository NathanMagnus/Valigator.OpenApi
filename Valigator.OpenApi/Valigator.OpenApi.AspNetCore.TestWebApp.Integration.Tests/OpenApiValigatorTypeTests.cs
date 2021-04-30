//using FluentAssertions;
//using Functional;
//using System;
//using System.Reflection;
//using Validator.OpenApi.Integration.Tests.TestResources;
//using Valigator;
//using Xunit;

//namespace Validator.OpenApi.Integration.Tests
//{
//	public class OpenApiValigatorTypeTests
//	{
//		[Theory]
//		[InlineData(typeof(int))]
//		[InlineData(typeof(CycleA))]
//		[InlineData(typeof(CycleB))]
//		[InlineData(typeof(CycleC))]
//		[InlineData(typeof(GuidIdentifier))]
//		[InlineData(typeof(ExampleExternalClass))]
//		[InlineData(typeof(ExamplePostResource))]
//		[InlineData(typeof(ObjectWithOneProperty))]
//		[InlineData(typeof(TestGetEndpointResource))]
//		[InlineData(typeof(UnexpectedError))]
//		public void GetTypeInfoWorks(Type type)
//		{
//			var openApiValigatorType = new OpenApiValigatorType(type, Array.Empty<PropertyInfo>());

//			var typeInfo = openApiValigatorType.GetTypeInfo();
//			typeInfo.Should().NotBeNull();
//		}

//		[Fact]
//		public void GetTypeInfoWorksOnAnonymousObject()
//		{
//			var type = new { Item1 = 1, Item2 = ((Data<int>)Data.Required<int>()).WithValue(22) }.GetType();
//			var openApiValigatorType = new OpenApiValigatorType(type, Array.Empty<PropertyInfo>());

//			var typeInfo = openApiValigatorType.GetTypeInfo();
//			typeInfo.Should().NotBeNull();
//		}
//	}
//}
