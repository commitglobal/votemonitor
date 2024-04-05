import { Progress } from "tamagui";

export function ProgressBar() {
  const progress = 20;
  return (
    <Progress value={progress}>
      <Progress.Indicator animation="bouncy" />
    </Progress>
  );
}
