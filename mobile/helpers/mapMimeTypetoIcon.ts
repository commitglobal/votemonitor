import { AttachmentMimeType } from "../services/api/get-attachments.api";

export const mapMimeTypeToIcon = (attachmentType: AttachmentMimeType): string => {
  switch (attachmentType) {
    case AttachmentMimeType.IMG:
      return "photo";
    case AttachmentMimeType.VIDEO:
      return "video";
    case AttachmentMimeType.AUDIO_M4A:
    case AttachmentMimeType.AUDIO_MPEG:
      return "audio";
    default:
      return "photo";
  }
};
