import { create } from "zustand";
import {
  AttachmentProgressStatusEnum,
  IAttachmentProgressState,
  attachmentUploadProgressSlice,
} from "./attachment-upload-state/attachment-upload-slice";

interface AttchmentUploadProgressState {
  progresses: {};
  setProgresses: (
    fn: (prev: Record<string, IAttachmentProgressState>) => Record<string, any>,
  ) => void;
}

const useStore = create<AttchmentUploadProgressState>()((set: any) => ({
  ...attachmentUploadProgressSlice(set),
}));

export default useStore;
