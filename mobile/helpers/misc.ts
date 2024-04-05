export const performanceLog = async (
  func: any,
  funcName = "Unnamed function"
) => {
  console.log("🚀🚀🚀🚀🚀[PERFORMANCE CHECK STARTED FOR] ", funcName);
  const startTime = performance.now();
  try {
    const data = await func();
    const endTime = performance.now();
    console.log(
      `🚀🚀🚀🚀🚀[PERFORMANCE CHECK ENDED FOR] ${funcName}: took ${
        endTime - startTime
      } milliseconds.`
    );
    return data;
  } catch (err) {
    console.log(err);
  }
};
