import { Screen } from "../../../../../../components/Screen";
import { router, useLocalSearchParams } from "expo-router";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { Typography } from "../../../../../../components/Typography";
import { YStack, Image, View, XStack, AlertDialog, AlertDialogProps } from "tamagui";
import { useQuickReportById } from "../../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import React, { ReactNode } from "react";
import Card from "../../../../../../components/Card";
import { QuickReportAttachmentAPIResponse } from "../../../../../../services/api/quick-report/get-quick-reports.api";

type SearchParamsType = {
  reportId: string;
  reportTitle: string;
};

const ReportDetails = () => {
  const { reportTitle, reportId } = useLocalSearchParams<SearchParamsType>();
  const { t } = useTranslation(["report_details", "common"]);

  if (!reportId || !reportTitle) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound } = useUserData();

  const {
    data: quickReport,
    isLoading: isLoadingCurrentReport,
    error: currentReportError,
  } = useQuickReportById(activeElectionRound?.id, reportId);

  if (isLoadingCurrentReport) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentReportError) {
    return (
      <Screen preset="fixed">
        <Header
          title={`${reportTitle}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack paddingVertical="$xxl" alignItems="center">
          <Typography>{t("error")}</Typography>
        </YStack>
      </Screen>
    );
  }

  const attachments = quickReport?.attachments || [];
  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
      backgroundColor="white"
    >
      <Header
        title={`${reportTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack gap={32} paddingHorizontal={16} paddingTop={32} justifyContent="center">
        <YStack gap={16}>
          <Typography preset="subheading" fontWeight="500">
            {quickReport?.title}
          </Typography>
          <Typography preset="body1" lineHeight={24} color="$gray8">
            {quickReport?.description}
          </Typography>
        </YStack>

        {attachments.length === 0 ? (
          <Typography fontWeight="500">{t("no_files")}</Typography>
        ) : (
          <YStack gap={16}>
            <Typography fontWeight="500" color="$gray10">
              {t("uploaded_media")}
            </Typography>
            {attachments.map((attachment, key) => (
              <MediaPreview key={key} attachment={attachment} />
            ))}
          </YStack>
        )}
      </YStack>
    </Screen>
  );
};

interface attachementProps {
  attachment: QuickReportAttachmentAPIResponse;
}

const MediaPreview = (props: attachementProps) => {
  const { attachment } = props;
  return (
    <MediaDialog
      trigger={
        <Card>
          <Typography preset="body1" fontWeight="700" key={attachment.id}>
            {attachment.fileName}
          </Typography>
        </Card>
      }
      header={
        <XStack justifyContent="space-between" backgroundColor="white" height="5%">
          <Typography>{attachment.fileName}</Typography>
          <AlertDialog.Cancel>
            <Icon icon="x" />
          </AlertDialog.Cancel>
        </XStack>
      }
      content={
        attachment.mimeType.includes("image") ? (
          <Image
            source={{ uri: attachment.presignedUrl }}
            width="100%"
            height="90%"
            resizeMode="cover"
          />
        ) : (
          <View>
            <Typography>Not an image</Typography>
          </View>
        )
      }
    />
  );
};

/**
 * This is similiar to Dialog component from /components,
 * but with some modifications to fit the requirements of the ImagePreview component
 * TODO: Move it to /components, in case is good for use.
 */

interface DialogProps extends AlertDialogProps {
  // what you press on in order to open the dialog
  trigger?: ReactNode;
  // dialog header
  header?: ReactNode;
  // content inside dialog
  content?: ReactNode;
}

export const MediaDialog: React.FC<DialogProps> = ({ header, content, trigger, ...props }) => {
  return (
    <AlertDialog {...props}>
      {trigger && <AlertDialog.Trigger asChild>{trigger}</AlertDialog.Trigger>}
      <AlertDialog.Portal>
        {/* backdrop for the modal */}
        <AlertDialog.Overlay
          key="overlay"
          animation="quick"
          opacity={1}
          enterStyle={{ opacity: 0 }}
          exitStyle={{ opacity: 0 }}
        />
        {/* the actual content inside the modal */}
        <AlertDialog.Content
          backgroundColor="white"
          style={{ padding: 0 }}
          width="90%"
          maxHeight="70%"
          elevate
          key="content"
          animation={[
            "quick",
            {
              opacity: {
                overshootClamping: true,
              },
            },
          ]}
          enterStyle={{ x: 0, y: -20, opacity: 0, scale: 0.9 }}
          exitStyle={{ x: 0, y: 10, opacity: 0, scale: 0.95 }}
          x={0}
          scale={1}
          opacity={1}
          y={0}
          gap="$md"
        >
          {header}
          {content}
        </AlertDialog.Content>
      </AlertDialog.Portal>
    </AlertDialog>
  );
};

export default ReportDetails;
