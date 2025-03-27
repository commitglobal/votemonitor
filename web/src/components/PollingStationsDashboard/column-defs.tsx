import i18n from "@/i18n";
import { DataTableColumnHeader } from "../ui/DataTable/DataTableColumnHeader";
import { ColumnDef } from "@tanstack/react-table";
import { PollingStation } from "@/common/types";
import TableTagList from "../table-tag-list/TableTagList";

export const pollingStationColDefs: ColumnDef<PollingStation>[] = [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level1')} column={column} />
      ),
      accessorKey: 'level1',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level1 },
        },
      }) => <p>{level1}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level2')} column={column} />
      ),
      accessorKey: 'level2',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level2 },
        },
      }) => <p>{level2}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level3')} column={column} />
      ),
      accessorKey: 'level3',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level3 },
        },
      }) => <p>{level3}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level4')} column={column} />
      ),
      accessorKey: 'level4',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level4 },
        },
      }) => <p>{level4}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level5')} column={column} />
      ),
      accessorKey: 'level5',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level5 },
        },
      }) => <p>{level5}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.number')} column={column} />
      ),
      accessorKey: 'number',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { number },
        },
      }) => <p>{number}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader
          title={i18n.t('translation:electionEvent.pollingStations.headers.address')}
          column={column}
        />
      ),
      accessorKey: 'address',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { address },
        },
      }) => <p>{address}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader
          title={i18n.t('translation:electionEvent.pollingStations.headers.displayOrder')}
          column={column}
        />
      ),
      accessorKey: 'displayOrder',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { displayOrder },
        },
      }) => <p>{displayOrder}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.tags')} column={column} />
      ),
      accessorKey: 'tags',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { tags },
        },
      }) => <TableTagList tags={Object.entries(tags ?? {}).map(([key, value]) => `${key} : ${value}`)} />,
    },
  ];
  