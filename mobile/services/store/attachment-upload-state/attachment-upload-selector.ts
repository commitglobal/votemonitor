import useStore from "../store";
import { IAttachmentProgressState } from "./attachment-upload-slice";

export const useAttachmentUploadProgressState = () => {
  const progresses: Record<string, IAttachmentProgressState> = useStore(
    (state) => state.progresses,
  );
  return { progresses };
};
