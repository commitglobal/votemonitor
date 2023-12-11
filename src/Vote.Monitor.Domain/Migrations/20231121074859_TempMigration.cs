using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vote.Monitor.Domain.Migrations
{
    /// <inheritdoc />
    public partial class TempMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Code = table.Column<string>(type: "text", nullable: false),
                    LanguageCode = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "FullName", "Iso2", "Iso3", "Name", "NumericCode" },
                values: new object[,]
                {
                    { new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"), "Republic of Azerbaijan", "AZ", "AZE", "Azerbaijan", "031" },
                    { new Guid("015a9f83-6e57-bc1e-8227-24a4e5248582"), "Republic of the Union of Myanmar", "MM", "MMR", "Myanmar", "104" },
                    { new Guid("057884bc-3c2e-dea9-6522-b003c9297f7a"), "Republic of Palau", "PW", "PLW", "Palau", "585" },
                    { new Guid("067c9448-9ad0-2c21-a1dc-fbdf5a63d18d"), "State of Qatar", "QA", "QAT", "Qatar", "634" },
                    { new Guid("06f8ad57-7133-9a5e-5a83-53052012b014"), "Tunisian Republic", "TN", "TUN", "Tunisia", "788" },
                    { new Guid("0797a7d5-bbc0-2e52-0de8-14a42fc80baa"), "Kingdom of Belgium", "BE", "BEL", "Belgium", "056" },
                    { new Guid("0868cdd3-7f50-5a25-88d6-98c45f9157e3"), "United States Minor Outlying Islands", "UM", "UMI", "United States Minor Outlying Islands", "581" },
                    { new Guid("08a999e4-e420-b864-2864-bef78c138448"), "Mayotte", "YT", "MYT", "Mayotte", "175" },
                    { new Guid("0932ed88-c79f-591a-d684-9a77735f947e"), "Kyrgyz Republic", "KG", "KGZ", "Kyrgyz Republic", "417" },
                    { new Guid("096a8586-9702-6fec-5f6a-6eb3b7b7837f"), "Guam", "GU", "GUM", "Guam", "316" },
                    { new Guid("0a25f96f-5173-2fff-a2f8-c6872393edf6"), "Republic of San Marino", "SM", "SMR", "San Marino", "674" },
                    { new Guid("0ab731f0-5326-44be-af3a-20aa33ad0f35"), "Kingdom of Sweden", "SE", "SWE", "Sweden", "752" },
                    { new Guid("0aebadaa-91b2-8794-c153-4f903a2a1004"), "Republic of Honduras", "HN", "HND", "Honduras", "340" },
                    { new Guid("0b3b04b4-9782-79e3-bc55-9ab33b6ae9c7"), "United Kingdom of Great Britain & Northern Ireland", "GB", "GBR", "United Kingdom of Great Britain and Northern Ireland", "826" },
                    { new Guid("0c0fef20-0e8d-98ea-7724-12cea9b3b926"), "Republic of Namibia", "NA", "NAM", "Namibia", "516" },
                    { new Guid("0d4fe6e6-ea1e-d1ce-5134-6c0c1a696a00"), "Faroe Islands", "FO", "FRO", "Faroe Islands", "234" },
                    { new Guid("0e0fefd5-9a05-fde5-bee9-ef56db7748a1"), "Turks and Caicos Islands", "TC", "TCA", "Turks and Caicos Islands", "796" },
                    { new Guid("0e2a1681-d852-67ae-7387-0d04be9e7fd3"), "Republic of Fiji", "FJ", "FJI", "Fiji", "242" },
                    { new Guid("0f1ba59e-ade5-23e5-6fce-e2fd3282e114"), "Christmas Island", "CX", "CXR", "Christmas Island", "162" },
                    { new Guid("10b58d9b-42ef-edb8-54a3-712636fda55a"), "Republic of Mozambique", "MZ", "MOZ", "Mozambique", "508" },
                    { new Guid("11765ad0-30f2-bab8-b616-20f88b28b21e"), "Tokelau", "TK", "TKL", "Tokelau", "772" },
                    { new Guid("11dbce82-a154-7aee-7b5e-d5981f220572"), "French Polynesia", "PF", "PYF", "French Polynesia", "258" },
                    { new Guid("1258ec90-c47e-ff72-b7e3-f90c3ee320f8"), "Democratic Republic of the Congo", "CD", "COD", "Congo", "180" },
                    { new Guid("13c69e56-375d-8a7e-c326-be2be2fd4cd8"), "Japan", "JP", "JPN", "Japan", "392" },
                    { new Guid("141e589a-7046-a265-d2f6-b2f85e6eeadd"), "Sint Maarten (Dutch part)", "SX", "SXM", "Sint Maarten (Dutch part)", "534" },
                    { new Guid("14f190c6-97c9-3e12-2eba-db17c59d6a04"), "Republic of Botswana", "BW", "BWA", "Botswana", "072" },
                    { new Guid("15639386-e4fc-120c-6916-c0c980e24be1"), "Commonwealth of Australia", "AU", "AUS", "Australia", "036" },
                    { new Guid("17ed5f0f-e091-94ff-0512-ad291bde94d7"), "Republic of Cabo Verde", "CV", "CPV", "Cabo Verde", "132" },
                    { new Guid("1934954c-66c2-6226-c5b6-491065a3e4c0"), "Republic of the Congo", "CG", "COG", "Congo", "178" },
                    { new Guid("19ea3a6a-1a76-23c8-8e4e-1d298f15207f"), "Commonwealth of Dominica", "DM", "DMA", "Dominica", "212" },
                    { new Guid("1b634ca2-2b90-7e54-715a-74cee7e4d294"), "Republic of Mauritius", "MU", "MUS", "Mauritius", "480" },
                    { new Guid("1d2aa3ab-e1c3-8c76-9be6-7a3b3eca35da"), "Republic of Maldives", "MV", "MDV", "Maldives", "462" },
                    { new Guid("1d974338-decf-08e5-3e62-89e1bbdbb003"), "Republic of Indonesia", "ID", "IDN", "Indonesia", "360" },
                    { new Guid("1e5c0dcc-83e9-f275-c81d-3bc49f88e70c"), "Lebanese Republic", "LB", "LBN", "Lebanon", "422" },
                    { new Guid("1f8be615-5746-277e-d82b-47596b5bb922"), "Republic of Croatia", "HR", "HRV", "Croatia", "191" },
                    { new Guid("2167da32-4f80-d31d-226c-0551970304eb"), "Republic of Seychelles", "SC", "SYC", "Seychelles", "690" },
                    { new Guid("220e980a-7363-0150-c250-89e83b967fb4"), "Saint Lucia", "LC", "LCA", "Saint Lucia", "662" },
                    { new Guid("29201cbb-ca65-1924-75a9-0c4d4db43001"), "United Arab Emirates", "AE", "ARE", "United Arab Emirates", "784" },
                    { new Guid("294978f0-2702-d35d-cfc4-e676148aea2e"), "Ireland", "IE", "IRL", "Ireland", "372" },
                    { new Guid("2a039b16-2adf-0fb8-3bdf-fbdf14358d9d"), "Portuguese Republic", "PT", "PRT", "Portugal", "620" },
                    { new Guid("2a1ca5b6-fba0-cfa8-9928-d7a2382bc4d7"), "Republic of Chad", "TD", "TCD", "Chad", "148" },
                    { new Guid("2a848549-9777-cf48-a0f2-b32c6f942096"), "Republic of Tajikistan", "TJ", "TJK", "Tajikistan", "762" },
                    { new Guid("2b68fb11-a0e0-3d23-5fb8-99721ecfc182"), "Anguilla", "AI", "AIA", "Anguilla", "660" },
                    { new Guid("2bebebe4-edaa-9160-5a0c-4d99048bd8d5"), "Republic of Haiti", "HT", "HTI", "Haiti", "332" },
                    { new Guid("2dc643bd-cc6c-eb0c-7314-44123576f0ee"), "Republic of Estonia", "EE", "EST", "Estonia", "233" },
                    { new Guid("2e1bd9d8-df06-d773-0eb9-98e274b63b43"), "Federal Republic of Nigeria", "NG", "NGA", "Nigeria", "566" },
                    { new Guid("2f00fe86-a06b-dc95-0ea7-4520d1dec784"), "Greenland", "GL", "GRL", "Greenland", "304" },
                    { new Guid("2f49855b-ff93-c399-d72a-121f2bf28bc9"), "Saint Vincent and the Grenadines", "VC", "VCT", "Saint Vincent and the Grenadines", "670" },
                    { new Guid("2f4cc994-53f1-1763-8220-5d89e063804f"), "Western Sahara", "EH", "ESH", "Western Sahara", "732" },
                    { new Guid("316c68fc-9144-f6e1-8bf1-899fc54b2327"), "Barbados", "BB", "BRB", "Barbados", "052" },
                    { new Guid("3175ac19-c801-0b87-8e66-7480a40dcf1e"), "Republic of Senegal", "SN", "SEN", "Senegal", "686" },
                    { new Guid("3252e51a-5bc1-f065-7101-5b34ba493dc4"), "Slovakia (Slovak Republic)", "SK", "SVK", "Slovakia (Slovak Republic)", "703" },
                    { new Guid("32da0208-9048-1339-a8ee-6955cfff4c12"), "Bouvet Island (Bouvetøya)", "BV", "BVT", "Bouvet Island (Bouvetøya)", "074" },
                    { new Guid("3345e205-3e72-43ed-de1b-ac6e050543e5"), "Curaçao", "CW", "CUW", "Curaçao", "531" },
                    { new Guid("357369e3-85a8-86f7-91c7-349772ae7744"), "Republic of Uzbekistan", "UZ", "UZB", "Uzbekistan", "860" },
                    { new Guid("357c121b-e28d-1765-e699-cc4ec5ff86fc"), "Republic of Slovenia", "SI", "SVN", "Slovenia", "705" },
                    { new Guid("360e3c61-aaac-fa2f-d731-fc0824c05107"), "New Zealand", "NZ", "NZL", "New Zealand", "554" },
                    { new Guid("37a79267-d38a-aaef-577a-aa68a96880ae"), "Republic of Djibouti", "DJ", "DJI", "Djibouti", "262" },
                    { new Guid("37c89068-a8e9-87e8-d651-f86fac63673a"), "Swiss Confederation", "CH", "CHE", "Switzerland", "756" },
                    { new Guid("39be5e86-aea5-f64f-fd7e-1017fe24e543"), "British Virgin Islands", "VG", "VGB", "British Virgin Islands", "092" },
                    { new Guid("3bcd2aad-fb69-09f4-1ad7-2c7f5fa23f9f"), "Guadeloupe", "GP", "GLP", "Guadeloupe", "312" },
                    { new Guid("3c5828e0-16a8-79ba-4e5c-9b45065df113"), "Cayman Islands", "KY", "CYM", "Cayman Islands", "136" },
                    { new Guid("3ce3d958-7341-bd79-f294-f2e6907c186c"), "Republic of Singapore", "SG", "SGP", "Singapore", "702" },
                    { new Guid("3e2cccbe-1615-c707-a97b-421a799b2559"), "Republic of Uganda", "UG", "UGA", "Uganda", "800" },
                    { new Guid("3eea06f4-c085-f619-6d52-b76a5f6fd2b6"), "Niue", "NU", "NIU", "Niue", "570" },
                    { new Guid("3ffe68ca-7350-175b-4e95-0c34f54dc1f4"), "Republic of Guinea", "GN", "GIN", "Guinea", "324" },
                    { new Guid("414a34ce-2781-8f96-2bd0-7ada86c8cf38"), "Kingdom of Spain", "ES", "ESP", "Spain", "724" },
                    { new Guid("42697d56-52cf-b411-321e-c51929f02f90"), "Burkina Faso", "BF", "BFA", "Burkina Faso", "854" },
                    { new Guid("44caa0f4-1e78-d2fb-96be-d01b3224bdc1"), "Kingdom of Bahrain", "BH", "BHR", "Bahrain", "048" },
                    { new Guid("46576b73-c05b-7498-5b07-9bbf59b7645d"), "Republic of Bulgaria", "BG", "BGR", "Bulgaria", "100" },
                    { new Guid("46e88019-c521-57b2-d1c0-c0e2478d3b05"), "Commonwealth of the Bahamas", "BS", "BHS", "Bahamas", "044" },
                    { new Guid("46ef1468-86f6-0c99-f4e9-46f966167b05"), "Federal Republic of Germany", "DE", "DEU", "Germany", "276" },
                    { new Guid("4736c1ad-54bd-c8e8-d9ee-492a88268de8"), "United Republic of Tanzania", "TZ", "TZA", "Tanzania", "834" },
                    { new Guid("47804b6a-e705-b925-f4fd-4adf6500180b"), "Norfolk Island", "NF", "NFK", "Norfolk Island", "574" },
                    { new Guid("478786f7-1842-8c1e-921c-12e7ed5329c5"), "Republic of Angola", "AO", "AGO", "Angola", "024" },
                    { new Guid("4826bc0f-235e-572f-2b1a-21f1c9e05f83"), "Gabonese Republic", "GA", "GAB", "Gabon", "266" },
                    { new Guid("49c82f1b-968d-b5e7-8559-e39567d46787"), "Republic of Ecuador", "EC", "ECU", "Ecuador", "218" },
                    { new Guid("4b0729b6-f698-5730-767c-88e2d36691bb"), "New Caledonia", "NC", "NCL", "New Caledonia", "540" },
                    { new Guid("4d8bcda4-5598-16cd-b379-97eb7a5e1c29"), "Republic of El Salvador", "SV", "SLV", "El Salvador", "222" },
                    { new Guid("4ee6400d-5534-7c67-1521-870d6b732366"), "Iceland", "IS", "ISL", "Iceland", "352" },
                    { new Guid("4fc1a9dc-cc74-f6ce-5743-c5cee8d709ef"), "Hellenic Republic of Greece", "GR", "GRC", "Greece", "300" },
                    { new Guid("500bb0de-61f5-dc9b-0488-1c507456ea4d"), "Hong Kong Special Administrative Region of China", "HK", "HKG", "Hong Kong", "344" },
                    { new Guid("50e5954d-7cb4-2201-b96c-f2a846ab3ae3"), "Montserrat", "MS", "MSR", "Montserrat", "500" },
                    { new Guid("51aa4900-30a6-91b7-2728-071542a064ff"), "Romania", "RO", "ROU", "Romania", "642" },
                    { new Guid("52538361-bbdf-fafb-e434-5655fc7451e5"), "Republic of Lithuania", "LT", "LTU", "Lithuania", "440" },
                    { new Guid("5283afbb-2744-e930-2c16-c5ea6b0ff7cc"), "Federative Republic of Brazil", "BR", "BRA", "Brazil", "076" },
                    { new Guid("52d9992c-19bd-82b4-9188-11dabcac6171"), "Bolivarian Republic of Venezuela", "VE", "VEN", "Venezuela", "862" },
                    { new Guid("538114de-7db0-9242-35e6-324fa7eff44d"), "American Samoa", "AS", "ASM", "American Samoa", "016" },
                    { new Guid("5476986b-11a4-8463-9bd7-0f7354ec7a20"), "Saint Pierre and Miquelon", "PM", "SPM", "Saint Pierre and Miquelon", "666" },
                    { new Guid("550ca5df-3995-617c-c39d-437beb400a42"), "Turkmenistan", "TM", "TKM", "Turkmenistan", "795" },
                    { new Guid("57765d87-2424-2c86-ad9c-1af58ef3127a"), "Republic of Cuba", "CU", "CUB", "Cuba", "192" },
                    { new Guid("58337ef3-3d24-43e9-a440-832306e7fc07"), "Russian Federation", "RU", "RUS", "Russian Federation", "643" },
                    { new Guid("592b4658-a210-ab0a-5660-3dcc673dc581"), "Heard Island and McDonald Islands", "HM", "HMD", "Heard Island and McDonald Islands", "334" },
                    { new Guid("5a5d9168-081b-1e02-1fbb-cdfa910e526c"), "Republic of Finland", "FI", "FIN", "Finland", "246" },
                    { new Guid("5aa0aeb7-4dc8-6a29-fc2f-35daec1541dd"), "Republic of Albania", "AL", "ALB", "Albania", "008" },
                    { new Guid("5b0ee3be-596d-bdc1-f101-00ef33170655"), "Bailiwick of Guernsey", "GG", "GGY", "Guernsey", "831" },
                    { new Guid("5be18efe-6db8-a727-7f2a-62bd71bc6593"), "Republic of Cote d'Ivoire", "CI", "CIV", "Cote d'Ivoire", "384" },
                    { new Guid("5c0e654b-8547-5d02-ee7b-d65e3c5c5273"), "Canada", "CA", "CAN", "Canada", "124" },
                    { new Guid("5cab34ca-8c74-0766-c7ca-4a826b44c5bd"), "Principality of Monaco", "MC", "MCO", "Monaco", "492" },
                    { new Guid("5e7a08f2-7d59-bcdb-7ddd-876b87181420"), "Union of the Comoros", "KM", "COM", "Comoros", "174" },
                    { new Guid("61ba1844-4d33-84b4-dbac-70718aa91d59"), "Republic of Suriname", "SR", "SUR", "Suriname", "740" },
                    { new Guid("65d871be-4a1d-a632-9cdb-62e3ff04928d"), "Bailiwick of Jersey", "JE", "JEY", "Jersey", "832" },
                    { new Guid("6699efd5-0939-7812-315e-21f37b279ee9"), "Jamaica", "JM", "JAM", "Jamaica", "388" },
                    { new Guid("687320c8-e841-c911-6d30-b14eb998feb6"), "Democratic Socialist Republic of Sri Lanka", "LK", "LKA", "Sri Lanka", "144" },
                    { new Guid("688af4c8-9d64-ae1c-147f-b8afd54801e3"), "Republic of Armenia", "AM", "ARM", "Armenia", "051" },
                    { new Guid("695c85b3-a6c6-c217-9be8-3baebc7719ce"), "State of Libya", "LY", "LBY", "Libya", "434" },
                    { new Guid("6984f722-6963-d067-d4d4-9fd3ef2edbf6"), "Republic of Zimbabwe", "ZW", "ZWE", "Zimbabwe", "716" },
                    { new Guid("6a76d068-49e1-da80-ddb4-9ef3d11191e6"), "Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", "Saint Helena, Ascension and Tristan da Cunha", "654" },
                    { new Guid("6aac6f0e-d13a-a629-4c2b-9d6eaf6680e4"), "Republic of South Sudan", "SS", "SSD", "South Sudan", "728" },
                    { new Guid("6ac64a20-5688-ccd0-4eca-88d8a2560079"), "Commonwealth of the Northern Mariana Islands", "MP", "MNP", "Northern Mariana Islands", "580" },
                    { new Guid("6af4d03e-edd0-d98a-bc7e-abc7df87d3dd"), "South Georgia and the South Sandwich Islands", "GS", "SGS", "South Georgia and the South Sandwich Islands", "239" },
                    { new Guid("6c366974-3672-3a2c-2345-0fda33942304"), "Sultanate of Oman", "OM", "OMN", "Oman", "512" },
                    { new Guid("6c8be2e6-8c2e-cd80-68a6-d18c80d0eedc"), "Republic of Iraq", "IQ", "IRQ", "Iraq", "368" },
                    { new Guid("6d0c77a7-a4aa-c2bd-2db6-0e2ad2d61f8a"), "Republic of Ghana", "GH", "GHA", "Ghana", "288" },
                    { new Guid("704254eb-6959-8ddc-a5df-ac8f9658dc68"), "Republic of Austria", "AT", "AUT", "Austria", "040" },
                    { new Guid("70673250-4cc3-3ba1-a42c-6b62ea8ab1d5"), "Grand Duchy of Luxembourg", "LU", "LUX", "Luxembourg", "442" },
                    { new Guid("72d8d1fe-d5f6-f440-1185-82ec69427027"), "Republic of India", "IN", "IND", "India", "356" },
                    { new Guid("7453c201-ecf1-d3dd-0409-e94d0733173b"), "Solomon Islands", "SB", "SLB", "Solomon Islands", "090" },
                    { new Guid("74da982f-cf20-e1b4-517b-a040511af23c"), "Islamic Republic of Mauritania", "MR", "MRT", "Mauritania", "478" },
                    { new Guid("75634729-8e4a-4cfd-739d-9f679bfca3ab"), "Republic of Peru", "PE", "PER", "Peru", "604" },
                    { new Guid("75e4464b-a784-63b8-1ecc-69ee1f09f43f"), "Republic of Burundi", "BI", "BDI", "Burundi", "108" },
                    { new Guid("766c1ebb-78c1-bada-37fb-c45d1bd4baff"), "Democratic Republic of Sao Tome and Principe", "ST", "STP", "Sao Tome and Principe", "678" },
                    { new Guid("77f6f69b-ec41-8818-9395-8d39bf09e653"), "Saint Barthélemy", "BL", "BLM", "Saint Barthélemy", "652" },
                    { new Guid("7bbf15f4-a907-c0b2-7029-144aafb3c59d"), "Republic of Italy", "IT", "ITA", "Italy", "380" },
                    { new Guid("7bf4a786-3733-c670-e85f-03ee3caa6ef9"), "Republic of Panama", "PA", "PAN", "Panama", "591" },
                    { new Guid("7bf934fa-bcf4-80b5-fd7d-ab4cca45c67b"), "Republic of Korea", "KR", "KOR", "Korea", "410" },
                    { new Guid("7ffa909b-8a6a-3028-9589-fcc3dfa530a8"), "State of Israel", "IL", "ISR", "Israel", "376" },
                    { new Guid("802c05db-3866-545d-dc1a-a02c83ea6cf6"), "Federal Republic of Somalia", "SO", "SOM", "Somalia", "706" },
                    { new Guid("809c3424-8654-b82c-cbd4-d857d096943e"), "People's Republic of Bangladesh", "BD", "BGD", "Bangladesh", "050" },
                    { new Guid("824392e8-a6cc-0cd4-af13-3067dad3258e"), "Republic of Equatorial Guinea", "GQ", "GNQ", "Equatorial Guinea", "226" },
                    { new Guid("8250c49f-9438-7c2e-f403-54d962db0c18"), "People's Republic of China", "CN", "CHN", "China", "156" },
                    { new Guid("84d58b3d-d131-1506-0792-1b3228b6f71f"), "Kingdom of Thailand", "TH", "THA", "Thailand", "764" },
                    { new Guid("86db2170-be87-fd1d-bf57-05ff61ae83a7"), "Montenegro", "ME", "MNE", "Montenegro", "499" },
                    { new Guid("875060ca-73f6-af3b-d844-1b1416ce4583"), "Taiwan, Province of China", "TW", "TWN", "Taiwan", "158" },
                    { new Guid("881b4bb8-b6da-c73e-55c0-c9f31c02aaef"), "Réunion", "RE", "REU", "Réunion", "638" },
                    { new Guid("899c2a9f-f35d-5a49-a6cd-f92531bb2266"), "Saint Martin (French part)", "MF", "MAF", "Saint Martin", "663" },
                    { new Guid("8a4fcb23-f3e6-fb5b-8cda-975872f600d5"), "Kingdom of Denmark", "DK", "DNK", "Denmark", "208" },
                    { new Guid("8b5a477a-070a-a84f-bd3b-f54dc2a172de"), "State of Eritrea", "ER", "ERI", "Eritrea", "232" },
                    { new Guid("8c4441fd-8cd4-ff1e-928e-e46f9ca12552"), "Yemen", "YE", "YEM", "Yemen", "887" },
                    { new Guid("8d32a12d-3230-1431-8fbb-72c789184345"), "Macao Special Administrative Region of China", "MO", "MAC", "Macao", "446" },
                    { new Guid("8e0de349-f9ab-2bca-3910-efd48bf1170a"), "Gibraltar", "GI", "GIB", "Gibraltar", "292" },
                    { new Guid("8e787470-aae6-575a-fe0b-d65fc78b648a"), "Eastern Republic of Uruguay", "UY", "URY", "Uruguay", "858" },
                    { new Guid("8ed6a34e-8135-27fa-f86a-caa247b29768"), "Kingdom of Bhutan", "BT", "BTN", "Bhutan", "064" },
                    { new Guid("903bee63-bcf0-0264-6eaf-a8cde95c5f41"), "French Southern Territories", "TF", "ATF", "French Southern Territories", "260" },
                    { new Guid("914618fd-86f9-827a-91b8-826f0db9e02d"), "Republic of Kiribati", "KI", "KIR", "Kiribati", "296" },
                    { new Guid("914d7923-3ac5-75e8-c8e2-47d72561e35d"), "Kingdom of Norway", "NO", "NOR", "Norway", "578" },
                    { new Guid("915805f0-9ff0-48ff-39b3-44a4af5e0482"), "Kingdom of Morocco", "MA", "MAR", "Morocco", "504" },
                    { new Guid("9205dbfc-60cd-91d9-b0b8-8a18a3755286"), "Republic of Latvia", "LV", "LVA", "Latvia", "428" },
                    { new Guid("943d2419-2ca6-95f8-9c3b-ed445aea0371"), "Republic of the Marshall Islands", "MH", "MHL", "Marshall Islands", "584" },
                    { new Guid("95467997-f989-f456-34b7-0b578302dcba"), "Republic of Trinidad and Tobago", "TT", "TTO", "Trinidad and Tobago", "780" },
                    { new Guid("96a22cee-9af7-8f03-b483-b3e774a36d3b"), "Republic of Benin", "BJ", "BEN", "Benin", "204" },
                    { new Guid("971c7e66-c6e3-71f4-580a-5caf2852f9f4"), "Republic of Serbia", "RS", "SRB", "Serbia", "688" },
                    { new Guid("976e496f-ca38-d113-1697-8af2d9a3b159"), "Republic of Madagascar", "MG", "MDG", "Madagascar", "450" },
                    { new Guid("97cd39d5-1aca-8f10-9f5e-3f611d7606d8"), "Republic of Niger", "NE", "NER", "Niger", "562" },
                    { new Guid("980176e8-7d9d-9729-b3e9-ebc455fb8fc4"), "Georgia", "GE", "GEO", "Georgia", "268" },
                    { new Guid("9ae7ad80-9ce7-6657-75cf-28b4c0254238"), "Hashemite Kingdom of Jordan", "JO", "JOR", "Jordan", "400" },
                    { new Guid("9d4ec95b-974a-f5bb-bb4b-ba6747440631"), "Czech Republic", "CZ", "CZE", "Czechia", "203" },
                    { new Guid("9d6e6446-185e-235e-8771-9eb2d19f22e7"), "Principality of Liechtenstein", "LI", "LIE", "Liechtenstein", "438" },
                    { new Guid("9dacf00b-7d0a-d744-cc60-e5fa66371e9d"), "Togolese Republic", "TG", "TGO", "Togo", "768" },
                    { new Guid("9e7dbdc3-2c8b-e8ae-082b-e02695f8268e"), "Kingdom of Tonga", "TO", "TON", "Tonga", "776" },
                    { new Guid("a0098040-b7a0-59a1-e64b-0a9778b7f74c"), "Antarctica (the territory South of 60 deg S)", "AQ", "ATA", "Antarctica", "010" },
                    { new Guid("a16263a5-810c-bf6a-206d-72cb914e2d5c"), "Cocos (Keeling) Islands", "CC", "CCK", "Cocos (Keeling) Islands", "166" },
                    { new Guid("a1b83be0-6a9b-c8a9-2cce-531705a29664"), "Isle of Man", "IM", "IMN", "Isle of Man", "833" },
                    { new Guid("a2da72dc-5866-ba2f-6283-6575af00ade5"), "Federated States of Micronesia", "FM", "FSM", "Micronesia", "583" },
                    { new Guid("a32a9fc2-677f-43e0-97aa-9e83943d785c"), "Kingdom of Eswatini", "SZ", "SWZ", "Eswatini", "748" },
                    { new Guid("a40b91b3-cc13-2470-65f0-a0fdc946f2a2"), "Republic of the Gambia", "GM", "GMB", "Gambia", "270" },
                    { new Guid("a5d0c9af-2022-2b43-9332-eb6a2ce4305d"), "Pitcairn Islands", "PN", "PCN", "Pitcairn Islands", "612" },
                    { new Guid("a7716d29-6ef6-b775-51c5-97094536329d"), "Bosnia and Herzegovina", "BA", "BIH", "Bosnia and Herzegovina", "070" },
                    { new Guid("a7afb7b1-b26d-4571-1a1f-3fff738ff21e"), "Argentine Republic", "AR", "ARG", "Argentina", "032" },
                    { new Guid("a7c4c9db-8fe4-7d43-e830-1a70954970c3"), "Independent State of Samoa", "WS", "WSM", "Samoa", "882" },
                    { new Guid("a8f30b36-4a25-3fb9-c69e-84ce6640d785"), "Kingdom of Saudi Arabia", "SA", "SAU", "Saudi Arabia", "682" },
                    { new Guid("a96fe9bb-4ef4-fca0-f38b-0ec729822f37"), "Åland Islands", "AX", "ALA", "Åland Islands", "248" },
                    { new Guid("a9940e91-93ef-19f7-79c0-00d31c6a9f87"), "United Mexican States", "MX", "MEX", "Mexico", "484" },
                    { new Guid("a9949ac7-8d2d-32b5-3f4f-e2a3ef291a67"), "Co-operative Republic of Guyana", "GY", "GUY", "Guyana", "328" },
                    { new Guid("a9a5f440-a9bd-487d-e7f4-914df0d52fa6"), "Republic of Guinea-Bissau", "GW", "GNB", "Guinea-Bissau", "624" },
                    { new Guid("aa0f69b2-93aa-ec51-b43b-60145db79e38"), "Republic of North Macedonia", "MK", "MKD", "North Macedonia", "807" },
                    { new Guid("ab0b7e83-bf02-16e6-e5ae-46c4bd4c093b"), "Republic of Zambia", "ZM", "ZMB", "Zambia", "894" },
                    { new Guid("ac6cde6e-f645-d04e-8afc-0391ecf38a70"), "French Guiana", "GF", "GUF", "French Guiana", "254" },
                    { new Guid("ad4f938a-bf7b-684b-2c9e-e824d3fa3863"), "Republic of Chile", "CL", "CHL", "Chile", "152" },
                    { new Guid("af79558d-51fb-b08d-185b-afeb983ab99b"), "Cook Islands", "CK", "COK", "Cook Islands", "184" },
                    { new Guid("b0f4bdfa-17dd-9714-4fe8-3c3b1f010ffa"), "Republic of Sierra Leone", "SL", "SLE", "Sierra Leone", "694" },
                    { new Guid("b2261c50-1a57-7f1f-d72d-f8c21593874f"), "French Republic", "FR", "FRA", "France", "250" },
                    { new Guid("b2c4d2d7-7ada-7864-426f-10a28d9f9eba"), "Dominican Republic", "DO", "DOM", "Dominican Republic", "214" },
                    { new Guid("b32fe2b5-a06e-0d76-ffd2-f186c3e64b15"), "Republic of Kenya", "KE", "KEN", "Kenya", "404" },
                    { new Guid("b3460bab-2a35-57bc-17e2-4e117748bbb1"), "Islamic Republic of Iran", "IR", "IRN", "Iran", "364" },
                    { new Guid("b4e0625c-7597-c185-b8ae-cfb35a731f2f"), "Central African Republic", "CF", "CAF", "Central African Republic", "140" },
                    { new Guid("b6f70436-9515-7ef8-af57-aad196503499"), "State of Kuwait", "KW", "KWT", "Kuwait", "414" },
                    { new Guid("b723594d-7800-0f37-db86-0f6b85bb6cf9"), "Republic of Kazakhstan", "KZ", "KAZ", "Kazakhstan", "398" },
                    { new Guid("b86375dc-edbb-922c-9ed4-2f724094a5a2"), "Falkland Islands (Malvinas)", "FK", "FLK", "Falkland Islands (Malvinas)", "238" },
                    { new Guid("b8b09512-ea4c-4a61-9331-304f55324ef7"), "British Indian Ocean Territory (Chagos Archipelago)", "IO", "IOT", "British Indian Ocean Territory (Chagos Archipelago)", "086" },
                    { new Guid("bd4bbfc7-d8bc-9d8d-7f7c-7b299c94e9e5"), "Principality of Andorra", "AD", "AND", "Andorra", "020" },
                    { new Guid("bf210ee6-6c75-cf08-052e-5c3e608aed15"), "Kingdom of Lesotho", "LS", "LSO", "Lesotho", "426" },
                    { new Guid("c03d71a5-b215-8672-ec0c-dd8fe5c20e05"), "Republic of Mali", "ML", "MLI", "Mali", "466" },
                    { new Guid("c0b7e39e-223a-ebb0-b899-5404573bbdb7"), "Republic of Cameroon", "CM", "CMR", "Cameroon", "120" },
                    { new Guid("c1a923f6-b9ec-78f7-cc1c-7025e3d69d7d"), "Syrian Arab Republic", "SY", "SYR", "Syrian Arab Republic", "760" },
                    { new Guid("c4754c00-cfa5-aa6f-a9c8-a200457de7a8"), "Lao People's Democratic Republic", "LA", "LAO", "Lao People's Democratic Republic", "418" },
                    { new Guid("c522b3d3-74cc-846f-0394-737dff4d2b1a"), "Mongolia", "MN", "MNG", "Mongolia", "496" },
                    { new Guid("c64288fc-d941-0615-47f9-28e6c294ce26"), "Republic of Colombia", "CO", "COL", "Colombia", "170" },
                    { new Guid("c89e02a0-9506-90df-5545-b98a2453cd63"), "Belize", "BZ", "BLZ", "Belize", "084" },
                    { new Guid("c926f091-fe96-35b3-56b5-d418d17e0159"), "Independent State of Papua New Guinea", "PG", "PNG", "Papua New Guinea", "598" },
                    { new Guid("c93bccaf-1835-3c02-e2ee-c113ced19e43"), "Republic of the Philippines", "PH", "PHL", "Philippines", "608" },
                    { new Guid("c9702851-1f67-f2a6-89d4-37b3fbb12044"), "Kingdom of Cambodia", "KH", "KHM", "Cambodia", "116" },
                    { new Guid("c98174ef-8198-54ba-2ff1-b93f3c646db8"), "Republic of Vanuatu", "VU", "VUT", "Vanuatu", "548" },
                    { new Guid("ca2a5560-d4c4-3c87-3090-6f5436310b55"), "Bermuda", "BM", "BMU", "Bermuda", "060" },
                    { new Guid("cb2e209b-d4c6-6d5c-8901-d989a9188783"), "United States of America", "US", "USA", "United States of America", "840" },
                    { new Guid("cc7fabfc-4c2b-d9ff-bb45-003bfc2e468a"), "Islamic Republic of Pakistan", "PK", "PAK", "Pakistan", "586" },
                    { new Guid("cd0e8275-3def-1de4-8858-61aab36851c4"), "Republic of Nicaragua", "NI", "NIC", "Nicaragua", "558" },
                    { new Guid("cd2c97c3-5473-0719-3803-fcacedfe2ea2"), "Commonwealth of Puerto Rico", "PR", "PRI", "Puerto Rico", "630" },
                    { new Guid("cfff3443-1378-9c7d-9d58-66146d7f29a6"), "Kingdom of the Netherlands", "NL", "NLD", "Netherlands", "528" },
                    { new Guid("d0e11a85-6623-69f5-bd95-3779dfeec297"), "Holy See (Vatican City State)", "VA", "VAT", "Holy See (Vatican City State)", "336" },
                    { new Guid("d13935c1-8956-1399-7c4e-0354795cd37b"), "Republic of Costa Rica", "CR", "CRI", "Costa Rica", "188" },
                    { new Guid("d24b46ba-8e9d-2a09-7995-e35e8ae54f6b"), "Republic of Guatemala", "GT", "GTM", "Guatemala", "320" },
                    { new Guid("d292ea2d-fbb6-7c1e-cb7d-23d552673776"), "Malaysia", "MY", "MYS", "Malaysia", "458" },
                    { new Guid("d525de3a-aecc-07de-0426-68f32af2968e"), "Svalbard & Jan Mayen Islands", "SJ", "SJM", "Svalbard & Jan Mayen Islands", "744" },
                    { new Guid("d6d31cdd-280a-56bc-24a4-a414028d2b67"), "State of Palestine", "PS", "PSE", "Palestine", "275" },
                    { new Guid("d7236157-d5a7-6b7a-3bc1-69802313fa30"), "Socialist Republic of Vietnam", "VN", "VNM", "Vietnam", "704" },
                    { new Guid("d8101f9d-8313-4054-c5f3-42c7a1c72862"), "Bonaire, Sint Eustatius and Saba", "BQ", "BES", "Bonaire, Sint Eustatius and Saba", "535" },
                    { new Guid("d97b5460-11ab-45c5-9a6f-ffa441ed70d6"), "Republic of Belarus", "BY", "BLR", "Belarus", "112" },
                    { new Guid("daf6bc7a-92c4-ef47-3111-e13199b86b90"), "Republic of Moldova", "MD", "MDA", "Moldova", "498" },
                    { new Guid("db6ce903-ab43-3793-960c-659529bae6df"), "Republic of Paraguay", "PY", "PRY", "Paraguay", "600" },
                    { new Guid("dcf19e1d-74a6-7b8b-a5ed-76b94a8ac2a7"), "Hungary", "HU", "HUN", "Hungary", "348" },
                    { new Guid("de503629-2607-b948-e279-0509d8109d0f"), "Republic of Poland", "PL", "POL", "Poland", "616" },
                    { new Guid("df20d0d7-9fbe-e725-d966-4fdf9f5c9dfb"), "Republic of Cyprus", "CY", "CYP", "Cyprus", "196" },
                    { new Guid("e087f51c-feba-19b6-5595-fcbdce170411"), "Ukraine", "UA", "UKR", "Ukraine", "804" },
                    { new Guid("e0d562ca-f573-3c2f-eb83-f72d4d70d4fc"), "Tuvalu", "TV", "TUV", "Tuvalu", "798" },
                    { new Guid("e186a953-7ab3-c009-501c-a754267b770b"), "Wallis and Futuna", "WF", "WLF", "Wallis and Futuna", "876" },
                    { new Guid("e1947bdc-ff2c-d2c1-3c55-f1f9bf778578"), "United States Virgin Islands", "VI", "VIR", "United States Virgin Islands", "850" },
                    { new Guid("e3bacefb-d79b-1569-a91c-43d7e4f6f230"), "Republic of Nauru", "NR", "NRU", "Nauru", "520" },
                    { new Guid("e6c7651f-182e-cf9c-1ef9-6293b95b500c"), "Aruba", "AW", "ABW", "Aruba", "533" },
                    { new Guid("e75515a6-63cf-3612-a3a2-befa0d7048a7"), "Federal Democratic Republic of Ethiopia", "ET", "ETH", "Ethiopia", "231" },
                    { new Guid("e81c5db3-401a-e047-001e-045f39bef8ef"), "Nepal", "NP", "NPL", "Nepal", "524" },
                    { new Guid("ebf38b9a-6fbe-6e82-3977-2c4763bea072"), "Republic of South Africa", "ZA", "ZAF", "South Africa", "710" },
                    { new Guid("ed6278e0-436c-9fd9-0b9e-44fd424cbd1b"), "Brunei Darussalam", "BN", "BRN", "Brunei Darussalam", "096" },
                    { new Guid("edd4319b-86f3-24cb-248c-71da624c02f7"), "Islamic Republic of Afghanistan", "AF", "AFG", "Afghanistan", "004" },
                    { new Guid("ee5dfc29-80f1-86ae-cde7-02484a18907a"), "Arab Republic of Egypt", "EG", "EGY", "Egypt", "818" },
                    { new Guid("ee926d09-799c-7c6a-2419-a6ff814b2c03"), "Republic of Liberia", "LR", "LBR", "Liberia", "430" },
                    { new Guid("f0219540-8b2c-bd29-4f76-b832de53a56f"), "Republic of Malta", "MT", "MLT", "Malta", "470" },
                    { new Guid("f0965449-6b15-6c1a-f5cb-ebd2d575c02c"), "Republic of Sudan", "SD", "SDN", "Sudan", "729" },
                    { new Guid("f33ced84-eb43-fb39-ef79-b266e4d4cd94"), "Plurinational State of Bolivia", "BO", "BOL", "Bolivia", "068" },
                    { new Guid("f39cca22-449e-9866-3a65-465a5510483e"), "Republic of Türkiye", "TR", "TUR", "Türkiye", "792" },
                    { new Guid("f3eef99a-661e-2c68-7a4c-3053e2f28007"), "Antigua and Barbuda", "AG", "ATG", "Antigua and Barbuda", "028" },
                    { new Guid("f5b15ea6-133d-c2c9-7ef9-b0916ea96edb"), "Republic of Rwanda", "RW", "RWA", "Rwanda", "646" },
                    { new Guid("f70ae426-f130-5637-0383-a5b63a06c500"), "Democratic People's Republic of Korea", "KP", "PRK", "Korea", "408" },
                    { new Guid("fa633273-9866-840d-9739-c6c957901e46"), "Federation of Saint Kitts and Nevis", "KN", "KNA", "Saint Kitts and Nevis", "659" },
                    { new Guid("fb9a713c-2de1-882a-64b7-0e8fef5d2f7e"), "Democratic Republic of Timor-Leste", "TL", "TLS", "Timor-Leste", "626" },
                    { new Guid("fbf4479d-d70d-c76e-b053-699362443a17"), "Republic of Malawi", "MW", "MWI", "Malawi", "454" },
                    { new Guid("fc78fa89-b372-dcf7-7f1c-1e1bb14ecbe7"), "Martinique", "MQ", "MTQ", "Martinique", "474" },
                    { new Guid("fee6f04f-c4c1-e3e4-645d-bb6bb703aeb7"), "People's Democratic Republic of Algeria", "DZ", "DZA", "Algeria", "012" },
                    { new Guid("ff5b4d88-c179-ff0d-6285-cf46ba475d7d"), "Grenada", "GD", "GRD", "Grenada", "308" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("015a9f83-6e57-bc1e-8227-24a4e5248582"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("057884bc-3c2e-dea9-6522-b003c9297f7a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("067c9448-9ad0-2c21-a1dc-fbdf5a63d18d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("06f8ad57-7133-9a5e-5a83-53052012b014"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0797a7d5-bbc0-2e52-0de8-14a42fc80baa"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0868cdd3-7f50-5a25-88d6-98c45f9157e3"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("08a999e4-e420-b864-2864-bef78c138448"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0932ed88-c79f-591a-d684-9a77735f947e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("096a8586-9702-6fec-5f6a-6eb3b7b7837f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0a25f96f-5173-2fff-a2f8-c6872393edf6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0ab731f0-5326-44be-af3a-20aa33ad0f35"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0aebadaa-91b2-8794-c153-4f903a2a1004"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0b3b04b4-9782-79e3-bc55-9ab33b6ae9c7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0c0fef20-0e8d-98ea-7724-12cea9b3b926"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0d4fe6e6-ea1e-d1ce-5134-6c0c1a696a00"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0e0fefd5-9a05-fde5-bee9-ef56db7748a1"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0e2a1681-d852-67ae-7387-0d04be9e7fd3"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("0f1ba59e-ade5-23e5-6fce-e2fd3282e114"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("10b58d9b-42ef-edb8-54a3-712636fda55a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("11765ad0-30f2-bab8-b616-20f88b28b21e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("11dbce82-a154-7aee-7b5e-d5981f220572"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1258ec90-c47e-ff72-b7e3-f90c3ee320f8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("13c69e56-375d-8a7e-c326-be2be2fd4cd8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("141e589a-7046-a265-d2f6-b2f85e6eeadd"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("14f190c6-97c9-3e12-2eba-db17c59d6a04"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("15639386-e4fc-120c-6916-c0c980e24be1"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("17ed5f0f-e091-94ff-0512-ad291bde94d7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1934954c-66c2-6226-c5b6-491065a3e4c0"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("19ea3a6a-1a76-23c8-8e4e-1d298f15207f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1b634ca2-2b90-7e54-715a-74cee7e4d294"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1d2aa3ab-e1c3-8c76-9be6-7a3b3eca35da"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1d974338-decf-08e5-3e62-89e1bbdbb003"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1e5c0dcc-83e9-f275-c81d-3bc49f88e70c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("1f8be615-5746-277e-d82b-47596b5bb922"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2167da32-4f80-d31d-226c-0551970304eb"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("220e980a-7363-0150-c250-89e83b967fb4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("29201cbb-ca65-1924-75a9-0c4d4db43001"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("294978f0-2702-d35d-cfc4-e676148aea2e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2a039b16-2adf-0fb8-3bdf-fbdf14358d9d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2a1ca5b6-fba0-cfa8-9928-d7a2382bc4d7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2a848549-9777-cf48-a0f2-b32c6f942096"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2b68fb11-a0e0-3d23-5fb8-99721ecfc182"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2bebebe4-edaa-9160-5a0c-4d99048bd8d5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2dc643bd-cc6c-eb0c-7314-44123576f0ee"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2e1bd9d8-df06-d773-0eb9-98e274b63b43"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2f00fe86-a06b-dc95-0ea7-4520d1dec784"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2f49855b-ff93-c399-d72a-121f2bf28bc9"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("2f4cc994-53f1-1763-8220-5d89e063804f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("316c68fc-9144-f6e1-8bf1-899fc54b2327"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3175ac19-c801-0b87-8e66-7480a40dcf1e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3252e51a-5bc1-f065-7101-5b34ba493dc4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("32da0208-9048-1339-a8ee-6955cfff4c12"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3345e205-3e72-43ed-de1b-ac6e050543e5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("357369e3-85a8-86f7-91c7-349772ae7744"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("357c121b-e28d-1765-e699-cc4ec5ff86fc"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("360e3c61-aaac-fa2f-d731-fc0824c05107"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("37a79267-d38a-aaef-577a-aa68a96880ae"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("37c89068-a8e9-87e8-d651-f86fac63673a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("39be5e86-aea5-f64f-fd7e-1017fe24e543"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3bcd2aad-fb69-09f4-1ad7-2c7f5fa23f9f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3c5828e0-16a8-79ba-4e5c-9b45065df113"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3ce3d958-7341-bd79-f294-f2e6907c186c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3e2cccbe-1615-c707-a97b-421a799b2559"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3eea06f4-c085-f619-6d52-b76a5f6fd2b6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("3ffe68ca-7350-175b-4e95-0c34f54dc1f4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("414a34ce-2781-8f96-2bd0-7ada86c8cf38"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("42697d56-52cf-b411-321e-c51929f02f90"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("44caa0f4-1e78-d2fb-96be-d01b3224bdc1"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("46576b73-c05b-7498-5b07-9bbf59b7645d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("46e88019-c521-57b2-d1c0-c0e2478d3b05"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("46ef1468-86f6-0c99-f4e9-46f966167b05"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4736c1ad-54bd-c8e8-d9ee-492a88268de8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("47804b6a-e705-b925-f4fd-4adf6500180b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("478786f7-1842-8c1e-921c-12e7ed5329c5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4826bc0f-235e-572f-2b1a-21f1c9e05f83"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("49c82f1b-968d-b5e7-8559-e39567d46787"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4b0729b6-f698-5730-767c-88e2d36691bb"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4d8bcda4-5598-16cd-b379-97eb7a5e1c29"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4ee6400d-5534-7c67-1521-870d6b732366"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("4fc1a9dc-cc74-f6ce-5743-c5cee8d709ef"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("500bb0de-61f5-dc9b-0488-1c507456ea4d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("50e5954d-7cb4-2201-b96c-f2a846ab3ae3"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("51aa4900-30a6-91b7-2728-071542a064ff"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("52538361-bbdf-fafb-e434-5655fc7451e5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5283afbb-2744-e930-2c16-c5ea6b0ff7cc"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("52d9992c-19bd-82b4-9188-11dabcac6171"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("538114de-7db0-9242-35e6-324fa7eff44d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5476986b-11a4-8463-9bd7-0f7354ec7a20"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("550ca5df-3995-617c-c39d-437beb400a42"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("57765d87-2424-2c86-ad9c-1af58ef3127a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("58337ef3-3d24-43e9-a440-832306e7fc07"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("592b4658-a210-ab0a-5660-3dcc673dc581"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5a5d9168-081b-1e02-1fbb-cdfa910e526c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5aa0aeb7-4dc8-6a29-fc2f-35daec1541dd"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5b0ee3be-596d-bdc1-f101-00ef33170655"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5be18efe-6db8-a727-7f2a-62bd71bc6593"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5c0e654b-8547-5d02-ee7b-d65e3c5c5273"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5cab34ca-8c74-0766-c7ca-4a826b44c5bd"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5e7a08f2-7d59-bcdb-7ddd-876b87181420"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("61ba1844-4d33-84b4-dbac-70718aa91d59"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("65d871be-4a1d-a632-9cdb-62e3ff04928d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6699efd5-0939-7812-315e-21f37b279ee9"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("687320c8-e841-c911-6d30-b14eb998feb6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("688af4c8-9d64-ae1c-147f-b8afd54801e3"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("695c85b3-a6c6-c217-9be8-3baebc7719ce"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6984f722-6963-d067-d4d4-9fd3ef2edbf6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6a76d068-49e1-da80-ddb4-9ef3d11191e6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6aac6f0e-d13a-a629-4c2b-9d6eaf6680e4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6ac64a20-5688-ccd0-4eca-88d8a2560079"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6af4d03e-edd0-d98a-bc7e-abc7df87d3dd"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6c366974-3672-3a2c-2345-0fda33942304"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6c8be2e6-8c2e-cd80-68a6-d18c80d0eedc"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("6d0c77a7-a4aa-c2bd-2db6-0e2ad2d61f8a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("704254eb-6959-8ddc-a5df-ac8f9658dc68"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("70673250-4cc3-3ba1-a42c-6b62ea8ab1d5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("72d8d1fe-d5f6-f440-1185-82ec69427027"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7453c201-ecf1-d3dd-0409-e94d0733173b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("74da982f-cf20-e1b4-517b-a040511af23c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("75634729-8e4a-4cfd-739d-9f679bfca3ab"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("75e4464b-a784-63b8-1ecc-69ee1f09f43f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("766c1ebb-78c1-bada-37fb-c45d1bd4baff"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("77f6f69b-ec41-8818-9395-8d39bf09e653"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7bbf15f4-a907-c0b2-7029-144aafb3c59d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7bf4a786-3733-c670-e85f-03ee3caa6ef9"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7bf934fa-bcf4-80b5-fd7d-ab4cca45c67b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("7ffa909b-8a6a-3028-9589-fcc3dfa530a8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("802c05db-3866-545d-dc1a-a02c83ea6cf6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("809c3424-8654-b82c-cbd4-d857d096943e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("824392e8-a6cc-0cd4-af13-3067dad3258e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8250c49f-9438-7c2e-f403-54d962db0c18"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("84d58b3d-d131-1506-0792-1b3228b6f71f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("86db2170-be87-fd1d-bf57-05ff61ae83a7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("875060ca-73f6-af3b-d844-1b1416ce4583"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("881b4bb8-b6da-c73e-55c0-c9f31c02aaef"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("899c2a9f-f35d-5a49-a6cd-f92531bb2266"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8a4fcb23-f3e6-fb5b-8cda-975872f600d5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8b5a477a-070a-a84f-bd3b-f54dc2a172de"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8c4441fd-8cd4-ff1e-928e-e46f9ca12552"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8d32a12d-3230-1431-8fbb-72c789184345"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8e0de349-f9ab-2bca-3910-efd48bf1170a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8e787470-aae6-575a-fe0b-d65fc78b648a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("8ed6a34e-8135-27fa-f86a-caa247b29768"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("903bee63-bcf0-0264-6eaf-a8cde95c5f41"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("914618fd-86f9-827a-91b8-826f0db9e02d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("914d7923-3ac5-75e8-c8e2-47d72561e35d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("915805f0-9ff0-48ff-39b3-44a4af5e0482"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9205dbfc-60cd-91d9-b0b8-8a18a3755286"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("943d2419-2ca6-95f8-9c3b-ed445aea0371"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("95467997-f989-f456-34b7-0b578302dcba"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("96a22cee-9af7-8f03-b483-b3e774a36d3b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("971c7e66-c6e3-71f4-580a-5caf2852f9f4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("976e496f-ca38-d113-1697-8af2d9a3b159"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("97cd39d5-1aca-8f10-9f5e-3f611d7606d8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("980176e8-7d9d-9729-b3e9-ebc455fb8fc4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9ae7ad80-9ce7-6657-75cf-28b4c0254238"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9d4ec95b-974a-f5bb-bb4b-ba6747440631"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9d6e6446-185e-235e-8771-9eb2d19f22e7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9dacf00b-7d0a-d744-cc60-e5fa66371e9d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("9e7dbdc3-2c8b-e8ae-082b-e02695f8268e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a0098040-b7a0-59a1-e64b-0a9778b7f74c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a16263a5-810c-bf6a-206d-72cb914e2d5c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a1b83be0-6a9b-c8a9-2cce-531705a29664"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a2da72dc-5866-ba2f-6283-6575af00ade5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a32a9fc2-677f-43e0-97aa-9e83943d785c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a40b91b3-cc13-2470-65f0-a0fdc946f2a2"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a5d0c9af-2022-2b43-9332-eb6a2ce4305d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a7716d29-6ef6-b775-51c5-97094536329d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a7afb7b1-b26d-4571-1a1f-3fff738ff21e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a7c4c9db-8fe4-7d43-e830-1a70954970c3"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a8f30b36-4a25-3fb9-c69e-84ce6640d785"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a96fe9bb-4ef4-fca0-f38b-0ec729822f37"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a9940e91-93ef-19f7-79c0-00d31c6a9f87"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a9949ac7-8d2d-32b5-3f4f-e2a3ef291a67"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("a9a5f440-a9bd-487d-e7f4-914df0d52fa6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("aa0f69b2-93aa-ec51-b43b-60145db79e38"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ab0b7e83-bf02-16e6-e5ae-46c4bd4c093b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ac6cde6e-f645-d04e-8afc-0391ecf38a70"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ad4f938a-bf7b-684b-2c9e-e824d3fa3863"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("af79558d-51fb-b08d-185b-afeb983ab99b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b0f4bdfa-17dd-9714-4fe8-3c3b1f010ffa"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b2261c50-1a57-7f1f-d72d-f8c21593874f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b2c4d2d7-7ada-7864-426f-10a28d9f9eba"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b32fe2b5-a06e-0d76-ffd2-f186c3e64b15"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b3460bab-2a35-57bc-17e2-4e117748bbb1"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b4e0625c-7597-c185-b8ae-cfb35a731f2f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b6f70436-9515-7ef8-af57-aad196503499"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b723594d-7800-0f37-db86-0f6b85bb6cf9"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b86375dc-edbb-922c-9ed4-2f724094a5a2"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("b8b09512-ea4c-4a61-9331-304f55324ef7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("bd4bbfc7-d8bc-9d8d-7f7c-7b299c94e9e5"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("bf210ee6-6c75-cf08-052e-5c3e608aed15"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c03d71a5-b215-8672-ec0c-dd8fe5c20e05"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c0b7e39e-223a-ebb0-b899-5404573bbdb7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c1a923f6-b9ec-78f7-cc1c-7025e3d69d7d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c4754c00-cfa5-aa6f-a9c8-a200457de7a8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c522b3d3-74cc-846f-0394-737dff4d2b1a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c64288fc-d941-0615-47f9-28e6c294ce26"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c89e02a0-9506-90df-5545-b98a2453cd63"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c926f091-fe96-35b3-56b5-d418d17e0159"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c93bccaf-1835-3c02-e2ee-c113ced19e43"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c9702851-1f67-f2a6-89d4-37b3fbb12044"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("c98174ef-8198-54ba-2ff1-b93f3c646db8"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ca2a5560-d4c4-3c87-3090-6f5436310b55"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cb2e209b-d4c6-6d5c-8901-d989a9188783"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cc7fabfc-4c2b-d9ff-bb45-003bfc2e468a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cd0e8275-3def-1de4-8858-61aab36851c4"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cd2c97c3-5473-0719-3803-fcacedfe2ea2"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("cfff3443-1378-9c7d-9d58-66146d7f29a6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d0e11a85-6623-69f5-bd95-3779dfeec297"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d13935c1-8956-1399-7c4e-0354795cd37b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d24b46ba-8e9d-2a09-7995-e35e8ae54f6b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d292ea2d-fbb6-7c1e-cb7d-23d552673776"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d525de3a-aecc-07de-0426-68f32af2968e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d6d31cdd-280a-56bc-24a4-a414028d2b67"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d7236157-d5a7-6b7a-3bc1-69802313fa30"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d8101f9d-8313-4054-c5f3-42c7a1c72862"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d97b5460-11ab-45c5-9a6f-ffa441ed70d6"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("daf6bc7a-92c4-ef47-3111-e13199b86b90"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("db6ce903-ab43-3793-960c-659529bae6df"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("dcf19e1d-74a6-7b8b-a5ed-76b94a8ac2a7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("de503629-2607-b948-e279-0509d8109d0f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("df20d0d7-9fbe-e725-d966-4fdf9f5c9dfb"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e087f51c-feba-19b6-5595-fcbdce170411"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e0d562ca-f573-3c2f-eb83-f72d4d70d4fc"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e186a953-7ab3-c009-501c-a754267b770b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e1947bdc-ff2c-d2c1-3c55-f1f9bf778578"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e3bacefb-d79b-1569-a91c-43d7e4f6f230"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e6c7651f-182e-cf9c-1ef9-6293b95b500c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e75515a6-63cf-3612-a3a2-befa0d7048a7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e81c5db3-401a-e047-001e-045f39bef8ef"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ebf38b9a-6fbe-6e82-3977-2c4763bea072"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ed6278e0-436c-9fd9-0b9e-44fd424cbd1b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("edd4319b-86f3-24cb-248c-71da624c02f7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ee5dfc29-80f1-86ae-cde7-02484a18907a"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ee926d09-799c-7c6a-2419-a6ff814b2c03"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f0219540-8b2c-bd29-4f76-b832de53a56f"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f0965449-6b15-6c1a-f5cb-ebd2d575c02c"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f33ced84-eb43-fb39-ef79-b266e4d4cd94"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f39cca22-449e-9866-3a65-465a5510483e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f3eef99a-661e-2c68-7a4c-3053e2f28007"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f5b15ea6-133d-c2c9-7ef9-b0916ea96edb"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("f70ae426-f130-5637-0383-a5b63a06c500"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fa633273-9866-840d-9739-c6c957901e46"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fb9a713c-2de1-882a-64b7-0e8fef5d2f7e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fbf4479d-d70d-c76e-b053-699362443a17"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fc78fa89-b372-dcf7-7f1c-1e1bb14ecbe7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("fee6f04f-c4c1-e3e4-645d-bb6bb703aeb7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("ff5b4d88-c179-ff0d-6285-cf46ba475d7d"));
        }
    }
}
