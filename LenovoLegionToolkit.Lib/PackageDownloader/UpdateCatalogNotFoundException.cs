using System;

namespace LenovoLegionToolkit.Lib.PackageDownloader;

public class UpdateCatalogNotFoundException(string? message, Exception? ex) : Exception(message, ex);
