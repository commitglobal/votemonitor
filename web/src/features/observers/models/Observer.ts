export enum ObserverStatus {
  ACTIVE = 'Active',
  Pending = 'Pending',
  Deactivated = 'Deactivated',
}

export interface Observer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  status: ObserverStatus;
  phoneNumber: string;
}
