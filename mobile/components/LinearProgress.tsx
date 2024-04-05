import { Progress } from "tamagui";

export function LinearProgress() {
  const progress = 10;

  return (
    <Progress value={progress} height={4} borderRadius={8} backgroundColor="$yellow3">
      <Progress.Indicator backgroundColor="$yellow4" />
    </Progress>
  );
}
