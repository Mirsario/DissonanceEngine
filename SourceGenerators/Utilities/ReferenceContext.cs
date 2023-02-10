using System.Collections.Generic;

namespace SourceGenerators.Utilities;

public struct ReferenceContext
{
	public HashSet<string>? ImportedNamespaces;

	public bool OmitNamespace(string @namespace)
		=> ImportedNamespaces != null && ImportedNamespaces.Contains(@namespace);
}
