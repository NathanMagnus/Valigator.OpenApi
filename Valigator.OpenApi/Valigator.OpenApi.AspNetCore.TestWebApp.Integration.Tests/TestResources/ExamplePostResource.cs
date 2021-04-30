using Functional;
using System;
using System.Collections.Generic;
using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	[ValigatorModel]
	public class ExamplePostResource
	{
		public Data<string> Str { get; set; } = Data.Required<string>();
		public Data<int[]> LessThanInclusive { get; set; } = Data.Collection<int>(s => s.LessThanOrEqualTo(100)).Required().ItemCount(2);
		public Data<Option<int[]>> LessThanExclusive { get; set; } = Data.Collection<int>(s => s.LessThan(200)).Optional();
		public Data<int[]> GreaterThanInclusive { get; set; } = Data.Collection<int>(s => s.GreaterThanOrEqualTo(500)).Defaulted(new[] { 1, 2 }).ItemCount(300);
		public Data<ExampleNestedClass> ExampleNested { get; set; } = Data.Required<ExampleNestedClass>();
		public Data<int[]> GreaterThanExclusive { get; set; } = Data.Collection<int>(s => s.GreaterThan(400)).Required();
		public Data<int[]> RangeMixed { get; set; } = Data.Collection<int>(s => s.InRange(lessThan: 100, greaterThanOrEqualTo: 5)).Required();
		public Data<int[]> InSet { get; set; } = Data.Collection<int>(s => s.InSet(1, 3, 5, 7, 9)).Required();
		public Data<int[]> Not0 { get; set; } = Data.Collection<int>(s => s.NotZero()).Required();
		public Data<DateTime> Date { get; set; } = Data.Required<DateTime>().InRange(DateTime.Parse("1990-01-22"));
		public Data<decimal> Money { get; set; } = Data.Required<decimal>().GreaterThan(0);
		public Data<int> Int { get; set; } = Data.Required<int>().InRange(greaterThanOrEqualTo: 1);
		public Data<int> Int2 { get; set; } = Data.Required<int>().InRange(lessThan: 100);
		public Data<Guid> Guid { get; set; } = Data.Required<Guid>();
		public Data<IEnumerable<ExampleNestedClass>> ExampleNestedArray { get; set; } = Data.Required<IEnumerable<ExampleNestedClass>>();
		public Data<CycleA> CycleA { get; set; } = Data.Required<CycleA>();
		public Data<ExampleEnum> ExampleEnumValue { get; set; } = Data.Defaulted(ExampleEnum.Value2);
		public Data<GuidIdentifier> CartIdentifier { get; set; } = Data.Required<GuidIdentifier>();

		public enum ExampleEnum
		{
			Value1,
			Value2
		}

		public class ExampleNestedClass
		{
			public Data<int> Int { get; set; } = Data.Required<int>();
			public Data<ExampleNestedClass> Self { get; set; } = Data.Required<ExampleNestedClass>();
		}
	}
}
