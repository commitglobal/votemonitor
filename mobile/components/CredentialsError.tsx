import { XStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

interface CredentialsErrorProps {
  error: string;
}

const CredentialsError = (props: CredentialsErrorProps) => {
  const { error } = props;

  return (
    <XStack backgroundColor="$red1" borderRadius={6} padding="$md" alignItems="flex-start" gap={8}>
      <Icon icon="loginError" size={20} color="blue" />
      <Typography color="$red6" fontWeight="500" flexGrow={1} flexShrink={1}>
        {error}
      </Typography>
    </XStack>
  );
};

export default CredentialsError;
