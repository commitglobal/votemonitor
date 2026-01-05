import {
    Breadcrumb,
    BreadcrumbItem,
    BreadcrumbLink,
    BreadcrumbList,
    BreadcrumbPage,
    BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { useFormContext } from '@/hooks/form-context'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId/edit.$languageCode'
import { Link } from '@tanstack/react-router'

export function FormBreadcrumb() {
    const { electionRoundId, formId, languageCode } = Route.useParams()
    const { from } = Route.useSearch()
    const { data: formData } = useSuspenseGetFormDetails(electionRoundId, formId)
    const form = useFormContext()


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
                    <BreadcrumbLink asChild>
                        <Link
                            to='/elections/$electionRoundId/forms/$formId'
                            params={{ electionRoundId, formId }}
                            className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
                        >
                            <form.Subscribe selector={(state) => state.values.name}>
                                {(name) => (
                                    <span>{name}</span>
                                )}
                            </form.Subscribe>
                        </Link>
                    </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                    <BreadcrumbPage>Edit</BreadcrumbPage>
                </BreadcrumbItem>
            </BreadcrumbList>
        </Breadcrumb>
    )
}

