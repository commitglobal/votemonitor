using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Domain.Constants;

public record CountryDetails
{
    public CountryDetails(string name, string fullName, string iso2, string iso3, string numericCode)
    {
        Id = iso2.ToGuid();
        Name = name;
        FullName = fullName;
        Iso2 = iso2;
        Iso3 = iso3;
        NumericCode = numericCode;
    }

    public Guid Id { get; }

    /// <summary>
    /// Two-letter country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string Iso2 { get; }

    /// <summary>
    /// Three-letter country code (ISO 3166-1 alpha-3)
    /// </summary>
    public string Iso3 { get; }

    /// <summary>
    /// Three-digit country number (ISO 3166-1 numeric)
    /// </summary>
    public string NumericCode { get; }

    /// <summary>
    /// English country name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Full English country name
    /// </summary>
    public string FullName { get; }

    public Country ToEntity()
    {
        // Set the time to first of January in order to not regenerate the migration every time.
        var timeService = new FreezeTimeProvider(new DateTime(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc));
        return new Country(Name, FullName, Iso2, Iso3, NumericCode, timeService);
    }
}

/// <summary>
/// Courtesy of https://github.com/Svish/iso-3166-country-codes/blob/master/countries.sql
/// </summary>
public static class CountriesList
{
    public static readonly CountryDetails AF = new("Afghanistan", "Islamic Republic of Afghanistan", "AF", "AFG", "004");
    public static readonly CountryDetails AX = new("Åland Islands", "Åland Islands", "AX", "ALA", "248");
    public static readonly CountryDetails AL = new("Albania", "Republic of Albania", "AL", "ALB", "008");
    public static readonly CountryDetails DZ = new("Algeria", "People's Democratic Republic of Algeria", "DZ", "DZA", "012");
    public static readonly CountryDetails AS = new("American Samoa", "American Samoa", "AS", "ASM", "016");
    public static readonly CountryDetails AD = new("Andorra", "Principality of Andorra", "AD", "AND", "020");
    public static readonly CountryDetails AO = new("Angola", "Republic of Angola", "AO", "AGO", "024");
    public static readonly CountryDetails AI = new("Anguilla", "Anguilla", "AI", "AIA", "660");
    public static readonly CountryDetails AQ = new("Antarctica", "Antarctica (the territory South of 60 deg S)", "AQ", "ATA", "010");
    public static readonly CountryDetails AG = new("Antigua and Barbuda", "Antigua and Barbuda", "AG", "ATG", "028");
    public static readonly CountryDetails AR = new("Argentina", "Argentine Republic", "AR", "ARG", "032");
    public static readonly CountryDetails AM = new("Armenia", "Republic of Armenia", "AM", "ARM", "051");
    public static readonly CountryDetails AW = new("Aruba", "Aruba", "AW", "ABW", "533");
    public static readonly CountryDetails AU = new("Australia", "Commonwealth of Australia", "AU", "AUS", "036");
    public static readonly CountryDetails AT = new("Austria", "Republic of Austria", "AT", "AUT", "040");
    public static readonly CountryDetails AZ = new("Azerbaijan", "Republic of Azerbaijan", "AZ", "AZE", "031");
    public static readonly CountryDetails BS = new("Bahamas", "Commonwealth of the Bahamas", "BS", "BHS", "044");
    public static readonly CountryDetails BH = new("Bahrain", "Kingdom of Bahrain", "BH", "BHR", "048");
    public static readonly CountryDetails BD = new("Bangladesh", "People's Republic of Bangladesh", "BD", "BGD", "050");
    public static readonly CountryDetails BB = new("Barbados", "Barbados", "BB", "BRB", "052");
    public static readonly CountryDetails BY = new("Belarus", "Republic of Belarus", "BY", "BLR", "112");
    public static readonly CountryDetails BE = new("Belgium", "Kingdom of Belgium", "BE", "BEL", "056");
    public static readonly CountryDetails BZ = new("Belize", "Belize", "BZ", "BLZ", "084");
    public static readonly CountryDetails BJ = new("Benin", "Republic of Benin", "BJ", "BEN", "204");
    public static readonly CountryDetails BM = new("Bermuda", "Bermuda", "BM", "BMU", "060");
    public static readonly CountryDetails BT = new("Bhutan", "Kingdom of Bhutan", "BT", "BTN", "064");
    public static readonly CountryDetails BO = new("Bolivia", "Plurinational State of Bolivia", "BO", "BOL", "068");
    public static readonly CountryDetails BQ = new("Bonaire, Sint Eustatius and Saba", "Bonaire, Sint Eustatius and Saba", "BQ", "BES", "535");
    public static readonly CountryDetails BA = new("Bosnia and Herzegovina", "Bosnia and Herzegovina", "BA", "BIH", "070");
    public static readonly CountryDetails BW = new("Botswana", "Republic of Botswana", "BW", "BWA", "072");
    public static readonly CountryDetails BV = new("Bouvet Island (Bouvetøya)", "Bouvet Island (Bouvetøya)", "BV", "BVT", "074");
    public static readonly CountryDetails BR = new("Brazil", "Federative Republic of Brazil", "BR", "BRA", "076");
    public static readonly CountryDetails IO = new("British Indian Ocean Territory (Chagos Archipelago)", "British Indian Ocean Territory (Chagos Archipelago)", "IO", "IOT", "086");
    public static readonly CountryDetails VG = new("British Virgin Islands", "British Virgin Islands", "VG", "VGB", "092");
    public static readonly CountryDetails BN = new("Brunei Darussalam", "Brunei Darussalam", "BN", "BRN", "096");
    public static readonly CountryDetails BG = new("Bulgaria", "Republic of Bulgaria", "BG", "BGR", "100");
    public static readonly CountryDetails BF = new("Burkina Faso", "Burkina Faso", "BF", "BFA", "854");
    public static readonly CountryDetails BI = new("Burundi", "Republic of Burundi", "BI", "BDI", "108");
    public static readonly CountryDetails KH = new("Cambodia", "Kingdom of Cambodia", "KH", "KHM", "116");
    public static readonly CountryDetails CM = new("Cameroon", "Republic of Cameroon", "CM", "CMR", "120");
    public static readonly CountryDetails CA = new("Canada", "Canada", "CA", "CAN", "124");
    public static readonly CountryDetails CV = new("Cabo Verde", "Republic of Cabo Verde", "CV", "CPV", "132");
    public static readonly CountryDetails KY = new("Cayman Islands", "Cayman Islands", "KY", "CYM", "136");
    public static readonly CountryDetails CF = new("Central African Republic", "Central African Republic", "CF", "CAF", "140");
    public static readonly CountryDetails TD = new("Chad", "Republic of Chad", "TD", "TCD", "148");
    public static readonly CountryDetails CL = new("Chile", "Republic of Chile", "CL", "CHL", "152");
    public static readonly CountryDetails CN = new("China", "People's Republic of China", "CN", "CHN", "156");
    public static readonly CountryDetails CX = new("Christmas Island", "Christmas Island", "CX", "CXR", "162");
    public static readonly CountryDetails CC = new("Cocos (Keeling) Islands", "Cocos (Keeling) Islands", "CC", "CCK", "166");
    public static readonly CountryDetails CO = new("Colombia", "Republic of Colombia", "CO", "COL", "170");
    public static readonly CountryDetails KM = new("Comoros", "Union of the Comoros", "KM", "COM", "174");
    public static readonly CountryDetails CD = new("Congo", "Democratic Republic of the Congo", "CD", "COD", "180");
    public static readonly CountryDetails CG = new("Congo", "Republic of the Congo", "CG", "COG", "178");
    public static readonly CountryDetails CK = new("Cook Islands", "Cook Islands", "CK", "COK", "184");
    public static readonly CountryDetails CR = new("Costa Rica", "Republic of Costa Rica", "CR", "CRI", "188");
    public static readonly CountryDetails CI = new("Cote d'Ivoire", "Republic of Cote d'Ivoire", "CI", "CIV", "384");
    public static readonly CountryDetails HR = new("Croatia", "Republic of Croatia", "HR", "HRV", "191");
    public static readonly CountryDetails CU = new("Cuba", "Republic of Cuba", "CU", "CUB", "192");
    public static readonly CountryDetails CW = new("Curaçao", "Curaçao", "CW", "CUW", "531");
    public static readonly CountryDetails CY = new("Cyprus", "Republic of Cyprus", "CY", "CYP", "196");
    public static readonly CountryDetails CZ = new("Czechia", "Czech Republic", "CZ", "CZE", "203");
    public static readonly CountryDetails DK = new("Denmark", "Kingdom of Denmark", "DK", "DNK", "208");
    public static readonly CountryDetails DJ = new("Djibouti", "Republic of Djibouti", "DJ", "DJI", "262");
    public static readonly CountryDetails DM = new("Dominica", "Commonwealth of Dominica", "DM", "DMA", "212");
    public static readonly CountryDetails DO = new("Dominican Republic", "Dominican Republic", "DO", "DOM", "214");
    public static readonly CountryDetails EC = new("Ecuador", "Republic of Ecuador", "EC", "ECU", "218");
    public static readonly CountryDetails EG = new("Egypt", "Arab Republic of Egypt", "EG", "EGY", "818");
    public static readonly CountryDetails SV = new("El Salvador", "Republic of El Salvador", "SV", "SLV", "222");
    public static readonly CountryDetails GQ = new("Equatorial Guinea", "Republic of Equatorial Guinea", "GQ", "GNQ", "226");
    public static readonly CountryDetails ER = new("Eritrea", "State of Eritrea", "ER", "ERI", "232");
    public static readonly CountryDetails EE = new("Estonia", "Republic of Estonia", "EE", "EST", "233");
    public static readonly CountryDetails ET = new("Ethiopia", "Federal Democratic Republic of Ethiopia", "ET", "ETH", "231");
    public static readonly CountryDetails FO = new("Faroe Islands", "Faroe Islands", "FO", "FRO", "234");
    public static readonly CountryDetails FK = new("Falkland Islands (Malvinas)", "Falkland Islands (Malvinas)", "FK", "FLK", "238");
    public static readonly CountryDetails FJ = new("Fiji", "Republic of Fiji", "FJ", "FJI", "242");
    public static readonly CountryDetails FI = new("Finland", "Republic of Finland", "FI", "FIN", "246");
    public static readonly CountryDetails FR = new("France", "French Republic", "FR", "FRA", "250");
    public static readonly CountryDetails GF = new("French Guiana", "French Guiana", "GF", "GUF", "254");
    public static readonly CountryDetails PF = new("French Polynesia", "French Polynesia", "PF", "PYF", "258");
    public static readonly CountryDetails TF = new("French Southern Territories", "French Southern Territories", "TF", "ATF", "260");
    public static readonly CountryDetails GA = new("Gabon", "Gabonese Republic", "GA", "GAB", "266");
    public static readonly CountryDetails GM = new("Gambia", "Republic of the Gambia", "GM", "GMB", "270");
    public static readonly CountryDetails GE = new("Georgia", "Georgia", "GE", "GEO", "268");
    public static readonly CountryDetails DE = new("Germany", "Federal Republic of Germany", "DE", "DEU", "276");
    public static readonly CountryDetails GH = new("Ghana", "Republic of Ghana", "GH", "GHA", "288");
    public static readonly CountryDetails GI = new("Gibraltar", "Gibraltar", "GI", "GIB", "292");
    public static readonly CountryDetails GR = new("Greece", "Hellenic Republic of Greece", "GR", "GRC", "300");
    public static readonly CountryDetails GL = new("Greenland", "Greenland", "GL", "GRL", "304");
    public static readonly CountryDetails GD = new("Grenada", "Grenada", "GD", "GRD", "308");
    public static readonly CountryDetails GP = new("Guadeloupe", "Guadeloupe", "GP", "GLP", "312");
    public static readonly CountryDetails GU = new("Guam", "Guam", "GU", "GUM", "316");
    public static readonly CountryDetails GT = new("Guatemala", "Republic of Guatemala", "GT", "GTM", "320");
    public static readonly CountryDetails GG = new("Guernsey", "Bailiwick of Guernsey", "GG", "GGY", "831");
    public static readonly CountryDetails GN = new("Guinea", "Republic of Guinea", "GN", "GIN", "324");
    public static readonly CountryDetails GW = new("Guinea-Bissau", "Republic of Guinea-Bissau", "GW", "GNB", "624");
    public static readonly CountryDetails GY = new("Guyana", "Co-operative Republic of Guyana", "GY", "GUY", "328");
    public static readonly CountryDetails HT = new("Haiti", "Republic of Haiti", "HT", "HTI", "332");
    public static readonly CountryDetails HM = new("Heard Island and McDonald Islands", "Heard Island and McDonald Islands", "HM", "HMD", "334");
    public static readonly CountryDetails VA = new("Holy See (Vatican City State)", "Holy See (Vatican City State)", "VA", "VAT", "336");
    public static readonly CountryDetails HN = new("Honduras", "Republic of Honduras", "HN", "HND", "340");
    public static readonly CountryDetails HK = new("Hong Kong", "Hong Kong Special Administrative Region of China", "HK", "HKG", "344");
    public static readonly CountryDetails HU = new("Hungary", "Hungary", "HU", "HUN", "348");
    public static readonly CountryDetails IS = new("Iceland", "Iceland", "IS", "ISL", "352");
    public static readonly CountryDetails IN = new("India", "Republic of India", "IN", "IND", "356");
    public static readonly CountryDetails ID = new("Indonesia", "Republic of Indonesia", "ID", "IDN", "360");
    public static readonly CountryDetails IR = new("Iran", "Islamic Republic of Iran", "IR", "IRN", "364");
    public static readonly CountryDetails IQ = new("Iraq", "Republic of Iraq", "IQ", "IRQ", "368");
    public static readonly CountryDetails IE = new("Ireland", "Ireland", "IE", "IRL", "372");
    public static readonly CountryDetails IM = new("Isle of Man", "Isle of Man", "IM", "IMN", "833");
    public static readonly CountryDetails IL = new("Israel", "State of Israel", "IL", "ISR", "376");
    public static readonly CountryDetails IT = new("Italy", "Republic of Italy", "IT", "ITA", "380");
    public static readonly CountryDetails JM = new("Jamaica", "Jamaica", "JM", "JAM", "388");
    public static readonly CountryDetails JP = new("Japan", "Japan", "JP", "JPN", "392");
    public static readonly CountryDetails JE = new("Jersey", "Bailiwick of Jersey", "JE", "JEY", "832");
    public static readonly CountryDetails JO = new("Jordan", "Hashemite Kingdom of Jordan", "JO", "JOR", "400");
    public static readonly CountryDetails KZ = new("Kazakhstan", "Republic of Kazakhstan", "KZ", "KAZ", "398");
    public static readonly CountryDetails KE = new("Kenya", "Republic of Kenya", "KE", "KEN", "404");
    public static readonly CountryDetails KI = new("Kiribati", "Republic of Kiribati", "KI", "KIR", "296");
    public static readonly CountryDetails KP = new("Korea", "Democratic People's Republic of Korea", "KP", "PRK", "408");
    public static readonly CountryDetails KR = new("Korea", "Republic of Korea", "KR", "KOR", "410");
    public static readonly CountryDetails KW = new("Kuwait", "State of Kuwait", "KW", "KWT", "414");
    public static readonly CountryDetails KG = new("Kyrgyz Republic", "Kyrgyz Republic", "KG", "KGZ", "417");
    public static readonly CountryDetails LA = new("Lao People's Democratic Republic", "Lao People's Democratic Republic", "LA", "LAO", "418");
    public static readonly CountryDetails LV = new("Latvia", "Republic of Latvia", "LV", "LVA", "428");
    public static readonly CountryDetails LB = new("Lebanon", "Lebanese Republic", "LB", "LBN", "422");
    public static readonly CountryDetails LS = new("Lesotho", "Kingdom of Lesotho", "LS", "LSO", "426");
    public static readonly CountryDetails LR = new("Liberia", "Republic of Liberia", "LR", "LBR", "430");
    public static readonly CountryDetails LY = new("Libya", "State of Libya", "LY", "LBY", "434");
    public static readonly CountryDetails LI = new("Liechtenstein", "Principality of Liechtenstein", "LI", "LIE", "438");
    public static readonly CountryDetails LT = new("Lithuania", "Republic of Lithuania", "LT", "LTU", "440");
    public static readonly CountryDetails LU = new("Luxembourg", "Grand Duchy of Luxembourg", "LU", "LUX", "442");
    public static readonly CountryDetails MO = new("Macao", "Macao Special Administrative Region of China", "MO", "MAC", "446");
    public static readonly CountryDetails MG = new("Madagascar", "Republic of Madagascar", "MG", "MDG", "450");
    public static readonly CountryDetails MW = new("Malawi", "Republic of Malawi", "MW", "MWI", "454");
    public static readonly CountryDetails MY = new("Malaysia", "Malaysia", "MY", "MYS", "458");
    public static readonly CountryDetails MV = new("Maldives", "Republic of Maldives", "MV", "MDV", "462");
    public static readonly CountryDetails ML = new("Mali", "Republic of Mali", "ML", "MLI", "466");
    public static readonly CountryDetails MT = new("Malta", "Republic of Malta", "MT", "MLT", "470");
    public static readonly CountryDetails MH = new("Marshall Islands", "Republic of the Marshall Islands", "MH", "MHL", "584");
    public static readonly CountryDetails MQ = new("Martinique", "Martinique", "MQ", "MTQ", "474");
    public static readonly CountryDetails MR = new("Mauritania", "Islamic Republic of Mauritania", "MR", "MRT", "478");
    public static readonly CountryDetails MU = new("Mauritius", "Republic of Mauritius", "MU", "MUS", "480");
    public static readonly CountryDetails YT = new("Mayotte", "Mayotte", "YT", "MYT", "175");
    public static readonly CountryDetails MX = new("Mexico", "United Mexican States", "MX", "MEX", "484");
    public static readonly CountryDetails FM = new("Micronesia", "Federated States of Micronesia", "FM", "FSM", "583");
    public static readonly CountryDetails MD = new("Moldova", "Republic of Moldova", "MD", "MDA", "498");
    public static readonly CountryDetails MC = new("Monaco", "Principality of Monaco", "MC", "MCO", "492");
    public static readonly CountryDetails MN = new("Mongolia", "Mongolia", "MN", "MNG", "496");
    public static readonly CountryDetails ME = new("Montenegro", "Montenegro", "ME", "MNE", "499");
    public static readonly CountryDetails MS = new("Montserrat", "Montserrat", "MS", "MSR", "500");
    public static readonly CountryDetails MA = new("Morocco", "Kingdom of Morocco", "MA", "MAR", "504");
    public static readonly CountryDetails MZ = new("Mozambique", "Republic of Mozambique", "MZ", "MOZ", "508");
    public static readonly CountryDetails MM = new("Myanmar", "Republic of the Union of Myanmar", "MM", "MMR", "104");
    public static readonly CountryDetails NA = new("Namibia", "Republic of Namibia", "NA", "NAM", "516");
    public static readonly CountryDetails NR = new("Nauru", "Republic of Nauru", "NR", "NRU", "520");
    public static readonly CountryDetails NP = new("Nepal", "Nepal", "NP", "NPL", "524");
    public static readonly CountryDetails NL = new("Netherlands", "Kingdom of the Netherlands", "NL", "NLD", "528");
    public static readonly CountryDetails NC = new("New Caledonia", "New Caledonia", "NC", "NCL", "540");
    public static readonly CountryDetails NZ = new("New Zealand", "New Zealand", "NZ", "NZL", "554");
    public static readonly CountryDetails NI = new("Nicaragua", "Republic of Nicaragua", "NI", "NIC", "558");
    public static readonly CountryDetails NE = new("Niger", "Republic of Niger", "NE", "NER", "562");
    public static readonly CountryDetails NG = new("Nigeria", "Federal Republic of Nigeria", "NG", "NGA", "566");
    public static readonly CountryDetails NU = new("Niue", "Niue", "NU", "NIU", "570");
    public static readonly CountryDetails NF = new("Norfolk Island", "Norfolk Island", "NF", "NFK", "574");
    public static readonly CountryDetails MK = new("North Macedonia", "Republic of North Macedonia", "MK", "MKD", "807");
    public static readonly CountryDetails MP = new("Northern Mariana Islands", "Commonwealth of the Northern Mariana Islands", "MP", "MNP", "580");
    public static readonly CountryDetails NO = new("Norway", "Kingdom of Norway", "NO", "NOR", "578");
    public static readonly CountryDetails OM = new("Oman", "Sultanate of Oman", "OM", "OMN", "512");
    public static readonly CountryDetails PK = new("Pakistan", "Islamic Republic of Pakistan", "PK", "PAK", "586");
    public static readonly CountryDetails PW = new("Palau", "Republic of Palau", "PW", "PLW", "585");
    public static readonly CountryDetails PS = new("Palestine", "State of Palestine", "PS", "PSE", "275");
    public static readonly CountryDetails PA = new("Panama", "Republic of Panama", "PA", "PAN", "591");
    public static readonly CountryDetails PG = new("Papua New Guinea", "Independent State of Papua New Guinea", "PG", "PNG", "598");
    public static readonly CountryDetails PY = new("Paraguay", "Republic of Paraguay", "PY", "PRY", "600");
    public static readonly CountryDetails PE = new("Peru", "Republic of Peru", "PE", "PER", "604");
    public static readonly CountryDetails PH = new("Philippines", "Republic of the Philippines", "PH", "PHL", "608");
    public static readonly CountryDetails PN = new("Pitcairn Islands", "Pitcairn Islands", "PN", "PCN", "612");
    public static readonly CountryDetails PL = new("Poland", "Republic of Poland", "PL", "POL", "616");
    public static readonly CountryDetails PT = new("Portugal", "Portuguese Republic", "PT", "PRT", "620");
    public static readonly CountryDetails PR = new("Puerto Rico", "Commonwealth of Puerto Rico", "PR", "PRI", "630");
    public static readonly CountryDetails QA = new("Qatar", "State of Qatar", "QA", "QAT", "634");
    public static readonly CountryDetails RE = new("Réunion", "Réunion", "RE", "REU", "638");
    public static readonly CountryDetails RO = new("Romania", "Romania", "RO", "ROU", "642");
    public static readonly CountryDetails RU = new("Russian Federation", "Russian Federation", "RU", "RUS", "643");
    public static readonly CountryDetails RW = new("Rwanda", "Republic of Rwanda", "RW", "RWA", "646");
    public static readonly CountryDetails BL = new("Saint Barthélemy", "Saint Barthélemy", "BL", "BLM", "652");
    public static readonly CountryDetails SH = new("Saint Helena, Ascension and Tristan da Cunha", "Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", "654");
    public static readonly CountryDetails KN = new("Saint Kitts and Nevis", "Federation of Saint Kitts and Nevis", "KN", "KNA", "659");
    public static readonly CountryDetails LC = new("Saint Lucia", "Saint Lucia", "LC", "LCA", "662");
    public static readonly CountryDetails MF = new("Saint Martin", "Saint Martin (French part)", "MF", "MAF", "663");
    public static readonly CountryDetails PM = new("Saint Pierre and Miquelon", "Saint Pierre and Miquelon", "PM", "SPM", "666");
    public static readonly CountryDetails VC = new("Saint Vincent and the Grenadines", "Saint Vincent and the Grenadines", "VC", "VCT", "670");
    public static readonly CountryDetails WS = new("Samoa", "Independent State of Samoa", "WS", "WSM", "882");
    public static readonly CountryDetails SM = new("San Marino", "Republic of San Marino", "SM", "SMR", "674");
    public static readonly CountryDetails ST = new("Sao Tome and Principe", "Democratic Republic of Sao Tome and Principe", "ST", "STP", "678");
    public static readonly CountryDetails SA = new("Saudi Arabia", "Kingdom of Saudi Arabia", "SA", "SAU", "682");
    public static readonly CountryDetails SN = new("Senegal", "Republic of Senegal", "SN", "SEN", "686");
    public static readonly CountryDetails RS = new("Serbia", "Republic of Serbia", "RS", "SRB", "688");
    public static readonly CountryDetails SC = new("Seychelles", "Republic of Seychelles", "SC", "SYC", "690");
    public static readonly CountryDetails SL = new("Sierra Leone", "Republic of Sierra Leone", "SL", "SLE", "694");
    public static readonly CountryDetails SG = new("Singapore", "Republic of Singapore", "SG", "SGP", "702");
    public static readonly CountryDetails SX = new("Sint Maarten (Dutch part)", "Sint Maarten (Dutch part)", "SX", "SXM", "534");
    public static readonly CountryDetails SK = new("Slovakia (Slovak Republic)", "Slovakia (Slovak Republic)", "SK", "SVK", "703");
    public static readonly CountryDetails SI = new("Slovenia", "Republic of Slovenia", "SI", "SVN", "705");
    public static readonly CountryDetails SB = new("Solomon Islands", "Solomon Islands", "SB", "SLB", "090");
    public static readonly CountryDetails SO = new("Somalia", "Federal Republic of Somalia", "SO", "SOM", "706");
    public static readonly CountryDetails ZA = new("South Africa", "Republic of South Africa", "ZA", "ZAF", "710");
    public static readonly CountryDetails GS = new("South Georgia and the South Sandwich Islands", "South Georgia and the South Sandwich Islands", "GS", "SGS", "239");
    public static readonly CountryDetails SS = new("South Sudan", "Republic of South Sudan", "SS", "SSD", "728");
    public static readonly CountryDetails ES = new("Spain", "Kingdom of Spain", "ES", "ESP", "724");
    public static readonly CountryDetails LK = new("Sri Lanka", "Democratic Socialist Republic of Sri Lanka", "LK", "LKA", "144");
    public static readonly CountryDetails SD = new("Sudan", "Republic of Sudan", "SD", "SDN", "729");
    public static readonly CountryDetails SR = new("Suriname", "Republic of Suriname", "SR", "SUR", "740");
    public static readonly CountryDetails SJ = new("Svalbard & Jan Mayen Islands", "Svalbard & Jan Mayen Islands", "SJ", "SJM", "744");
    public static readonly CountryDetails SZ = new("Eswatini", "Kingdom of Eswatini", "SZ", "SWZ", "748");
    public static readonly CountryDetails SE = new("Sweden", "Kingdom of Sweden", "SE", "SWE", "752");
    public static readonly CountryDetails CH = new("Switzerland", "Swiss Confederation", "CH", "CHE", "756");
    public static readonly CountryDetails SY = new("Syrian Arab Republic", "Syrian Arab Republic", "SY", "SYR", "760");
    public static readonly CountryDetails TW = new("Taiwan", "Taiwan, Province of China", "TW", "TWN", "158");
    public static readonly CountryDetails TJ = new("Tajikistan", "Republic of Tajikistan", "TJ", "TJK", "762");
    public static readonly CountryDetails TZ = new("Tanzania", "United Republic of Tanzania", "TZ", "TZA", "834");
    public static readonly CountryDetails TH = new("Thailand", "Kingdom of Thailand", "TH", "THA", "764");
    public static readonly CountryDetails TL = new("Timor-Leste", "Democratic Republic of Timor-Leste", "TL", "TLS", "626");
    public static readonly CountryDetails TG = new("Togo", "Togolese Republic", "TG", "TGO", "768");
    public static readonly CountryDetails TK = new("Tokelau", "Tokelau", "TK", "TKL", "772");
    public static readonly CountryDetails TO = new("Tonga", "Kingdom of Tonga", "TO", "TON", "776");
    public static readonly CountryDetails TT = new("Trinidad and Tobago", "Republic of Trinidad and Tobago", "TT", "TTO", "780");
    public static readonly CountryDetails TN = new("Tunisia", "Tunisian Republic", "TN", "TUN", "788");
    public static readonly CountryDetails TR = new("Türkiye", "Republic of Türkiye", "TR", "TUR", "792");
    public static readonly CountryDetails TM = new("Turkmenistan", "Turkmenistan", "TM", "TKM", "795");
    public static readonly CountryDetails TC = new("Turks and Caicos Islands", "Turks and Caicos Islands", "TC", "TCA", "796");
    public static readonly CountryDetails TV = new("Tuvalu", "Tuvalu", "TV", "TUV", "798");
    public static readonly CountryDetails UG = new("Uganda", "Republic of Uganda", "UG", "UGA", "800");
    public static readonly CountryDetails UA = new("Ukraine", "Ukraine", "UA", "UKR", "804");
    public static readonly CountryDetails AE = new("United Arab Emirates", "United Arab Emirates", "AE", "ARE", "784");
    public static readonly CountryDetails GB = new("United Kingdom of Great Britain and Northern Ireland", "United Kingdom of Great Britain & Northern Ireland", "GB", "GBR", "826");
    public static readonly CountryDetails US = new("United States of America", "United States of America", "US", "USA", "840");
    public static readonly CountryDetails UM = new("United States Minor Outlying Islands", "United States Minor Outlying Islands", "UM", "UMI", "581");
    public static readonly CountryDetails VI = new("United States Virgin Islands", "United States Virgin Islands", "VI", "VIR", "850");
    public static readonly CountryDetails UY = new("Uruguay", "Eastern Republic of Uruguay", "UY", "URY", "858");
    public static readonly CountryDetails UZ = new("Uzbekistan", "Republic of Uzbekistan", "UZ", "UZB", "860");
    public static readonly CountryDetails VU = new("Vanuatu", "Republic of Vanuatu", "VU", "VUT", "548");
    public static readonly CountryDetails VE = new("Venezuela", "Bolivarian Republic of Venezuela", "VE", "VEN", "862");
    public static readonly CountryDetails VN = new("Vietnam", "Socialist Republic of Vietnam", "VN", "VNM", "704");
    public static readonly CountryDetails WF = new("Wallis and Futuna", "Wallis and Futuna", "WF", "WLF", "876");
    public static readonly CountryDetails EH = new("Western Sahara", "Western Sahara", "EH", "ESH", "732");
    public static readonly CountryDetails YE = new("Yemen", "Yemen", "YE", "YEM", "887");
    public static readonly CountryDetails ZM = new("Zambia", "Republic of Zambia", "ZM", "ZMB", "894");
    public static readonly CountryDetails ZW = new("Zimbabwe", "Republic of Zimbabwe", "ZW", "ZWE", "716");

    public static IEnumerable<CountryDetails> GetAll()
    {
        var fields = typeof(CountriesList)
            .GetFields(BindingFlags.Static |
                           BindingFlags.Public)
            .Where(x => x.FieldType == typeof(CountryDetails));

        foreach (var field in fields)
        {
            yield return (CountryDetails)field.GetValue(null)!;
        }
    }

    public static bool IsKnownCountry(Guid countryId)
    {
        return GetAll().Any(x => x.Id == countryId);
    }

    public static CountryDetails? Get(Guid countryId)
    {
        return GetAll().FirstOrDefault(x => x.Id == countryId);
    }
}
