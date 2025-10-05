"use client";

import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import {
  Card,
  CardAction,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  mapQuickReportIncidentCategory,
  mapQuickReportLocationType,
} from "@/lib/i18n";

import {
  Item,
  ItemActions,
  ItemContent,
  ItemDescription,
  ItemGroup,
  ItemMedia,
  ItemTitle,
} from "@/components/ui/item";

import QuickReportFollowUpStatusBadge from "@/components/quick-report-follow-up-status-badge";
import { Attachment } from "@/components/ui/attachment";
import { Button } from "@/components/ui/button";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { downloadFile } from "@/lib/utils";
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
import { DownloadIcon } from "lucide-react";
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
    <Item>
      <ItemContent>
        <ItemTitle>Polling station</ItemTitle>
        <div>
          <Breadcrumb>
            <BreadcrumbList>
              {levels.map(({ value, level }, index) => (
                <div key={level} className="flex items-center">
                  {index > 0 && <BreadcrumbSeparator />}
                  <BreadcrumbItem>
                    <BreadcrumbLink asChild>
                      <Link
                        to="/elections/$electionRoundId/incidents"
                        search={buildSearchFilters(quickReport, level)}
                        params={{ electionRoundId }}
                        className="underline text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance"
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
                    <BreadcrumbLink asChild>
                      <Link
                        to="/elections/$electionRoundId/incidents"
                        search={{
                          ...buildSearchFilters(quickReport, 5),
                          pollingStationNumberFilter: quickReport.number,
                        }}
                        params={{ electionRoundId }}
                        className="underline text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance"
                      >
                        {quickReport.number}
                      </Link>
                    </BreadcrumbLink>
                  </BreadcrumbItem>
                </>
              )}
            </BreadcrumbList>
          </Breadcrumb>
        </div>
      </ItemContent>
    </Item>
  ) : quickReport.quickReportLocationType ===
    QuickReportLocationType.OtherPollingStation ? (
    <Item>
      <ItemContent>
        <ItemTitle>Polling station details</ItemTitle>
        <ItemDescription>{quickReport.pollingStationDetails}</ItemDescription>
      </ItemContent>
    </Item>
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
        onSuccess: async (_, { electionRoundId }) => {
          toast.success("Follow-up status updated");
          invalidate();
          await queryClient.invalidateQueries({
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
    <>
      <Breadcrumb className="mb-4">
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link
                to="/elections/$electionRoundId/incidents"
                params={{ electionRoundId }}
                className="underline text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance"
              >
                Quick reports
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbPage>{quickReportId}</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>
      <Card>
        <CardContent>
          <ItemGroup className="flex flex-row gap-2 justify-between">
            <Item>
              <ItemContent>
                <ItemTitle>Observer</ItemTitle>
                <ItemDescription>
                  <Link
                    to="/elections/$electionRoundId/observers/$observerId"
                    params={{
                      electionRoundId,
                      observerId: quickReport.monitoringObserverId,
                    }}
                  >
                    {quickReport.observerName}
                  </Link>
                </ItemDescription>
              </ItemContent>
            </Item>

            <Item>
              <ItemContent>
                <ItemTitle>Follow up status</ItemTitle>
                <ItemDescription>
                  {isReadOnly ? (
                    <QuickReportFollowUpStatusBadge
                      status={quickReport.followUpStatus}
                    />
                  ) : (
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
                          {Object.values(QuickReportFollowUpStatus).map(
                            (status) => (
                              <SelectItem key={status} value={status}>
                                <QuickReportFollowUpStatusBadge
                                  status={status}
                                />
                              </SelectItem>
                            )
                          )}
                        </SelectGroup>
                      </SelectContent>
                    </Select>
                  )}
                </ItemDescription>
              </ItemContent>
            </Item>
          </ItemGroup>

          <ItemGroup>
            {!quickReport.isOwnObserver ? (
              <Item>
                <ItemContent>
                  <ItemTitle>NGO</ItemTitle>
                  <ItemDescription>{quickReport.ngoName}</ItemDescription>
                </ItemContent>
              </Item>
            ) : null}
            <Item>
              <ItemContent>
                <ItemTitle>Incident category</ItemTitle>
                <ItemDescription>
                  {mapQuickReportIncidentCategory(quickReport.incidentCategory)}
                </ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Location type</ItemTitle>
                <ItemDescription>
                  {mapQuickReportLocationType(
                    quickReport.quickReportLocationType
                  )}
                </ItemDescription>
              </ItemContent>
            </Item>

            <PollingStationDetails quickReport={quickReport} />

            <Item>
              <ItemContent>
                <ItemTitle>Title</ItemTitle>
                <ItemDescription>{quickReport.title}</ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Description</ItemTitle>
                <ItemDescription>{quickReport.description}</ItemDescription>
              </ItemContent>
            </Item>
          </ItemGroup>
          <ItemGroup>
            <Item>
              <ItemContent>
                <ItemTitle>Attachments</ItemTitle>
              </ItemContent>
            </Item>
            <Collapsible>
              <CollapsibleTrigger asChild>
                <Button variant="outline" className="w-fit mb-2">
                  {quickReport.attachments.length > 0
                    ? `Show Attachments (${quickReport.attachments.length})`
                    : "No Attachments"}
                </Button>
              </CollapsibleTrigger>
              <CollapsibleContent>
                <div className="flex flex-col gap-2 mt-2">
                  {quickReport.attachments.map((attachment, index) => (
                    <Item variant="outline" key={index}>
                      <ItemMedia>
                        <Attachment
                          src={attachment.presignedUrl}
                          mimeType={attachment.mimeType}
                          fileName={attachment.fileName}
                          width="530px"
                          height="300px"
                        />
                      </ItemMedia>
                      <ItemContent className="gap-1">
                        <ItemTitle>{attachment.fileName}</ItemTitle>
                        <ItemDescription>{attachment.mimeType}</ItemDescription>
                      </ItemContent>
                      <ItemActions>
                        <Button
                          variant="outline"
                          size="icon"
                          onClick={() =>
                            downloadFile(
                              attachment.presignedUrl,
                              attachment.fileName
                            )
                          }
                        >
                          <DownloadIcon className="size-4" />
                        </Button>
                      </ItemActions>
                    </Item>
                  ))}
                </div>
              </CollapsibleContent>
            </Collapsible>
          </ItemGroup>
        </CardContent>
      </Card>
    </>
  );
}

export default Page;
