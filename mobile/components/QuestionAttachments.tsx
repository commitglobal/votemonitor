import { YStack } from "tamagui";
import { Typography } from "./Typography";
import Card from "./Card";
import { Icon } from "./Icon";
import { useAttachments } from "../services/queries/attachments.query";

interface QuestionAttachmentsProps {
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
}

const QuestionAttachments: React.FC<QuestionAttachmentsProps> = ({
  electionRoundId,
  pollingStationId,
  formId,
  questionId,
}) => {
  const { data: attachments } = useAttachments(electionRoundId, pollingStationId, formId);

  return (
    attachments?.[questionId]?.length && (
      <YStack marginTop="$lg" gap="$xxs">
        <Typography fontWeight="500">Uploaded media</Typography>
        <YStack gap="$xxs">
          {attachments[questionId]?.map((attachment) => {
            return (
              <Card
                padding="$0"
                paddingLeft="$md"
                key={attachment.id}
                flexDirection="row"
                justifyContent="space-between"
                alignItems="center"
              >
                <Typography preset="body1" fontWeight="700" maxWidth="85%" numberOfLines={1}>
                  {attachment.fileName}
                </Typography>
                <Icon
                  icon="xCircle"
                  size={24}
                  color="$gray5"
                  onPress={() => console.log("delete media action")}
                  pressStyle={{ opacity: 0.5 }}
                  padding="$md"
                />
              </Card>
            );
          })}
        </YStack>
      </YStack>
    )
  );
};

export default QuestionAttachments;
