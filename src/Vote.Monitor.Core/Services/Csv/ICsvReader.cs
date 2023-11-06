﻿using CsvHelper.Configuration;

namespace Vote.Monitor.Core.Services.Csv;

public interface ICsvReader<T>
{
    IEnumerable<T> Read<TMap>(Stream stream) where TMap : ClassMap<T>;
    IEnumerable<T> Read(Stream stream);
}
