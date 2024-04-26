export const performanceLog = async (func: any, funcName = "Unnamed function") => {
  console.log("🚀🚀🚀🚀🚀[PERFORMANCE CHECK STARTED FOR] ", funcName);
  const startTime = performance.now();
  try {
    const data = await func();
    const endTime = performance.now();
    console.log(
      `🚀🚀🚀🚀🚀[PERFORMANCE CHECK ENDED FOR] ${funcName}: took ${
        endTime - startTime
      } milliseconds.`,
    );
    return data;
  } catch (err) {
    console.log(err);
  }
};

// Convert array to object with specified key
// CAUTION: will remove duplicate objects from array if the key matches
type StringKeys<T> = {
  [K in keyof T]: T[K] extends string | number | symbol ? K : never;
}[keyof T];
export const arrayToKeyObject = <
  T extends Record<StringKeys<T>, string | number | symbol>,
  TKeyName extends keyof Record<StringKeys<T>, string | number | symbol>,
>(
  array: T[],
  key: TKeyName,
): Record<T[TKeyName], T> =>
  Object.fromEntries(array.map((a) => [a[key], a])) as Record<T[TKeyName], T>;
