import { memo } from "react";
import { Progress, styled } from "tamagui";

export interface ProgressProps {
  total: number;
  current: number;
}

const LinearProgress = ({ total, current }: ProgressProps) => {
  const progress = (current / total) * 100;

  const StyledProgress = styled(Progress, {
    height: 4,
    borderRadius: 8,
    backgroundColor: "$yellow4",
  });

  return (
    <StyledProgress value={progress}>
      <Progress.Indicator backgroundColor="$yellow5" />
    </StyledProgress>
  );
};

export default memo(LinearProgress);
