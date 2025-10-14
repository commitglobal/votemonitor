import { createFileRoute, Outlet, useMatchRoute } from '@tanstack/react-router'
import { SubmissionsRoutePage } from '@/pages/NgoAdmin/Submissions/SubmissionsRoutePage'
import { createFileRoute, Outlet, useMatchRoute } from '@tanstack/react-router'
import { SubmissionsRoutePage } from '@/pages/NgoAdmin/Submissions/SubmissionsRoutePage'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/submissions'
  '/(app)/elections/$electionRoundId/submissions'
)({
  component: RouteComponent,
})

function RouteComponent() {
  const matchRoute = useMatchRoute()
  const matchSubmissionDetails = matchRoute({
    to: '/elections/$electionRoundId/submissions/$submissionId',
    fuzzy: false,
  })
  const matchAggregatedByForm = matchRoute({
    to: '/elections/$electionRoundId/submissions/by-form/$formId',
    fuzzy: false,
  })

  const isDetailRoute =
    matchSubmissionDetails && matchSubmissionDetails.submissionId !== 'by-form'

  return isDetailRoute ? (
    <Outlet />
  ) : matchAggregatedByForm ? (
    <Outlet />
  ) : (
    <SubmissionsRoutePage />
  )
}
