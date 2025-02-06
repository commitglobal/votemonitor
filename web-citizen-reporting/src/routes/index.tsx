import { FormCard } from "@/components/FormCard";
import { useCitizenForms } from "@/hooks";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
  component: RouteComponent,
});

function RouteComponent() {
  const { data } = useCitizenForms();
  const { DEFAULT_LANGUAGE } = Route.useRouteContext();
  return (
    <div className="flex flex-col md:min-h-screen justify-center items-center">
      <h1 className="mb-4 text-4xl font-extrabold leading-none tracking-tight text-purple-900 md:text-5xl lg:text-6xl dark:text-white">
        Citizen Reporting
      </h1>
      <p className="text-lg font-normal text-gray-500 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-400">
        Choose a form to get started
      </p>

      <div className="mt-8 grid grid-cols-1 md:grid-cols-4 gap-4">
        {data?.forms.map((form) => (
          <FormCard formData={form} languageCode={DEFAULT_LANGUAGE} />
        ))}
      </div>
    </div>
  );
}
