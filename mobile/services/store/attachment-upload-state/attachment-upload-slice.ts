export enum AttachmentProgressStatusEnum {
  IDLE = "idle",
  COMPRESSING = "compressing",
  STARTING = "starting",
  INPROGRESS = "inprogress",
  ABORTED = "aborted",
  COMPLETED = "completed",
}

export interface IAttachmentProgressState {
  progress: number;
  status: AttachmentProgressStatusEnum;
}

export const attachmentUploadProgressSlice = (set: any) => ({
  progresses: {},
  setProgresses: (
    fn: (
      prev: Record<string, IAttachmentProgressState>,
    ) => Record<string, IAttachmentProgressState>,
  ) => {
    set((state: any) => ({ progresses: fn(state.progresses) }));
  },
});

export default { attachmentUploadProgressSlice };
