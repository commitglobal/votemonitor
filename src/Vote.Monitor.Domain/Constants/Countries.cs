using System.Reflection;
using Vote.Monitor.Core;
using Vote.Monitor.Domain.Entities.CountryAggregate;

namespace Vote.Monitor.Domain.Constants;

public record CountryDetails(string Iso2, string Name, string Iso3, string NumericCode, string FullName)
{
    public Guid Id { get; } = Iso2.ToGuid();

    /// <summary>
    /// Two-letter country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string Iso2 { get; } = Iso2;

    /// <summary>
    /// Three-letter country code (ISO 3166-1 alpha-3)
    /// </summary>
    public string Iso3 { get; } = Iso3;

    /// <summary>
    /// Three-digit country number (ISO 3166-1 numeric)
    /// </summary>
    public string NumericCode { get; } = NumericCode;

    /// <summary>
    /// English country name
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    /// Full English country name
    /// </summary>
    public string FullName { get; } = FullName;

    public Country ToEntity()
    {
        return new Country(Iso2, Name, Iso3, NumericCode, FullName);
    }
}


public class Countries
{

    /// <summary>
    /// Courtesy of https://github.com/Svish/iso-3166-country-codes/blob/master/countries.sql
    /// </summary>

    public static readonly CountryDetails AF = new("AF", "Afghanistan", "AFG", "004", "Islamic Republic of Afghanistan");
    public static readonly CountryDetails AX = new("AX", "Åland Islands", "ALA", "248", "Åland Islands");
    public static readonly CountryDetails AL = new("AL", "Albania", "ALB", "008", "Republic of Albania");
    public static readonly CountryDetails DZ = new("DZ", "Algeria", "DZA", "012", "People's Democratic Republic of Algeria");
    public static readonly CountryDetails AS = new("AS", "American Samoa", "ASM", "016", "American Samoa");
    public static readonly CountryDetails AD = new("AD", "Andorra", "AND", "020", "Principality of Andorra");
    public static readonly CountryDetails AO = new("AO", "Angola", "AGO", "024", "Republic of Angola");
    public static readonly CountryDetails AI = new("AI", "Anguilla", "AIA", "660", "Anguilla");
    public static readonly CountryDetails AQ = new("AQ", "Antarctica", "ATA", "010", "Antarctica (the territory South of 60 deg S)");
    public static readonly CountryDetails AG = new("AG", "Antigua and Barbuda", "ATG", "028", "Antigua and Barbuda");
    public static readonly CountryDetails AR = new("AR", "Argentina", "ARG", "032", "Argentine Republic");
    public static readonly CountryDetails AM = new("AM", "Armenia", "ARM", "051", "Republic of Armenia");
    public static readonly CountryDetails AW = new("AW", "Aruba", "ABW", "533", "Aruba");
    public static readonly CountryDetails AU = new("AU", "Australia", "AUS", "036", "Commonwealth of Australia");
    public static readonly CountryDetails AT = new("AT", "Austria", "AUT", "040", "Republic of Austria");
    public static readonly CountryDetails AZ = new("AZ", "Azerbaijan", "AZE", "031", "Republic of Azerbaijan");
    public static readonly CountryDetails BS = new("BS", "Bahamas", "BHS", "044", "Commonwealth of the Bahamas");
    public static readonly CountryDetails BH = new("BH", "Bahrain", "BHR", "048", "Kingdom of Bahrain");
    public static readonly CountryDetails BD = new("BD", "Bangladesh", "BGD", "050", "People's Republic of Bangladesh");
    public static readonly CountryDetails BB = new("BB", "Barbados", "BRB", "052", "Barbados");
    public static readonly CountryDetails BY = new("BY", "Belarus", "BLR", "112", "Republic of Belarus");
    public static readonly CountryDetails BE = new("BE", "Belgium", "BEL", "056", "Kingdom of Belgium");
    public static readonly CountryDetails BZ = new("BZ", "Belize", "BLZ", "084", "Belize");
    public static readonly CountryDetails BJ = new("BJ", "Benin", "BEN", "204", "Republic of Benin");
    public static readonly CountryDetails BM = new("BM", "Bermuda", "BMU", "060", "Bermuda");
    public static readonly CountryDetails BT = new("BT", "Bhutan", "BTN", "064", "Kingdom of Bhutan");
    public static readonly CountryDetails BO = new("BO", "Bolivia", "BOL", "068", "Plurinational State of Bolivia");
    public static readonly CountryDetails BQ = new("BQ", "Bonaire, Sint Eustatius and Saba", "BES", "535", "Bonaire, Sint Eustatius and Saba");
    public static readonly CountryDetails BA = new("BA", "Bosnia and Herzegovina", "BIH", "070", "Bosnia and Herzegovina");
    public static readonly CountryDetails BW = new("BW", "Botswana", "BWA", "072", "Republic of Botswana");
    public static readonly CountryDetails BV = new("BV", "Bouvet Island (Bouvetøya)", "BVT", "074", "Bouvet Island (Bouvetøya)");
    public static readonly CountryDetails BR = new("BR", "Brazil", "BRA", "076", "Federative Republic of Brazil");
    public static readonly CountryDetails IO = new("IO", "British Indian Ocean Territory (Chagos Archipelago)", "IOT", "086", "British Indian Ocean Territory (Chagos Archipelago)");
    public static readonly CountryDetails VG = new("VG", "British Virgin Islands", "VGB", "092", "British Virgin Islands");
    public static readonly CountryDetails BN = new("BN", "Brunei Darussalam", "BRN", "096", "Brunei Darussalam");
    public static readonly CountryDetails BG = new("BG", "Bulgaria", "BGR", "100", "Republic of Bulgaria");
    public static readonly CountryDetails BF = new("BF", "Burkina Faso", "BFA", "854", "Burkina Faso");
    public static readonly CountryDetails BI = new("BI", "Burundi", "BDI", "108", "Republic of Burundi");
    public static readonly CountryDetails KH = new("KH", "Cambodia", "KHM", "116", "Kingdom of Cambodia");
    public static readonly CountryDetails CM = new("CM", "Cameroon", "CMR", "120", "Republic of Cameroon");
    public static readonly CountryDetails CA = new("CA", "Canada", "CAN", "124", "Canada");
    public static readonly CountryDetails CV = new("CV", "Cabo Verde", "CPV", "132", "Republic of Cabo Verde");
    public static readonly CountryDetails KY = new("KY", "Cayman Islands", "CYM", "136", "Cayman Islands");
    public static readonly CountryDetails CF = new("CF", "Central African Republic", "CAF", "140", "Central African Republic");
    public static readonly CountryDetails TD = new("TD", "Chad", "TCD", "148", "Republic of Chad");
    public static readonly CountryDetails CL = new("CL", "Chile", "CHL", "152", "Republic of Chile");
    public static readonly CountryDetails CN = new("CN", "China", "CHN", "156", "People's Republic of China");
    public static readonly CountryDetails CX = new("CX", "Christmas Island", "CXR", "162", "Christmas Island");
    public static readonly CountryDetails CC = new("CC", "Cocos (Keeling) Islands", "CCK", "166", "Cocos (Keeling) Islands");
    public static readonly CountryDetails CO = new("CO", "Colombia", "COL", "170", "Republic of Colombia");
    public static readonly CountryDetails KM = new("KM", "Comoros", "COM", "174", "Union of the Comoros");
    public static readonly CountryDetails CD = new("CD", "Congo", "COD", "180", "Democratic Republic of the Congo");
    public static readonly CountryDetails CG = new("CG", "Congo", "COG", "178", "Republic of the Congo");
    public static readonly CountryDetails CK = new("CK", "Cook Islands", "COK", "184", "Cook Islands");
    public static readonly CountryDetails CR = new("CR", "Costa Rica", "CRI", "188", "Republic of Costa Rica");
    public static readonly CountryDetails CI = new("CI", "Cote d'Ivoire", "CIV", "384", "Republic of Cote d'Ivoire");
    public static readonly CountryDetails HR = new("HR", "Croatia", "HRV", "191", "Republic of Croatia");
    public static readonly CountryDetails CU = new("CU", "Cuba", "CUB", "192", "Republic of Cuba");
    public static readonly CountryDetails CW = new("CW", "Curaçao", "CUW", "531", "Curaçao");
    public static readonly CountryDetails CY = new("CY", "Cyprus", "CYP", "196", "Republic of Cyprus");
    public static readonly CountryDetails CZ = new("CZ", "Czechia", "CZE", "203", "Czech Republic");
    public static readonly CountryDetails DK = new("DK", "Denmark", "DNK", "208", "Kingdom of Denmark");
    public static readonly CountryDetails DJ = new("DJ", "Djibouti", "DJI", "262", "Republic of Djibouti");
    public static readonly CountryDetails DM = new("DM", "Dominica", "DMA", "212", "Commonwealth of Dominica");
    public static readonly CountryDetails DO = new("DO", "Dominican Republic", "DOM", "214", "Dominican Republic");
    public static readonly CountryDetails EC = new("EC", "Ecuador", "ECU", "218", "Republic of Ecuador");
    public static readonly CountryDetails EG = new("EG", "Egypt", "EGY", "818", "Arab Republic of Egypt");
    public static readonly CountryDetails SV = new("SV", "El Salvador", "SLV", "222", "Republic of El Salvador");
    public static readonly CountryDetails GQ = new("GQ", "Equatorial Guinea", "GNQ", "226", "Republic of Equatorial Guinea");
    public static readonly CountryDetails ER = new("ER", "Eritrea", "ERI", "232", "State of Eritrea");
    public static readonly CountryDetails EE = new("EE", "Estonia", "EST", "233", "Republic of Estonia");
    public static readonly CountryDetails ET = new("ET", "Ethiopia", "ETH", "231", "Federal Democratic Republic of Ethiopia");
    public static readonly CountryDetails FO = new("FO", "Faroe Islands", "FRO", "234", "Faroe Islands");
    public static readonly CountryDetails FK = new("FK", "Falkland Islands (Malvinas)", "FLK", "238", "Falkland Islands (Malvinas)");
    public static readonly CountryDetails FJ = new("FJ", "Fiji", "FJI", "242", "Republic of Fiji");
    public static readonly CountryDetails FI = new("FI", "Finland", "FIN", "246", "Republic of Finland");
    public static readonly CountryDetails FR = new("FR", "France", "FRA", "250", "French Republic");
    public static readonly CountryDetails GF = new("GF", "French Guiana", "GUF", "254", "French Guiana");
    public static readonly CountryDetails PF = new("PF", "French Polynesia", "PYF", "258", "French Polynesia");
    public static readonly CountryDetails TF = new("TF", "French Southern Territories", "ATF", "260", "French Southern Territories");
    public static readonly CountryDetails GA = new("GA", "Gabon", "GAB", "266", "Gabonese Republic");
    public static readonly CountryDetails GM = new("GM", "Gambia", "GMB", "270", "Republic of the Gambia");
    public static readonly CountryDetails GE = new("GE", "Georgia", "GEO", "268", "Georgia");
    public static readonly CountryDetails DE = new("DE", "Germany", "DEU", "276", "Federal Republic of Germany");
    public static readonly CountryDetails GH = new("GH", "Ghana", "GHA", "288", "Republic of Ghana");
    public static readonly CountryDetails GI = new("GI", "Gibraltar", "GIB", "292", "Gibraltar");
    public static readonly CountryDetails GR = new("GR", "Greece", "GRC", "300", "Hellenic Republic of Greece");
    public static readonly CountryDetails GL = new("GL", "Greenland", "GRL", "304", "Greenland");
    public static readonly CountryDetails GD = new("GD", "Grenada", "GRD", "308", "Grenada");
    public static readonly CountryDetails GP = new("GP", "Guadeloupe", "GLP", "312", "Guadeloupe");
    public static readonly CountryDetails GU = new("GU", "Guam", "GUM", "316", "Guam");
    public static readonly CountryDetails GT = new("GT", "Guatemala", "GTM", "320", "Republic of Guatemala");
    public static readonly CountryDetails GG = new("GG", "Guernsey", "GGY", "831", "Bailiwick of Guernsey");
    public static readonly CountryDetails GN = new("GN", "Guinea", "GIN", "324", "Republic of Guinea");
    public static readonly CountryDetails GW = new("GW", "Guinea-Bissau", "GNB", "624", "Republic of Guinea-Bissau");
    public static readonly CountryDetails GY = new("GY", "Guyana", "GUY", "328", "Co-operative Republic of Guyana");
    public static readonly CountryDetails HT = new("HT", "Haiti", "HTI", "332", "Republic of Haiti");
    public static readonly CountryDetails HM = new("HM", "Heard Island and McDonald Islands", "HMD", "334", "Heard Island and McDonald Islands");
    public static readonly CountryDetails VA = new("VA", "Holy See (Vatican City State)", "VAT", "336", "Holy See (Vatican City State)");
    public static readonly CountryDetails HN = new("HN", "Honduras", "HND", "340", "Republic of Honduras");
    public static readonly CountryDetails HK = new("HK", "Hong Kong", "HKG", "344", "Hong Kong Special Administrative Region of China");
    public static readonly CountryDetails HU = new("HU", "Hungary", "HUN", "348", "Hungary");
    public static readonly CountryDetails IS = new("IS", "Iceland", "ISL", "352", "Iceland");
    public static readonly CountryDetails IN = new("IN", "India", "IND", "356", "Republic of India");
    public static readonly CountryDetails ID = new("ID", "Indonesia", "IDN", "360", "Republic of Indonesia");
    public static readonly CountryDetails IR = new("IR", "Iran", "IRN", "364", "Islamic Republic of Iran");
    public static readonly CountryDetails IQ = new("IQ", "Iraq", "IRQ", "368", "Republic of Iraq");
    public static readonly CountryDetails IE = new("IE", "Ireland", "IRL", "372", "Ireland");
    public static readonly CountryDetails IM = new("IM", "Isle of Man", "IMN", "833", "Isle of Man");
    public static readonly CountryDetails IL = new("IL", "Israel", "ISR", "376", "State of Israel");
    public static readonly CountryDetails IT = new("IT", "Italy", "ITA", "380", "Republic of Italy");
    public static readonly CountryDetails JM = new("JM", "Jamaica", "JAM", "388", "Jamaica");
    public static readonly CountryDetails JP = new("JP", "Japan", "JPN", "392", "Japan");
    public static readonly CountryDetails JE = new("JE", "Jersey", "JEY", "832", "Bailiwick of Jersey");
    public static readonly CountryDetails JO = new("JO", "Jordan", "JOR", "400", "Hashemite Kingdom of Jordan");
    public static readonly CountryDetails KZ = new("KZ", "Kazakhstan", "KAZ", "398", "Republic of Kazakhstan");
    public static readonly CountryDetails KE = new("KE", "Kenya", "KEN", "404", "Republic of Kenya");
    public static readonly CountryDetails KI = new("KI", "Kiribati", "KIR", "296", "Republic of Kiribati");
    public static readonly CountryDetails KP = new("KP", "Korea", "PRK", "408", "Democratic People's Republic of Korea");
    public static readonly CountryDetails KR = new("KR", "Korea", "KOR", "410", "Republic of Korea");
    public static readonly CountryDetails KW = new("KW", "Kuwait", "KWT", "414", "State of Kuwait");
    public static readonly CountryDetails KG = new("KG", "Kyrgyz Republic", "KGZ", "417", "Kyrgyz Republic");
    public static readonly CountryDetails LA = new("LA", "Lao People's Democratic Republic", "LAO", "418", "Lao People's Democratic Republic");
    public static readonly CountryDetails LV = new("LV", "Latvia", "LVA", "428", "Republic of Latvia");
    public static readonly CountryDetails LB = new("LB", "Lebanon", "LBN", "422", "Lebanese Republic");
    public static readonly CountryDetails LS = new("LS", "Lesotho", "LSO", "426", "Kingdom of Lesotho");
    public static readonly CountryDetails LR = new("LR", "Liberia", "LBR", "430", "Republic of Liberia");
    public static readonly CountryDetails LY = new("LY", "Libya", "LBY", "434", "State of Libya");
    public static readonly CountryDetails LI = new("LI", "Liechtenstein", "LIE", "438", "Principality of Liechtenstein");
    public static readonly CountryDetails LT = new("LT", "Lithuania", "LTU", "440", "Republic of Lithuania");
    public static readonly CountryDetails LU = new("LU", "Luxembourg", "LUX", "442", "Grand Duchy of Luxembourg");
    public static readonly CountryDetails MO = new("MO", "Macao", "MAC", "446", "Macao Special Administrative Region of China");
    public static readonly CountryDetails MG = new("MG", "Madagascar", "MDG", "450", "Republic of Madagascar");
    public static readonly CountryDetails MW = new("MW", "Malawi", "MWI", "454", "Republic of Malawi");
    public static readonly CountryDetails MY = new("MY", "Malaysia", "MYS", "458", "Malaysia");
    public static readonly CountryDetails MV = new("MV", "Maldives", "MDV", "462", "Republic of Maldives");
    public static readonly CountryDetails ML = new("ML", "Mali", "MLI", "466", "Republic of Mali");
    public static readonly CountryDetails MT = new("MT", "Malta", "MLT", "470", "Republic of Malta");
    public static readonly CountryDetails MH = new("MH", "Marshall Islands", "MHL", "584", "Republic of the Marshall Islands");
    public static readonly CountryDetails MQ = new("MQ", "Martinique", "MTQ", "474", "Martinique");
    public static readonly CountryDetails MR = new("MR", "Mauritania", "MRT", "478", "Islamic Republic of Mauritania");
    public static readonly CountryDetails MU = new("MU", "Mauritius", "MUS", "480", "Republic of Mauritius");
    public static readonly CountryDetails YT = new("YT", "Mayotte", "MYT", "175", "Mayotte");
    public static readonly CountryDetails MX = new("MX", "Mexico", "MEX", "484", "United Mexican States");
    public static readonly CountryDetails FM = new("FM", "Micronesia", "FSM", "583", "Federated States of Micronesia");
    public static readonly CountryDetails MD = new("MD", "Moldova", "MDA", "498", "Republic of Moldova");
    public static readonly CountryDetails MC = new("MC", "Monaco", "MCO", "492", "Principality of Monaco");
    public static readonly CountryDetails MN = new("MN", "Mongolia", "MNG", "496", "Mongolia");
    public static readonly CountryDetails ME = new("ME", "Montenegro", "MNE", "499", "Montenegro");
    public static readonly CountryDetails MS = new("MS", "Montserrat", "MSR", "500", "Montserrat");
    public static readonly CountryDetails MA = new("MA", "Morocco", "MAR", "504", "Kingdom of Morocco");
    public static readonly CountryDetails MZ = new("MZ", "Mozambique", "MOZ", "508", "Republic of Mozambique");
    public static readonly CountryDetails MM = new("MM", "Myanmar", "MMR", "104", "Republic of the Union of Myanmar");
    public static readonly CountryDetails NA = new("NA", "Namibia", "NAM", "516", "Republic of Namibia");
    public static readonly CountryDetails NR = new("NR", "Nauru", "NRU", "520", "Republic of Nauru");
    public static readonly CountryDetails NP = new("NP", "Nepal", "NPL", "524", "Nepal");
    public static readonly CountryDetails NL = new("NL", "Netherlands", "NLD", "528", "Kingdom of the Netherlands");
    public static readonly CountryDetails NC = new("NC", "New Caledonia", "NCL", "540", "New Caledonia");
    public static readonly CountryDetails NZ = new("NZ", "New Zealand", "NZL", "554", "New Zealand");
    public static readonly CountryDetails NI = new("NI", "Nicaragua", "NIC", "558", "Republic of Nicaragua");
    public static readonly CountryDetails NE = new("NE", "Niger", "NER", "562", "Republic of Niger");
    public static readonly CountryDetails NG = new("NG", "Nigeria", "NGA", "566", "Federal Republic of Nigeria");
    public static readonly CountryDetails NU = new("NU", "Niue", "NIU", "570", "Niue");
    public static readonly CountryDetails NF = new("NF", "Norfolk Island", "NFK", "574", "Norfolk Island");
    public static readonly CountryDetails MK = new("MK", "North Macedonia", "MKD", "807", "Republic of North Macedonia");
    public static readonly CountryDetails MP = new("MP", "Northern Mariana Islands", "MNP", "580", "Commonwealth of the Northern Mariana Islands");
    public static readonly CountryDetails NO = new("NO", "Norway", "NOR", "578", "Kingdom of Norway");
    public static readonly CountryDetails OM = new("OM", "Oman", "OMN", "512", "Sultanate of Oman");
    public static readonly CountryDetails PK = new("PK", "Pakistan", "PAK", "586", "Islamic Republic of Pakistan");
    public static readonly CountryDetails PW = new("PW", "Palau", "PLW", "585", "Republic of Palau");
    public static readonly CountryDetails PS = new("PS", "Palestine", "PSE", "275", "State of Palestine");
    public static readonly CountryDetails PA = new("PA", "Panama", "PAN", "591", "Republic of Panama");
    public static readonly CountryDetails PG = new("PG", "Papua New Guinea", "PNG", "598", "Independent State of Papua New Guinea");
    public static readonly CountryDetails PY = new("PY", "Paraguay", "PRY", "600", "Republic of Paraguay");
    public static readonly CountryDetails PE = new("PE", "Peru", "PER", "604", "Republic of Peru");
    public static readonly CountryDetails PH = new("PH", "Philippines", "PHL", "608", "Republic of the Philippines");
    public static readonly CountryDetails PN = new("PN", "Pitcairn Islands", "PCN", "612", "Pitcairn Islands");
    public static readonly CountryDetails PL = new("PL", "Poland", "POL", "616", "Republic of Poland");
    public static readonly CountryDetails PT = new("PT", "Portugal", "PRT", "620", "Portuguese Republic");
    public static readonly CountryDetails PR = new("PR", "Puerto Rico", "PRI", "630", "Commonwealth of Puerto Rico");
    public static readonly CountryDetails QA = new("QA", "Qatar", "QAT", "634", "State of Qatar");
    public static readonly CountryDetails RE = new("RE", "Réunion", "REU", "638", "Réunion");
    public static readonly CountryDetails RO = new("RO", "Romania", "ROU", "642", "Romania");
    public static readonly CountryDetails RU = new("RU", "Russian Federation", "RUS", "643", "Russian Federation");
    public static readonly CountryDetails RW = new("RW", "Rwanda", "RWA", "646", "Republic of Rwanda");
    public static readonly CountryDetails BL = new("BL", "Saint Barthélemy", "BLM", "652", "Saint Barthélemy");
    public static readonly CountryDetails SH = new("SH", "Saint Helena, Ascension and Tristan da Cunha", "SHN", "654", "Saint Helena, Ascension and Tristan da Cunha");
    public static readonly CountryDetails KN = new("KN", "Saint Kitts and Nevis", "KNA", "659", "Federation of Saint Kitts and Nevis");
    public static readonly CountryDetails LC = new("LC", "Saint Lucia", "LCA", "662", "Saint Lucia");
    public static readonly CountryDetails MF = new("MF", "Saint Martin", "MAF", "663", "Saint Martin (French part)");
    public static readonly CountryDetails PM = new("PM", "Saint Pierre and Miquelon", "SPM", "666", "Saint Pierre and Miquelon");
    public static readonly CountryDetails VC = new("VC", "Saint Vincent and the Grenadines", "VCT", "670", "Saint Vincent and the Grenadines");
    public static readonly CountryDetails WS = new("WS", "Samoa", "WSM", "882", "Independent State of Samoa");
    public static readonly CountryDetails SM = new("SM", "San Marino", "SMR", "674", "Republic of San Marino");
    public static readonly CountryDetails ST = new("ST", "Sao Tome and Principe", "STP", "678", "Democratic Republic of Sao Tome and Principe");
    public static readonly CountryDetails SA = new("SA", "Saudi Arabia", "SAU", "682", "Kingdom of Saudi Arabia");
    public static readonly CountryDetails SN = new("SN", "Senegal", "SEN", "686", "Republic of Senegal");
    public static readonly CountryDetails RS = new("RS", "Serbia", "SRB", "688", "Republic of Serbia");
    public static readonly CountryDetails SC = new("SC", "Seychelles", "SYC", "690", "Republic of Seychelles");
    public static readonly CountryDetails SL = new("SL", "Sierra Leone", "SLE", "694", "Republic of Sierra Leone");
    public static readonly CountryDetails SG = new("SG", "Singapore", "SGP", "702", "Republic of Singapore");
    public static readonly CountryDetails SX = new("SX", "Sint Maarten (Dutch part)", "SXM", "534", "Sint Maarten (Dutch part)");
    public static readonly CountryDetails SK = new("SK", "Slovakia (Slovak Republic)", "SVK", "703", "Slovakia (Slovak Republic)");
    public static readonly CountryDetails SI = new("SI", "Slovenia", "SVN", "705", "Republic of Slovenia");
    public static readonly CountryDetails SB = new("SB", "Solomon Islands", "SLB", "090", "Solomon Islands");
    public static readonly CountryDetails SO = new("SO", "Somalia", "SOM", "706", "Federal Republic of Somalia");
    public static readonly CountryDetails ZA = new("ZA", "South Africa", "ZAF", "710", "Republic of South Africa");
    public static readonly CountryDetails GS = new("GS", "South Georgia and the South Sandwich Islands", "SGS", "239", "South Georgia and the South Sandwich Islands");
    public static readonly CountryDetails SS = new("SS", "South Sudan", "SSD", "728", "Republic of South Sudan");
    public static readonly CountryDetails ES = new("ES", "Spain", "ESP", "724", "Kingdom of Spain");
    public static readonly CountryDetails LK = new("LK", "Sri Lanka", "LKA", "144", "Democratic Socialist Republic of Sri Lanka");
    public static readonly CountryDetails SD = new("SD", "Sudan", "SDN", "729", "Republic of Sudan");
    public static readonly CountryDetails SR = new("SR", "Suriname", "SUR", "740", "Republic of Suriname");
    public static readonly CountryDetails SJ = new("SJ", "Svalbard & Jan Mayen Islands", "SJM", "744", "Svalbard & Jan Mayen Islands");
    public static readonly CountryDetails SZ = new("SZ", "Eswatini", "SWZ", "748", "Kingdom of Eswatini");
    public static readonly CountryDetails SE = new("SE", "Sweden", "SWE", "752", "Kingdom of Sweden");
    public static readonly CountryDetails CH = new("CH", "Switzerland", "CHE", "756", "Swiss Confederation");
    public static readonly CountryDetails SY = new("SY", "Syrian Arab Republic", "SYR", "760", "Syrian Arab Republic");
    public static readonly CountryDetails TW = new("TW", "Taiwan", "TWN", "158", "Taiwan, Province of China");
    public static readonly CountryDetails TJ = new("TJ", "Tajikistan", "TJK", "762", "Republic of Tajikistan");
    public static readonly CountryDetails TZ = new("TZ", "Tanzania", "TZA", "834", "United Republic of Tanzania");
    public static readonly CountryDetails TH = new("TH", "Thailand", "THA", "764", "Kingdom of Thailand");
    public static readonly CountryDetails TL = new("TL", "Timor-Leste", "TLS", "626", "Democratic Republic of Timor-Leste");
    public static readonly CountryDetails TG = new("TG", "Togo", "TGO", "768", "Togolese Republic");
    public static readonly CountryDetails TK = new("TK", "Tokelau", "TKL", "772", "Tokelau");
    public static readonly CountryDetails TO = new("TO", "Tonga", "TON", "776", "Kingdom of Tonga");
    public static readonly CountryDetails TT = new("TT", "Trinidad and Tobago", "TTO", "780", "Republic of Trinidad and Tobago");
    public static readonly CountryDetails TN = new("TN", "Tunisia", "TUN", "788", "Tunisian Republic");
    public static readonly CountryDetails TR = new("TR", "Türkiye", "TUR", "792", "Republic of Türkiye");
    public static readonly CountryDetails TM = new("TM", "Turkmenistan", "TKM", "795", "Turkmenistan");
    public static readonly CountryDetails TC = new("TC", "Turks and Caicos Islands", "TCA", "796", "Turks and Caicos Islands");
    public static readonly CountryDetails TV = new("TV", "Tuvalu", "TUV", "798", "Tuvalu");
    public static readonly CountryDetails UG = new("UG", "Uganda", "UGA", "800", "Republic of Uganda");
    public static readonly CountryDetails UA = new("UA", "Ukraine", "UKR", "804", "Ukraine");
    public static readonly CountryDetails AE = new("AE", "United Arab Emirates", "ARE", "784", "United Arab Emirates");
    public static readonly CountryDetails GB = new("GB", "United Kingdom of Great Britain and Northern Ireland", "GBR", "826", "United Kingdom of Great Britain & Northern Ireland");
    public static readonly CountryDetails US = new("US", "United States of America", "USA", "840", "United States of America");
    public static readonly CountryDetails UM = new("UM", "United States Minor Outlying Islands", "UMI", "581", "United States Minor Outlying Islands");
    public static readonly CountryDetails VI = new("VI", "United States Virgin Islands", "VIR", "850", "United States Virgin Islands");
    public static readonly CountryDetails UY = new("UY", "Uruguay", "URY", "858", "Eastern Republic of Uruguay");
    public static readonly CountryDetails UZ = new("UZ", "Uzbekistan", "UZB", "860", "Republic of Uzbekistan");
    public static readonly CountryDetails VU = new("VU", "Vanuatu", "VUT", "548", "Republic of Vanuatu");
    public static readonly CountryDetails VE = new("VE", "Venezuela", "VEN", "862", "Bolivarian Republic of Venezuela");
    public static readonly CountryDetails VN = new("VN", "Vietnam", "VNM", "704", "Socialist Republic of Vietnam");
    public static readonly CountryDetails WF = new("WF", "Wallis and Futuna", "WLF", "876", "Wallis and Futuna");
    public static readonly CountryDetails EH = new("EH", "Western Sahara", "ESH", "732", "Western Sahara");
    public static readonly CountryDetails YE = new("YE", "Yemen", "YEM", "887", "Yemen");
    public static readonly CountryDetails ZM = new("ZM", "Zambia", "ZMB", "894", "Republic of Zambia");
    public static readonly CountryDetails ZW = new("ZW", "Zimbabwe", "ZWE", "716", "Republic of Zimbabwe");

    public static IEnumerable<CountryDetails> GetAll()
    {
        var properties = typeof(Countries)
            .GetProperties(BindingFlags.Static |
                           BindingFlags.Public)
            .Where(x => x.PropertyType == typeof(CountryDetails));

        foreach (var property in properties)
        {
            yield return (CountryDetails)property.GetValue(null, null);
        }
    }
}
