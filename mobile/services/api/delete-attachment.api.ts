import API from "../api";

/** ========================================================================
    ================= DELETE deleteAttachment ====================
    ========================================================================
    @description delete an attachment 
    @param {string} electionRoundId 
    @param {string} id 
*/

type DeleteAttachmentAPIPayload = {
  electionRoundId: string;
  id: string;
};

export const deleteAttachment = ({ electionRoundId, id }: DeleteAttachmentAPIPayload) => {
  return API.delete(`election-rounds/${electionRoundId}/attachments/${id}`).then((res) => res.data);
};
