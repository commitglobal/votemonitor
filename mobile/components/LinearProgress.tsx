import { Progress, styled } from "tamagui";

export interface ProgressProps {
  total: number;
  current: number;
}

const LinearProgress = (props: ProgressProps) => {
  const { total, current } = props;
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

export default LinearProgress;
