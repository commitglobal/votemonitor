import { useForms } from "@/queries/use-forms";

function FormsList() {
  const { data: form } = useForms();

  return <>{JSON.stringify(form)}</>;
}

export default FormsList;
