import { Progress } from "tamagui";

export interface ProgressProps {
  total: number;
  current: number;
}

export function LinearProgress(props: ProgressProps) {
  const { total, current } = props;
  const progress = (current / total) * 100;

  return (
    <Progress value={progress} height={4} borderRadius={8} backgroundColor="$yellow3">
      <Progress.Indicator backgroundColor="$yellow4" />
    </Progress>
  );
}
