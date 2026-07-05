# NMimeType

Fast MIME type lookup for .NET applications.

The package resolves MIME types from file names, paths, or extensions using an embedded lookup table. It keeps lookups case-insensitive and indexes the table in memory so repeated reads are fast.

## Why this package matters

MIME type detection shows up in file uploads, downloads, API responses, static file serving, email attachments, background processing, and storage pipelines. Getting it wrong can cause browsers to render files incorrectly, APIs to return poor metadata, or upload systems to accept files without enough classification.

This package keeps that lookup simple:

- No operating system dependency for the MIME table.
- No repeated JSON scanning during normal lookups.
- Case-insensitive extension matching.
- Works with file names, full paths, or raw extensions.
- Supports custom embedded MIME resources for product-specific file types.
- Targets older compatible libraries and current .NET applications.

## Supported frameworks

- `netstandard2.0`
- `net8.0`
- `net9.0`
- `net10.0`

`net8.0`, `net9.0`, and `net10.0` use the built-in `System.Text.Json`. The `netstandard2.0` target references the Microsoft `System.Text.Json` package.

## Features

- Fast indexed lookups after the embedded table is loaded.
- Built-in MIME table with common document, image, audio, video, archive, web, app package, and source file extensions.
- Modern extensions such as `avif`, `heic`, `wasm`, `webmanifest`, `zst`, `br`, `yaml`, `toml`, `ndjson`, and `geojson`.
- `Get` for simple lookups.
- `TryGet` for unknown-safe lookups.
- `GetOrDefault` for fallback MIME values.
- `Contains` to check whether an extension is known.
- `GetKnownTypes` to inspect the loaded table.
- Custom embedded JSON resources with override support.

## Basic usage

```csharp
using MimeType;

string mimeType = Mime.Get("report.pdf");
// application/pdf

string fromExtension = Mime.Get(".json");
// application/json

string fromPath = Mime.Get("/uploads/avatar.webp");
// image/webp
```

## Fallback values

Use `GetOrDefault` when your pipeline needs a real MIME type even for unknown extensions.

```csharp
string mimeType = Mime.GetOrDefault("upload.unknown");
// application/octet-stream

string customFallback = Mime.GetOrDefault("upload.unknown", "application/x-custom-fallback");
// application/x-custom-fallback
```

## TryGet and Contains

Use `TryGet` when an extension may be unknown and you want to avoid treating an empty string as a result.

```csharp
if (Mime.TryGet("video.webm", out var mimeType))
{
    Console.WriteLine(mimeType);
}

bool supported = Mime.Contains("archive.zst");
```

## Instance API

```csharp
IMime mime = new Mime();

string value = mime["document.docx"];
string sameValue = mime.Get("document.docx");
```

## List known types

```csharp
IReadOnlyDictionary<string, string> knownTypes = Mime.GetKnownTypes();

foreach (var item in knownTypes)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}
```

## Custom MIME resources

You can add your own embedded JSON resources. Custom resources override the built-in table when the same extension appears in both.

Example JSON:

```json
{
  "custom": "application/x-custom",
  "pdf": "application/pdf"
}
```

Marker type:

```csharp
using MimeType;

[MimeTypeResourceName("Content")]
public class MyMimeTypes
{
}
```

Register the resource:

```csharp
Mime.AddMimeResources(typeof(MyMimeTypes));
```

The JSON file should be embedded in the same assembly as the marker type. The attribute value should match the namespace or folder segment used in the embedded resource name.

## Build

```bash
dotnet build MimeType.sln
```

## Release package

The repository includes a GitHub Actions workflow that creates and publishes the NuGet package when a GitHub Release is published.

Before publishing a release, add this repository secret in GitHub:

- `NUGET_API_KEY`: an API key from nuget.org with permission to push this package.

Release flow:

1. Create a tag such as `v1.0.0`.
2. Create and publish a GitHub Release for that tag.
3. The workflow builds the solution, packs `NMimeType`, uploads the `.nupkg` as a workflow artifact, and pushes it to nuget.org.

The package version is taken from the release tag. For example, `v1.0.0` publishes package version `1.0.0`.

## Notes

- Extension lookup is case-insensitive.
- File names and paths are supported; the last extension is used.
- Unknown extensions return `string.Empty` from `Mime.Get`.
- Prefer `Mime.TryGet` or `Mime.GetOrDefault` when unknown extensions are expected.
