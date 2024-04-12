import AsyncStorage from "@react-native-async-storage/async-storage";
import { Buffer } from "buffer";

function byteLength(obj: any, type?: any) {
  return type === "JSON" ? Buffer.byteLength(JSON.stringify(obj)) : Buffer.byteLength(obj);
}

function formatBytes(bytes: any, decimals = 2) {
  if (!+bytes) return "0 Bytes";

  const k = 1024;
  const dm = decimals < 0 ? 0 : decimals;
  const sizes = ["Bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"];

  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
}

export async function clearStorage() {
  await AsyncStorage.clear();
}

export async function calculateSizes() {
  const keys = await AsyncStorage.getAllKeys();
  console.info("Total keys in AsyncStorage:", keys.length);

  let totalSizeInBytes = 0;

  for (const key of keys) {
    try {
      const item = await AsyncStorage.getItem(key);
      if (item) {
        let sizeInBytes = 0;
        try {
          sizeInBytes = byteLength(JSON.parse(item), "JSON");
        } catch (err) {
          // Not json
          sizeInBytes = byteLength(item);
        }
        console.info(`${key} (${formatBytes(sizeInBytes)})`);
        totalSizeInBytes += sizeInBytes;
      }
    } catch (error: any) {
      console.error(`Error reading '${key}' from AsyncStorage: ${error.message}`);
    }
  }

  console.info(`Total size of all keys: ${formatBytes(totalSizeInBytes)}`);
}
