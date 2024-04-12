import { FormItemStatus, FormListItem } from "../app/(app)/(drawer)/(tabs)";

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

// Function to generate a random status
function getRandomStatus(): FormItemStatus {
  const statuses = ["not started", "in progress", "completed"];
  const randomIndex = Math.floor(Math.random() * statuses.length);
  return statuses[randomIndex] as FormItemStatus;
}

// Generate an array of 25 elements
export const formList: FormListItem[] = Array.from({ length: 25 }, (_, index) => ({
  id: `id_${index + 1}`,
  name: `Form ${index + 1}`,
  options: `Option ${index + 1}`,
  numberOfQuestions: Math.floor(Math.random() * 10) + 1,
  numberOfCompletedQuestions: Math.floor(Math.random() * 10),
  status: getRandomStatus(),
}));
