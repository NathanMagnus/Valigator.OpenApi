using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;
using Valigator.OpenApi.Core.Extensions;
using static Valigator.OpenApi.Core.Extensions.DiscriminatorMappingExtensions;

namespace Valigator.OpenApi.Core.Discriminators
{
	internal class DiscriminatorCleanupDocumentProcessor : IDocumentProcessor
	{
		public DiscriminatorMappings DiscriminatorMappings { get; }

		public DiscriminatorCleanupDocumentProcessor(DiscriminatorMappings discriminatorMappings)
			=> DiscriminatorMappings = discriminatorMappings;

		public void Process(DocumentProcessorContext context)
			=> SetDiscriminatorMappings(context);

		private void SetDiscriminatorMappings(DocumentProcessorContext context)
			=> context
				.Document
				.Definitions
				.Where(definition => definition.Value.DiscriminatorObject != null)
				.Do(definition =>
				{
					var add = definition
						.Value
						.DiscriminatorObject
						.Mapping
						.Where(mapping => mapping.Value is CustomJsonSchema)
						.ToArray();

					definition
						.Value
						.DiscriminatorObject
						.Mapping
						.Clear();

					add.Do(item => definition
						.Value
						.DiscriminatorObject
						.Mapping
						.Add(item.Key, item.Value)
					);
				});
	}
}
