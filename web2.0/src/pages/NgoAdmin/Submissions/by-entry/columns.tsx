import { DataTableColumnHeader } from "@/components/data-table-column-header";
import FormSubmissionFollowUpStatusBadge from "@/components/form-submission-follow-up-status-badge";
import { DateTimeFormat } from "@/constants/formats";
import type { FormSubmissionModel } from "@/types/forms-submission";
import { Link } from "@tanstack/react-router";
import type { ColumnDef } from "@tanstack/react-table";
import { format } from "date-fns";
import { ChevronRightIcon } from "lucide-react";

export const getFormSubmissionsColumns = (
  electionRoundId: string
): ColumnDef<FormSubmissionModel>[] => {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Entry ID" column={column} />
      ),
      accessorFn: (row) => row.submissionId,
      id: "submissionId",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Time submitted" column={column} />
      ),
      accessorFn: (row) => row.timeSubmitted,
      id: "timeSubmitted",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>
      ),
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Form code" column={column} />
      ),
      accessorFn: (row) => row.formCode,
      id: "formCode",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Form type" column={column} />
      ),
      accessorFn: (row) => row.formType,
      id: "formType",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Form name" column={column} />
      ),
      accessorFn: (row) => row.formName,
      id: "formName",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div className="break-words">
          {row.original.formName[row.original.defaultLanguage] ?? "-"}
        </div>
      ),
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
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Station number" column={column} />
      ),
      accessorFn: (row) => row.number,
      id: "number",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Observer" column={column} />
      ),
      accessorFn: (row) => row.observerName,
      id: "observerName",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.observerName}</div>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="NGO" column={column} />
      ),
      accessorFn: (row) => row.ngoName,
      id: "ngoName",
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.ngoName}</div>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Observer tags" column={column} />
      ),
      accessorFn: (row) => row.tags,
      id: "tags",
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { tags },
        },
      }) => "TBD",
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Questions answered" column={column} />
      ),
      accessorFn: (row) => row.numberOfQuestionsAnswered,
      id: "numberOfQuestionsAnswered",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Flagged answers" column={column} />
      ),
      accessorFn: (row) => row.numberOfFlaggedAnswers,
      id: "numberOfFlaggedAnswers",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Question notes" column={column} />
      ),
      accessorFn: (row) => row.notesCount,
      id: "notesCount",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Media files" column={column} />
      ),
      accessorFn: (row) => row.mediaFilesCount,
      id: "mediaFilesCount",
      enableSorting: true,
      enableGlobalFilter: true,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title="Follow-up status" column={column} />
      ),
      accessorFn: (row) => row.followUpStatus,
      id: "followUpStatus",
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <FormSubmissionFollowUpStatusBadge
          followUpStatus={row.original.followUpStatus}
        />
      ),
    },
    {
      header: "",
      id: "action",
      enableSorting: false,
      cell: ({ row }) => (
        <div className="text-right">
          <Link
            className="inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100"
            params={{
              electionRoundId,
              submissionId: row.original.submissionId,
            }}
            to="/elections/$electionRoundId/submissions/$submissionId"
          >
            <ChevronRightIcon className="w-4 h-4" />
          </Link>
        </div>
      ),
    },
  ];
};
