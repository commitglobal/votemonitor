using System.Runtime.Serialization;

namespace Vote.Monitor.Core.Services.FileStorage;

public enum FileStorageType
{
    [EnumMember(Value = "LocalDisk")]
    LocalDisk,

    [EnumMember(Value = "S3")]
    S3,
    
    [EnumMember(Value = "MiniIO")]
    MiniIO
}
