export interface PollingStation {
    id: string;
    address: string;
    displayOrder: number;
    tags: {
        county: string;
        locality: string;
        sectionNumber: number;
        sectionName: string;
    };
}
