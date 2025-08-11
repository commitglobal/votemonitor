export interface CoalitionMemberModel {
  id: string;
  name: string;
}
export interface CoalitionModel {
  id: string;
  isInCoalition: boolean;
  name: string;
  leaderId: string;
  leaderName: string;
  numberOfMembers: number;
  members: CoalitionMemberModel[];
}
