import { ScrollView, XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { Dialog } from "./Dialog";
import Button from "./Button";

interface InfoModalProps {
  handleCloseInfoModal: () => void;
  paragraphs: string[];
}

const InfoModal = ({ handleCloseInfoModal, paragraphs }: InfoModalProps) => {
  const { t } = useTranslation("common");
  return (
    <Dialog
      open
      content={
        <YStack maxHeight="85%" gap="$md">
          <ScrollView
            contentContainerStyle={{ gap: 16, flexGrow: 1 }}
            showsVerticalScrollIndicator={false}
            bounces={false}
          >
            {paragraphs.map((p, index) => (
              <Typography key={index} color="$gray6">
                {p}
              </Typography>
            ))}
          </ScrollView>
        </YStack>
      }
      footer={
        <XStack justifyContent="center">
          <Button preset="chromeless" onPress={handleCloseInfoModal}>
            {t("ok")}
          </Button>
        </XStack>
      }
    />
  );
};

export default InfoModal;