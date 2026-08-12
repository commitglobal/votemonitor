import { H1, P } from '@/components/ui/typography'
import Table from './components/Table'

function Page() {
  return (
    <div className='flex flex-col gap-6'>
      <div>
        <H1>Observers</H1>
        <P>Everyone monitoring this election round on your behalf</P>
      </div>
      <Table />
    </div>
  )
}

export default Page
