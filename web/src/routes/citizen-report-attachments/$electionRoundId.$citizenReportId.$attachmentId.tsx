import { authApi } from '@/common/auth-api';
import { Attachment } from '@/features/responses/models/common';
import { getFileCategory } from '@/lib/utils';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import { useMemo } from 'react';
import ReactPlayer from 'react-player';

export const citizenReportAttachmentQueryOptions = (
  electionRoundId: string,
  citizenReportId: string,
  attachmentId: string
) => {
  return queryOptions({
    queryKey: ['citizen-reports-attachments', electionRoundId, attachmentId],
    queryFn: async () => {
      const response = await authApi.get<Attachment>(
        `/api/election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}/attachments/${attachmentId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch citizen report attachment');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

export const Route = createFileRoute('/citizen-report-attachments/$electionRoundId/$citizenReportId/$attachmentId')({
  component: AttachmentDetails,
  loader: ({ context: { queryClient }, params: { electionRoundId, citizenReportId, attachmentId } }) =>
    queryClient.ensureQueryData(citizenReportAttachmentQueryOptions(electionRoundId, citizenReportId, attachmentId)),
});

function AttachmentDetails() {
  const { electionRoundId, citizenReportId, attachmentId } = Route.useParams();

  const { data: attachment } = useSuspenseQuery(
    citizenReportAttachmentQueryOptions(electionRoundId, citizenReportId, attachmentId)
  );

  const attachmentType = useMemo(() => {
    return getFileCategory(attachment.mimeType);
  }, [attachment.mimeType]);

  return (
    <div className='flex justify-center'>
      {attachmentType === 'image' ? (
        <img alt={attachment.fileName} src={attachment.presignedUrl} />
      ) : (
        <ReactPlayer alt={attachment.fileName} url={attachment.presignedUrl} controls={true} />
      )}
    </div>
  );
}
