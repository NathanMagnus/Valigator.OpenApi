using Functional;
using NSwag.Generation.AspNetCore;
using Valigator.Core;
using Valigator.OpenApi.AspNetCore.Extensions;
using Valigator.OpenApi.AspNetCore.TypeMapping;

namespace Valigator.OpenApi.AspNetCore.Extensions
{
	internal static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
	{
		public static void AddAdditionalTypeMappings(this AspNetCoreOpenApiDocumentGeneratorSettings settings, DataDescriptor dataDescriptor)
			=> dataDescriptor
				.MappingDescriptor
				.Do(some => settings.AddCustomTypeMapping(some.SourceType, dataDescriptor.PropertyType));
	}
}
