import { ElectionRoundVM } from "../common/models/election-round.model";

export const electionRoundSorter = (a: ElectionRoundVM, b: ElectionRoundVM) => {
  // Sort by startDate descending
  const dateDiff = b.startDate.localeCompare(a.startDate);
  if (dateDiff !== 0) return dateDiff;

  // If startDate is the same, sort by title ascending
  return a.title.localeCompare(b.title);
};
