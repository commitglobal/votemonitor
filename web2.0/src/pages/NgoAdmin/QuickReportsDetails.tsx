"use client";

import { Badge } from "@/components/ui/badge";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { DateTimeFormat } from "@/constants/formats";
import {
  mapQuickReportFollowUpStatus,
  mapQuickReportIncidentCategory,
} from "@/lib/i18n";
import { queryClient } from "@/main";
import { useUpdateQuickReportFollowUpStatusMutation } from "@/mutations/quick-reports";
import { useElectionRoundDetails } from "@/queries/elections";
import { quickReportKeys } from "@/queries/quick-reports";
import { Route } from "@/routes/(app)/elections/$electionRoundId/incidents/quick-report.$quickReportId";
import { ElectionRoundStatus } from "@/types/election";
import {
  QuickReportFollowUpStatus,
  QuickReportLocationType,
} from "@/types/quick-reports";
import { Link, useRouter } from "@tanstack/react-router";
import { format } from "date-fns/format";
import { Calendar, FileText, MapPin, User } from "lucide-react";
import { toast } from "sonner";

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

  const buildSearchFilters = (level: number) => {
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

  const renderLocationBreadcrumb = () => {
    const levels = [
      { value: quickReport.level1, level: 1 },
      { value: quickReport.level2, level: 2 },
      { value: quickReport.level3, level: 3 },
      { value: quickReport.level4, level: 4 },
      { value: quickReport.level5, level: 5 },
    ].filter((item) => item.value);

    return (
      <Breadcrumb>
        <BreadcrumbList>
          {levels.map(({ value, level }, index) => (
            <div key={level} className="flex items-center">
              {index > 0 && <BreadcrumbSeparator />}
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to="/elections/$electionRoundId/incidents"
                    search={buildSearchFilters(level)}
                    params={{ electionRoundId }}
                    className="underline transition-colors"
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
                      ...buildSearchFilters(5),
                      pollingStationNumberFilter: quickReport.number,
                    }}
                    params={{ electionRoundId }}
                    className="underline font-semibold transition-colors"
                  >
                    {quickReport.number}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
            </>
          )}
        </BreadcrumbList>
      </Breadcrumb>
    );
  };

  const renderLocationInfo = () => {
    const { quickReportLocationType } = quickReport;

    if (
      quickReportLocationType ===
      QuickReportLocationType.NotRelatedToAPollingStation
    ) {
      return (
        <div className="flex items-center gap-3 p-4 bg-gray-50 border border-gray-200 rounded-lg">
          <MapPin className="w-5 h-5 text-gray-500" />
          <span className="text-sm text-gray-700 font-medium">
            Not related to polling station
          </span>
        </div>
      );
    }

    if (
      quickReportLocationType === QuickReportLocationType.OtherPollingStation
    ) {
      return (
        <div className="space-y-3">
          <CardDescription className="text-sm font-medium text-gray-600">
            Polling Station Details
          </CardDescription>
          <div className="flex items-start gap-3 p-4  border  rounded-lg">
            <MapPin className="w-5 h-5  mt-0.5 flex-shrink-0" />
            <span className="text-sm  leading-relaxed">
              {quickReport.pollingStationDetails}
            </span>
          </div>
        </div>
      );
    }

    if (
      quickReportLocationType === QuickReportLocationType.VisitedPollingStation
    ) {
      return (
        <div className="space-y-3">
          <div className="p-4 bg-gray-50 border border-gray-200 rounded-lg">
            {renderLocationBreadcrumb()}
          </div>
        </div>
      );
    }

    return null;
  };

  const isDisabled =
    !quickReport.isOwnObserver ||
    electionRound?.status === ElectionRoundStatus.Archived;

  return (
    <div className="container mx-auto px-4 py-6 max-w-4xl">
      <Card className="shadow-sm">
        <CardHeader className="space-y-6 pb-6">
          {/* Header with Title and Status */}
          <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
            <div className="flex-1 min-w-0">
              <CardTitle className="text-2xl font-bold text-gray-900 flex items-center gap-3">
                <FileText className="w-6 h-6 text-blue-600 flex-shrink-0" />
                <span className="truncate">{quickReport.title}</span>
              </CardTitle>
            </div>

            <Select
              onValueChange={handleFollowUpStatusChange}
              defaultValue={quickReport.followUpStatus}
              value={quickReport.followUpStatus}
              disabled={isDisabled}
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
          </div>

          <Separator />

          {/* Report Metadata */}
          <div className="grid sm:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div>
                <CardDescription className="text-sm font-medium text-gray-600 mb-2">
                  Reported At
                </CardDescription>
                <div className="flex items-center gap-2 text-sm text-gray-900">
                  <Calendar className="w-4 h-4 text-gray-500" />
                  <span>{format(quickReport.timestamp, DateTimeFormat)}</span>
                </div>
              </div>

              <div>
                <CardDescription className="text-sm font-medium text-gray-600 mb-2">
                  Incident Category
                </CardDescription>
                <Badge variant="outline" className="font-medium text-sm">
                  {mapQuickReportIncidentCategory(quickReport.incidentCategory)}
                </Badge>
              </div>
            </div>

            <div>
              <CardDescription className="text-sm font-medium text-gray-600 mb-2">
                Observer
              </CardDescription>
              <div className="flex items-center gap-2">
                <User className="w-4 h-4 text-gray-500" />
                <span className="font-medium text-gray-900">
                  {quickReport.observerName}
                </span>
              </div>
            </div>
          </div>

          <Separator />

          {/* Location Information */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900">
              Location Details
            </h3>
            {renderLocationInfo()}
          </div>
        </CardHeader>

        <CardContent className="pt-0">
          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-900">Description</h3>
            <div className="prose prose-sm max-w-none">
              <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
                <p className="text-gray-700 leading-relaxed whitespace-pre-wrap m-0">
                  {quickReport.description}
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

export default Page;
