"use client";

import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import { Card, CardContent } from "@/components/ui/card";
import {
  mapQuickReportFollowUpStatus,
  mapQuickReportIncidentCategory,
  mapQuickReportLocationType,
} from "@/lib/i18n";

import {
  DescriptionDetails,
  DescriptionList,
  DescriptionTerm,
} from "@/components/ui/description-list";

import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { queryClient } from "@/main";
import { useUpdateQuickReportFollowUpStatusMutation } from "@/mutations/quick-reports";
import { useElectionRoundDetails } from "@/queries/elections";
import { quickReportKeys } from "@/queries/quick-reports";
import { Route } from "@/routes/(app)/elections/$electionRoundId/incidents/quick-report.$quickReportId";
import { ElectionRoundStatus } from "@/types/election";
import {
  QuickReportFollowUpStatus,
  QuickReportLocationType,
  type QuickReportModel,
} from "@/types/quick-reports";
import { Link, useRouter } from "@tanstack/react-router";
import { toast } from "sonner";

const buildSearchFilters = (quickReport: QuickReportModel, level: number) => {
  const filters: Record<string, string> = {};
  const levels = [
    { key: "level1Filter", value: quickReport.level1 },
    { key: "level2Filter", value: quickReport.level2 },
    { key: "level3Filter", value: quickReport.level3 },
    { key: "level4Filter", value: quickReport.level4 },
    { key: "level5Filter", value: quickReport.level5 },
  ];

  levels.slice(0, level).forEach(({ key, value }) => {
    if (value) filters[key] = value;
  });

  return filters;
};

function PollingStationDetails({
  quickReport,
}: {
  quickReport: QuickReportModel;
}) {
  const { electionRoundId } = Route.useParams();

  const levels = [
    { value: quickReport.level1, level: 1 },
    { value: quickReport.level2, level: 2 },
    { value: quickReport.level3, level: 3 },
    { value: quickReport.level4, level: 4 },
    { value: quickReport.level5, level: 5 },
  ].filter((item) => item.value);

  return quickReport.quickReportLocationType ===
    QuickReportLocationType.VisitedPollingStation ? (
    <>
      <DescriptionTerm>Polling station</DescriptionTerm>
      <DescriptionDetails>
        <Breadcrumb>
          <BreadcrumbList className="text-foreground">
            {levels.map(({ value, level }, index) => (
              <div key={level} className="flex items-center">
                {index > 0 && <BreadcrumbSeparator />}
                <BreadcrumbItem>
                  <BreadcrumbLink
                    asChild
                    className="hover:text-muted-foreground"
                  >
                    <Link
                      to="/elections/$electionRoundId/incidents"
                      search={buildSearchFilters(quickReport, level)}
                      params={{ electionRoundId }}
                      className="underline "
                    >
                      {value}
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
              </div>
            ))}

            {quickReport.number && (
              <>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbLink
                    asChild
                    className="hover:text-muted-foreground"
                  >
                    <Link
                      to="/elections/$electionRoundId/incidents"
                      search={{
                        ...buildSearchFilters(quickReport, 5),
                        pollingStationNumberFilter: quickReport.number,
                      }}
                      params={{ electionRoundId }}
                      className="underline"
                    >
                      {quickReport.number}
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
              </>
            )}
          </BreadcrumbList>
        </Breadcrumb>
      </DescriptionDetails>
    </>
  ) : quickReport.quickReportLocationType ===
    QuickReportLocationType.OtherPollingStation ? (
    <>
      <DescriptionTerm>Polling station details</DescriptionTerm>
      <DescriptionDetails>
        {quickReport.pollingStationDetails}
      </DescriptionDetails>
    </>
  ) : (
    <></>
  );
}

function Page() {
  const { electionRoundId, quickReportId } = Route.useParams();
  const { invalidate } = useRouter();
  const quickReport = Route.useLoaderData();
  const { data: electionRound } = useElectionRoundDetails(electionRoundId);
  const { mutate: updateStatus } = useUpdateQuickReportFollowUpStatusMutation();

  const handleFollowUpStatusChange = (
    followUpStatus: QuickReportFollowUpStatus
  ) => {
    updateStatus(
      { electionRoundId, quickReportId, followUpStatus },
      {
        onSuccess: (_, { electionRoundId }) => {
          toast.success("Follow-up status updated");
          invalidate();
          void queryClient.invalidateQueries({
            queryKey: quickReportKeys.all(electionRoundId),
          });
        },
        onError: () => {
          toast.error("Error updating follow up status", {
            description: "Please contact tech support",
          });
        },
      }
    );
  };

  const isReadOnly =
    !quickReport.isOwnObserver ||
    electionRound?.status === ElectionRoundStatus.Archived;

  return (
    <Card>
      <CardContent className="pt-0">
        <DescriptionList>
          <DescriptionTerm>Follow up status</DescriptionTerm>
          <DescriptionDetails>
            <Select
              onValueChange={handleFollowUpStatusChange}
              defaultValue={quickReport.followUpStatus}
              value={quickReport.followUpStatus}
              disabled={isReadOnly}
            >
              <SelectTrigger className="w-full sm:w-[220px]">
                <SelectValue placeholder="Follow-up status" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {Object.values(QuickReportFollowUpStatus).map((status) => (
                    <SelectItem key={status} value={status}>
                      {mapQuickReportFollowUpStatus(status)}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </DescriptionDetails>
          <DescriptionTerm>Observer</DescriptionTerm>
          <DescriptionDetails>
            <Link
              to="/elections/$electionRoundId/observers/$observerId"
              className="underline hover:text-muted-foreground transition-colors"
              params={{
                electionRoundId,
                observerId: quickReport.monitoringObserverId,
              }}
            >
              {quickReport.observerName}
            </Link>
          </DescriptionDetails>
          {!quickReport.isOwnObserver ? (
            <>
              <DescriptionTerm>NGO</DescriptionTerm>
              <DescriptionDetails>{quickReport.ngoName}</DescriptionDetails>
            </>
          ) : null}
          <DescriptionTerm>Incident category</DescriptionTerm>
          <DescriptionDetails>
            {mapQuickReportIncidentCategory(quickReport.incidentCategory)}
          </DescriptionDetails>

          <DescriptionTerm>Location type</DescriptionTerm>
          <DescriptionDetails>
            {mapQuickReportLocationType(quickReport.quickReportLocationType)}
          </DescriptionDetails>

          <PollingStationDetails quickReport={quickReport} />
          <DescriptionTerm>Title</DescriptionTerm>
          <DescriptionDetails>{quickReport.title}</DescriptionDetails>
          <DescriptionTerm>Description</DescriptionTerm>
          <DescriptionDetails> {quickReport.description}</DescriptionDetails>

          <DescriptionTerm>Attachments</DescriptionTerm>
          <DescriptionDetails>$1,955.00</DescriptionDetails>
        </DescriptionList>
      </CardContent>
    </Card>
  );
}

export default Page;
