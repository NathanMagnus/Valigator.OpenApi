using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.Discriminators
{
	/// <summary>
	/// Mulitple Discriminator Mappings.
	/// </summary>
	public class DiscriminatorMappings : IReadOnlyDictionary<Type, DiscriminatorMapping>
	{
		private readonly IDictionary<Type, DiscriminatorMapping> _dictionary = new Dictionary<Type, DiscriminatorMapping>();
		public DiscriminatorMappings(params (Type Type, DiscriminatorMapping Mapping)[] mappings)
			=> mappings.Do(mapping => _dictionary.Add(mapping.Type, mapping.Mapping));

		public DiscriminatorMapping this[Type key] => _dictionary[key];
		public IEnumerable<Type> Keys => _dictionary.Keys;
		public IEnumerable<DiscriminatorMapping> Values => _dictionary.Values;
		public int Count => _dictionary.Count;
		public bool ContainsKey(Type key) => _dictionary.ContainsKey(key);
		public IEnumerator<KeyValuePair<Type, DiscriminatorMapping>> GetEnumerator() => _dictionary.GetEnumerator();
		public bool TryGetValue(Type key, [MaybeNullWhen(false)] out DiscriminatorMapping value) => _dictionary.TryGetValue(key, out value);
		IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
	}
}