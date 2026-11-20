// resolve hook: a relative import without an extension is tried as .ts, .tsx and index.ts
const CANDIDATES = [".ts", ".tsx", "/index.ts", "/index.tsx"];

export async function resolve(specifier, context, nextResolve) {
  try {
    return await nextResolve(specifier, context);
  } catch (error) {
    const isRelative = specifier.startsWith("./") || specifier.startsWith("../");
    if (!isRelative || /\.[cm]?[jt]sx?$/.test(specifier)) throw error;
    for (const suffix of CANDIDATES) {
      try {
        return await nextResolve(specifier + suffix, context);
      } catch {
        // try the next one
      }
    }
    throw error;
  }
}
