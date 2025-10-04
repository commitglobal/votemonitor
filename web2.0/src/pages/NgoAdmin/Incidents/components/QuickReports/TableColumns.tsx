"use client";

import type { ColumnDef } from "@tanstack/react-table";
import { format } from "date-fns";
import { ChevronRightIcon } from "lucide-react";

import { DataTableColumnHeader } from "@/components/data-table-column-header";

import { Badge } from "@/components/ui/badge";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { DateTimeFormat } from "@/constants/formats";
import {
  mapQuickReportIncidentCategory,
  mapQuickReportLocationType,
} from "@/lib/i18n";
import { cn } from "@/lib/utils";
import {
  QuickReportFollowUpStatus,
  type QuickReportModel,
} from "@/types/quick-reports";
import { Link } from "@tanstack/react-router";

interface GetTasksTableColumnsProps {
  electionRoundId: string;
}

export function getQuickReportsTableColumns({
  electionRoundId,
}: GetTasksTableColumnsProps): ColumnDef<QuickReportModel>[] {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Time submitted" column={column} />
      ),
      accessorFn: (row) => row.timestamp,
      id: "timestamp",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>{format(row.original.timestamp, DateTimeFormat)}</div>
      ),
      meta: {
        label: "Time submitted",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Observer" column={column} />
      ),
      id: "observerName",
      accessorFn: (row) => row.observerName,
      enableSorting: true,
      enableGlobalFilter: true,

      meta: {
        label: "Observer",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="NGO" column={column} />
      ),
      accessorFn: (row) => row.ngoName,
      id: "ngoName",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.ngoName}</div>,

      meta: {
        label: "NGO",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Location type" column={column} />
      ),
      accessorFn: (row) => row.quickReportLocationType,
      id: "locationType",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>
          {mapQuickReportLocationType(row.original.quickReportLocationType)}
        </div>
      ),

      meta: {
        label: "Location type",
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Incident category" column={column} />
      ),
      accessorFn: (row) => row.incidentCategory,
      id: "incidentCategory",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>
          {mapQuickReportIncidentCategory(row.original.incidentCategory)}
        </div>
      ),

      meta: {
        label: "Incident category",
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Issue title" column={column} />
      ),
      accessorFn: (row) => row.title,
      id: "title",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => {
        const title = row.original.title || "";
        const truncated =
          title.length > 100 ? title.slice(0, 100) + "..." : title;
        return (
          <Tooltip>
            <TooltipTrigger asChild>
              <div className="cursor-pointer hover:underline" title={title}>
                {truncated}
              </div>
            </TooltipTrigger>
            <TooltipContent className="max-w-md whitespace-pre-wrap break-words">
              {title}
            </TooltipContent>
          </Tooltip>
        );
      },

      meta: {
        label: "Issue title",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Description" column={column} />
      ),
      accessorFn: (row) => row.description,
      id: "description",
      enableSorting: true,
      enableGlobalFilter: true,
      size: 200,
      cell: ({ row }) => (
        <Tooltip>
          <TooltipTrigger asChild>
            <div
              className="cursor-pointer hover:underline"
              title={row.original.description}
            >
              {row.original.description.length > 100
                ? row.original.description.slice(0, 100) + "..."
                : row.original.description}
            </div>
          </TooltipTrigger>
          <TooltipContent className="max-w-md whitespace-pre-wrap break-words">
            {row.original.description}
          </TooltipContent>
        </Tooltip>
      ),

      meta: {
        label: "Description",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Media files" column={column} />
      ),
      accessorFn: (row) => row.numberOfAttachments,
      id: "numberOfAttachments",
      enableSorting: true,
      enableGlobalFilter: true,

      meta: {
        label: "Media files",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Location - L1" column={column} />
      ),
      accessorFn: (row) => row.level1,
      id: "level1",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level1 ?? "-"}</div>,
      meta: {
        label: "Location - L1",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Location - L2" column={column} />
      ),
      accessorFn: (row) => row.level2,
      id: "level2",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level2 ?? "-"}</div>,
      meta: {
        label: "Location - L2",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Location - L3" column={column} />
      ),
      accessorFn: (row) => row.level3,
      id: "level3",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level3 ?? "-"}</div>,
      meta: {
        label: "Location - L3",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Location - L4" column={column} />
      ),
      accessorFn: (row) => row.level4,
      id: "level4",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level4 ?? "-"}</div>,
      meta: {
        label: "Location - L4",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Location - L5" column={column} />
      ),
      accessorFn: (row) => row.level5,
      id: "level5",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level5 ?? "-"}</div>,
      meta: {
        label: "Location - L5",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="PS number" column={column} />
      ),
      accessorFn: (row) => row.number,
      id: "number",
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: "PS number",
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="PS details" column={column} />
      ),
      accessorFn: (row) => row.pollingStationDetails,
      id: "pollingStationDetails",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.pollingStationDetails ?? "-"}</div>,
      meta: {
        label: "PS details",
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Follow-up status" column={column} />
      ),
      accessorFn: (row) => row.followUpStatus,
      id: "followUpStatus",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <Badge
          className={cn({
            "text-slate-700 bg-slate-200":
              row.original.followUpStatus ===
              QuickReportFollowUpStatus.NotApplicable,
            "text-red-600 bg-red-200":
              row.original.followUpStatus ===
              QuickReportFollowUpStatus.NeedsFollowUp,
            "text-green-600 bg-green-200":
              row.original.followUpStatus ===
              QuickReportFollowUpStatus.Resolved,
          })}
        >
          {row.original.followUpStatus ===
          QuickReportFollowUpStatus.NotApplicable
            ? "Not Applicable"
            : row.original.followUpStatus ===
              QuickReportFollowUpStatus.NeedsFollowUp
            ? "Needs follow-up"
            : "Resolved"}
        </Badge>
      ),
      meta: {
        label: "Follow-up status",
      },
    },
    {
      header: "",
      id: "actions",
      enableSorting: false,
      cell: ({ row }) => (
        <div className="text-right">
          <Link
            className="inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100"
            to="/elections/$electionRoundId/incidents/quick-report/$quickReportId"
            params={{ electionRoundId, quickReportId: row.original.id }}
          >
            <ChevronRightIcon className="w-4 text-purple-600" />
          </Link>
        </div>
      ),
    },
  ];
}
