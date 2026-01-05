import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Link } from '@tanstack/react-router'
import { FormSearch } from '@/types/form'

interface FormBreadcrumbProps {
  electionRoundId: string
  formName: string
  from?: FormSearch
}

export function FormBreadcrumb({
  electionRoundId,
  formName,
  from,
}: FormBreadcrumbProps) {
  return (
    <Breadcrumb className='mb-4'>
      <BreadcrumbList>
        <BreadcrumbItem>
          <BreadcrumbLink asChild>
            <Link
              to='/elections/$electionRoundId/forms'
              params={{ electionRoundId }}
              search={from}
              className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
            >
              Forms
            </Link>
          </BreadcrumbLink>
        </BreadcrumbItem>
        <BreadcrumbSeparator />
        <BreadcrumbItem>
          <BreadcrumbPage>{formName}</BreadcrumbPage>
        </BreadcrumbItem>
      </BreadcrumbList>
    </Breadcrumb>
  )
}

