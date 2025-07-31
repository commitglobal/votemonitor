import type { MonitoredElection } from "@/types/monitored-election";
import ElectionCard from "./ElectionCard";

export default function ElectionList({
  elections,
  title,
}: {
  elections: MonitoredElection[];
  title: string;
}) {
  if (elections.length === 0) {
    return (
      <div className="space-y-4">
        <h2 className="text-2xl font-semibold tracking-tight">{title}</h2>
        <div className="text-center py-8 text-muted-foreground">
          No elections found in this category.
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-semibold tracking-tight">{title}</h2>
      <div className="space-y-4">
        {elections.map((election) => (
          <ElectionCard key={election.id} election={election} />
        ))}
      </div>
    </div>
  );
}
