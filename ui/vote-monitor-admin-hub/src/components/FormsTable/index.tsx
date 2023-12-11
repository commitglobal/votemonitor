import { DataTable } from "./data-table";
import { columns } from "./columns";
import { useGetFormsQuery } from "@/redux/api/formsApi";
import { DataTableLoading } from "./data-table-loading";

const FormsTable = () => {

  const { data: rows, isLoading } = useGetFormsQuery();
  return (
    <>
      <div className="hidden h-full flex-1 flex-col space-y-8 p-8 md:flex">
        <div className="flex items-center justify-between space-y-2">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Forms</h2>
          </div>
        </div>
        {!isLoading && rows && <DataTable data={rows!.items} columns={columns} />}
        {isLoading && <DataTableLoading columnCount={columns.length}/>}
      </div>
    </>
  )
}

export default FormsTable;
