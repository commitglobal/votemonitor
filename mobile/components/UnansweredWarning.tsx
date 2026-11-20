import { useTranslation } from "react-i18next";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

/** A dark amber triangle: plain yellow on white is too faint to read. */
export const WarningMark = ({ size = 20 }: { size?: number }) => (
  <Icon icon="warning" color="$yellow7" size={size} flexShrink={0} />
);

interface UnansweredWarningProps extends Omit<XStackProps, "onPress"> {
  /** how many questions are not answered: shown as "Unanswered questions: 3" */
  count?: number;
  /** a text of its own instead of the count (e.g. "Not answered" on a question) */
  label?: string;
  /** leads to the first unanswered question */
  onPress?: () => void;
}

/**
 * The warning for unanswered questions, on questions and on submissions. Always a mark with a text,
 * so it doesn't rely on color alone; pressing it leads to the first unanswered question.
 */
const UnansweredWarning = ({ count, label, onPress, ...rest }: UnansweredWarningProps) => {
  const { t } = useTranslation("common");
  const text = label ?? t("unanswered.count", { count });

  return (
    <XStack
      alignItems="center"
      gap="$xxs"
      accessible
      accessibilityRole={onPress ? "button" : "text"}
      accessibilityLabel={text}
      onPress={onPress}
      hitSlop={onPress ? 8 : undefined}
      pressStyle={onPress ? { opacity: 0.6 } : undefined}
      {...rest}
    >
      <WarningMark />
      <Typography preset="body2" color="$gray9" flexShrink={1}>
        {text}
      </Typography>
    </XStack>
  );
};

export default UnansweredWarning;
