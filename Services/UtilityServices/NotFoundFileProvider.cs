using System.Collections;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace SimpLN.Services.UtilityServices;

public class NotFoundFileProvider : IFileProvider
{
	public IDirectoryContents GetDirectoryContents(string subpath) => new NotFoundDirectoryContents();
	public IFileInfo GetFileInfo(string subpath) => new NotFoundFileInfo();
	public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
}

public class NotFoundFileInfo : IFileInfo
{
	public bool Exists => false;
	public long Length => 0;
	public string PhysicalPath => null;
	public string Name => null;
	public DateTimeOffset LastModified => DateTimeOffset.MinValue;
	public bool IsDirectory => false;
	public Stream CreateReadStream() => Stream.Null;
}

public class NotFoundDirectoryContents : IDirectoryContents
{
	public bool Exists => false;
	public IEnumerator<IFileInfo> GetEnumerator() => Enumerable.Empty<IFileInfo>().GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
