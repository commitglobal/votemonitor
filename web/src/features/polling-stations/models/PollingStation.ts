export interface PollingStation {
  id: string;
  level1: string;
  level2: string;
  level3: string;
  level4: string;
  level5: string;
  number: string;
  address: string;
  displayOrder: number;
  tags: Record<string, any>;
}