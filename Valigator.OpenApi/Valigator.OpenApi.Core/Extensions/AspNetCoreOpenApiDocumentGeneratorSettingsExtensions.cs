using Functional;
using NSwag.Generation.AspNetCore;
using Valigator.Core;
using Valigator.OpenApi.Core.Extensions;
using Valigator.OpenApi.Core.TypeMapping;

namespace Valigator.OpenApi.Core.Extensions
{
	internal static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
	{
		public static void AddAdditionalTypeMappings(this AspNetCoreOpenApiDocumentGeneratorSettings settings, DataDescriptor dataDescriptor)
			=> dataDescriptor
				.MappingDescriptor
				.Do(some => settings.AddCustomTypeMapping(some.SourceType, dataDescriptor.PropertyType));
	}
}
