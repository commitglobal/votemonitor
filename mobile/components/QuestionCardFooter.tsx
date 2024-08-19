import React, { useMemo } from "react";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { AttachmentMimeType } from "../services/api/get-attachments.api";
import { mapMimeTypeToIcon } from "../helpers/mapMimeTypetoIcon";

interface QuestionCardFooterProps extends XStackProps {
  numberOfNotes: number;
  lastNoteText?: string;
  numberOfAttachments: number;
  attachmentTypes?: AttachmentMimeType[];
}

const QuestionCardFooter: React.FC<QuestionCardFooterProps> = ({
  numberOfNotes,
  lastNoteText,
  numberOfAttachments,
  attachmentTypes,
  ...rest
}) => {
  return (
    <XStack alignItems="center" justifyContent="space-between" width="100%" {...rest}>
      <XStack gap="$xxs" alignItems="center" flex={1}>
        {/* attachments icons */}
        {numberOfAttachments > 0 &&
          attachmentTypes &&
          attachmentTypes.length !== 0 &&
          attachmentTypes.map((attachmentType, index) => {
            const icon = useMemo(() => mapMimeTypeToIcon(attachmentType), [attachmentType]);
            return <Icon key={index} icon={icon} width={20} height={20} />;
          })}

        {/* note icon and text */}
        {numberOfNotes > 0 && lastNoteText && (
          <>
            <Icon icon="note" width={20} height={20} />
            <Typography preset="body1" color="$gray6" numberOfLines={1} flex={0.9}>
              {lastNoteText}
            </Typography>
          </>
        )}
      </XStack>

      <Icon icon="chevronRight" color="$purple5" />
    </XStack>
  );
};

export default QuestionCardFooter;
