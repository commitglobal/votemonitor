import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/thank-you")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="flex flex-col min-h-screen justify-center items-center">
      <h1 className="mb-4 text-4xl font-extrabold leading-none tracking-tight text-purple-900 md:text-5xl lg:text-6xl dark:text-white">
        Thank you!
      </h1>
      <p className="text-lg font-normal text-gray-500 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-400">
        Your report was submitted.
      </p>
    </div>
  );
}
