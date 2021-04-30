using Functional;
using System;
using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	[ValigatorModel]
	public class TestGetEndpointResource
	{
		private TestGetEndpointResource() { }

		public TestGetEndpointResource(string name, string imageVersion, string assemblyVersion, string revision, DateTime? buildDate, string appEnvName)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (imageVersion == null)
				throw new ArgumentNullException(nameof(imageVersion));
			if (assemblyVersion == null)
				throw new ArgumentNullException(nameof(assemblyVersion));
			if (revision == null)
				throw new ArgumentNullException(nameof(revision));

			Name = Name.WithValue(name);
			ImageVersion = ImageVersion.WithValue(imageVersion);
			AssemblyVersion = AssemblyVersion.WithValue(assemblyVersion);
			Revision = Revision.WithValue(revision);
			BuildDate = BuildDate.WithValue(buildDate);
			AppEnvName = AppEnvName.WithValue(appEnvName);
		}

		public Data<string> Name { get; private set; } = Data.Required<string>().Length(1);

		public Data<string> ImageVersion { get; private set; } = Data.Required<string>().Length(1);

		public Data<string> AssemblyVersion { get; private set; } = Data.Required<string>().Length(1);

		public Data<string> Revision { get; private set; } = Data.Required<string>().Length(1);

		public Data<Option<DateTime>> BuildDate { get; private set; } = Data.Optional<DateTime>().Nullable();

		public Data<Option<string>> AppEnvName { get; private set; } = Data.Optional<string>().Nullable().Length(100);
	}
}
