using Functional;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Valigator.OpenApi.AspNetCore.TestWebApp.Controllers
{
	[Route("Test")]
	public class TestController : Controller
	{
		[HttpGet]
		public TestGetMethodObject TestGetMethod()
			=> throw new NotImplementedException();
	}

	[ValigatorModel]
	public class TestGetMethodObject
	{
		public Data<int> Range1To3 { get; private set; } = Data.Required<int>().InRange(greaterThan: 1, lessThan: 3);
		public Data<MappedValue> MappedFromInt { get; private set; } = Data.Required<MappedValue>().MappedFrom<int>(i => new MappedValue(), s => s.GreaterThan(22));
		public Data<Option<int>> Optional { get; private set; } = Data.Optional<int>();
		public Data<Option<int>> Nullable { get; private set; } = Data.Required<int>().Nullable();
		public Data<int> Defaulted { get; private set; } = Data.Defaulted<int>(55);
		public Data<string> StringLength { get; private set; } = Data.Required<string>().Length(1, 200);
		public Data<int> InSet { get; private set; } = Data.Required<int>().InSet(1, 2, 3, 5, 8, 13);
		public Data<int[]> ItemCount { get; private set; } = Data.Collection<int>().Required().ItemCount(100);
		public Data<int> MultipleOf { get; private set; } = Data.Required<int>().MultipleOf(3);
		public Data<decimal> Precision { get; private set; } = Data.Required<decimal>().Precision(1, 3);
		public Data<DiscriminatedObjectBase> DiscriminatedObject { get; private set; } = Data.Required<DiscriminatedObjectBase>();
	}

	public enum DiscriminatorEnum
	{
		Object1,
		ObjectTwo
	}

	[ValigatorModel]
	public class DiscriminatedObjectBase
	{
		public Data<DiscriminatorEnum> Discriminator { get; private set; } = Data.Required<DiscriminatorEnum>();

	}

	[ValigatorModel]
	public class DiscriminatedObject1 : DiscriminatedObjectBase
	{
		public Data<int> Object1Property { get; private set; } = Data.Required<int>();
	}

	[ValigatorModel]
	public class DiscriminatedObjectTwo : DiscriminatedObjectBase
	{
		public Data<int> ObjectTwoProperty { get; private set; } = Data.Required<int>();
	}

	public class MappedValue : Object { }
}
