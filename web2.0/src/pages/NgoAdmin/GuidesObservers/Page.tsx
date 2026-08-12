import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { ElectionRoundStatus } from '@/types/election'
import { H1, P } from '@/components/ui/typography'
import { GuidesDialogs } from './components/Dialogs'
import { GuidesProvider } from './components/GuidesProvider'
import GuidesTable from './components/Table'
import { UploadGuideMenu } from './components/UploadGuideMenu'

function Page() {
  const { electionRound } = useCurrentElectionRound()
  // Archived election rounds are frozen, so nothing new can be uploaded to them.
  const isArchived = electionRound?.status === ElectionRoundStatus.Archived

  return (
    <GuidesProvider>
      <div className='flex items-center justify-between'>
        <div>
          <H1>Observer guides</H1>
          <P>Here&apos;s all guides your observers have access to</P>
        </div>
        <UploadGuideMenu disabled={isArchived} />
      </div>
      <GuidesTable />
      <GuidesDialogs />
    </GuidesProvider>
  )
}

export default Page
