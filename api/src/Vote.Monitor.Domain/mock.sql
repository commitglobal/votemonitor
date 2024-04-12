-- Description: Mock data for the Users table
INSERT INTO public."Users" ("Id", "Name", "Login", "Password", "Role", "Status") 
VALUES ('1', 'John Doe', 'user', 'pass', 'PlatformAdmin', 'Active');

INSERT INTO public."PlatformAdmins" ("Id") VALUES ('1');

-- Description: Mock data for the PollingStations table
INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174001', 'Strada Victoriei 1, Bucuresti', 1, '{"county": "Bucuresti", "locality": "Sector 1", "sectionNumber": "1", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174002', 'Strada Mihai Viteazu 10, Cluj-Napoca', 2, '{"county": "Cluj", "locality": "Cluj-Napoca", "sectionNumber": "2", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174003', 'Strada Republicii 5, Timisoara', 3, '{"county": "Timis", "locality": "Timisoara", "sectionNumber": "3", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174004', 'Strada Avram Iancu 20, Cluj-Napoca', 4, '{"county": "Cluj", "locality": "Cluj-Napoca", "sectionNumber": "4", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174005', 'Bulevardul Unirii 1, Bucuresti', 5, '{"county": "Bucuresti", "locality": "Sector 3", "sectionNumber": "5", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174006', 'Strada Libertatii 15, Timisoara', 6, '{"county": "Timis", "locality": "Timisoara", "sectionNumber": "6", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174007', 'Strada Stefan cel Mare 8, Iasi', 7, '{"county": "Iasi", "locality": "Iasi", "sectionNumber": "7", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174008', 'Bulevardul Eroilor 12, Brasov', 8, '{"county": "Brasov", "locality": "Brasov", "sectionNumber": "8", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174009', 'Strada Mihai Eminescu 3, Constanta', 9, '{"county": "Constanta", "locality": "Constanta", "sectionNumber": "9", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174010', 'Strada Vasile Alecsandri 7, Craiova', 10, '{"county": "Dolj", "locality": "Craiova", "sectionNumber": "10", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174011', 'Bulevardul Carol I 25, Bucharest', 11, '{"county": "Bucharest", "locality": "Sector 5", "sectionNumber": "11", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174012', 'Strada Mihail Kogalniceanu 12, Cluj-Napoca', 12, '{"county": "Cluj", "locality": "Cluj-Napoca", "sectionNumber": "12", "sectionName": "Scoala nr. 1"}');

INSERT INTO "PollingStations" ("Id", "Address", "DisplayOrder", "Tags")
VALUES ('123e4567-e89b-12d3-a456-426614174013', 'Strada Vasile Lupu 5, Iasi', 13, '{"county": "Iasi", "locality": "Iasi", "sectionNumber": "13", "sectionName": "Scoala nr. 1"}');


-- Observers
INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174001', 'Ion Popescu', 'ion.popescu', 'password1', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174002', 'Maria Ionescu', 'maria.ionescu', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174003', 'John Smith', 'john.smith', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174004', 'Jane Doe', 'jane.doe', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174005', 'Michael Johnson', 'michael.johnson', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174006', 'Emily Wilson', 'emily.wilson', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174007', 'David Brown', 'david.brown', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174008', 'Sarah Davis', 'sarah.davis', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174009', 'Daniel Wilson', 'daniel.wilson', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174010', 'Olivia Johnson', 'olivia.johnson', 'password2', 'Observer', 'Active');

INSERT INTO "Users" ("Id", "Name", "Login", "Password", "Role", "Status")
VALUES ('123e4567-e89b-12d3-a456-426614174011', 'James Smith', 'james.smith', 'password2', 'Observer', 'Active');

INSERT INTO "Observers" ("Id")
SELECT "Id"
FROM "Users"
WHERE "Role" = 'Observer';

