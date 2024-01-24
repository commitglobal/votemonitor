using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Domain.Constants;

public record LanguageDetails
{
    public LanguageDetails(string name, string nativeName, string iso1)
    {
        Id = iso1.ToGuid();
        Name = name;
        NativeName = nativeName;
        Iso1 = iso1;
    }

    public Guid Id { get; }

    /// <summary>
    /// English language name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Native language name
    /// </summary>
    public string NativeName { get; }

    /// <summary>
    /// Two-letter language code (ISO 639-1)
    /// </summary>
    public string Iso1 { get; }

    public Language ToEntity()
    {
        // Set the time to first of January in order to not regenerate the migration every time.
        var timeService = new FreezeTimeProvider(new DateTime(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc));

        return new Language(Name, NativeName, Iso1, timeService);
    }
}

public static class LanguagesList
{
    public static readonly LanguageDetails AA = new("Afar", "Afaraf", "AA");
    public static readonly LanguageDetails AB = new("Abkhaz", "аҧсуа бызшәа", "AB");
    public static readonly LanguageDetails AE = new("Avestan", "avesta", "AE");
    public static readonly LanguageDetails AF = new("Afrikaans", "Afrikaans", "AF");
    public static readonly LanguageDetails AK = new("Akan", "Akan", "AK");
    public static readonly LanguageDetails AM = new("Amharic", "አማርኛ", "AM");
    public static readonly LanguageDetails AN = new("Aragonese", "aragonés", "AN");
    public static readonly LanguageDetails AR = new("Arabic", "اَلْعَرَبِيَّةُ", "AR");
    public static readonly LanguageDetails AS = new("Assamese", "অসমীয়া", "AS");
    public static readonly LanguageDetails AV = new("Avaric", "авар мацӀ", "AV");
    public static readonly LanguageDetails AY = new("Aymara", "aymar aru", "AY");
    public static readonly LanguageDetails AZ = new("Azerbaijani", "azərbaycan dili", "AZ");
    public static readonly LanguageDetails BA = new("Bashkir", "башҡорт теле", "BA");
    public static readonly LanguageDetails BE = new("Belarusian", "беларуская мова", "BE");
    public static readonly LanguageDetails BG = new("Bulgarian", "български език", "BG");
    public static readonly LanguageDetails BI = new("Bislama", "Bislama", "BI");
    public static readonly LanguageDetails BM = new("Bambara", "bamanankan", "BM");
    public static readonly LanguageDetails BN = new("Bengali", "বাংলা", "BN");
    public static readonly LanguageDetails BO = new("Tibetan", "བོད་ཡིག", "BO");
    public static readonly LanguageDetails BR = new("Breton", "brezhoneg", "BR");
    public static readonly LanguageDetails BS = new("Bosnian", "bosanski jezik", "BS");
    public static readonly LanguageDetails CA = new("Catalan", "Català", "CA");
    public static readonly LanguageDetails CE = new("Chechen", "нохчийн мотт", "CE");
    public static readonly LanguageDetails CH = new("Chamorro", "Chamoru", "CH");
    public static readonly LanguageDetails CO = new("Corsican", "corsu", "CO");
    public static readonly LanguageDetails CR = new("Cree", "ᓀᐦᐃᔭᐍᐏᐣ", "CR");
    public static readonly LanguageDetails CS = new("Czech", "čeština", "CS");
    public static readonly LanguageDetails CU = new("Old Church Slavonic", "ѩзыкъ словѣньскъ", "CU");
    public static readonly LanguageDetails CV = new("Chuvash", "чӑваш чӗлхи", "CV");
    public static readonly LanguageDetails CY = new("Welsh", "Cymraeg", "CY");
    public static readonly LanguageDetails DA = new("Danish", "dansk", "DA");
    public static readonly LanguageDetails DE = new("German", "Deutsch", "DE");
    public static readonly LanguageDetails DV = new("Divehi", "ދިވެހި", "DV");
    public static readonly LanguageDetails DZ = new("Dzongkha", "རྫོང་ཁ", "DZ");
    public static readonly LanguageDetails EE = new("Ewe", "Eʋegbe", "EE");
    public static readonly LanguageDetails EL = new("Greek", "Ελληνικά", "EL");
    public static readonly LanguageDetails EN = new("English", "English", "EN");
    public static readonly LanguageDetails EO = new("Esperanto", "Esperanto", "EO");
    public static readonly LanguageDetails ES = new("Spanish", "Español", "ES");
    public static readonly LanguageDetails ET = new("Estonian", "eesti", "ET");
    public static readonly LanguageDetails EU = new("Basque", "euskara", "EU");
    public static readonly LanguageDetails FA = new("Persian", "فارسی", "FA");
    public static readonly LanguageDetails FF = new("Fula", "Fulfulde", "FF");
    public static readonly LanguageDetails FI = new("Finnish", "suomi", "FI");
    public static readonly LanguageDetails FJ = new("Fijian", "vosa Vakaviti", "FJ");
    public static readonly LanguageDetails FO = new("Faroese", "føroyskt", "FO");
    public static readonly LanguageDetails FR = new("French", "Français", "FR");
    public static readonly LanguageDetails FY = new("Western Frisian", "Frysk", "FY");
    public static readonly LanguageDetails GA = new("Irish", "Gaeilge", "GA");
    public static readonly LanguageDetails GD = new("Scottish Gaelic", "Gàidhlig", "GD");
    public static readonly LanguageDetails GL = new("Galician", "galego", "GL");
    public static readonly LanguageDetails GN = new("Guaraní", "Avañe'ẽ", "GN");
    public static readonly LanguageDetails GU = new("Gujarati", "ગુજરાતી", "GU");
    public static readonly LanguageDetails GV = new("Manx", "Gaelg", "GV");
    public static readonly LanguageDetails HA = new("Hausa", "هَوُسَ", "HA");
    public static readonly LanguageDetails HE = new("Hebrew", "עברית", "HE");
    public static readonly LanguageDetails HI = new("Hindi", "हिन्दी", "HI");
    public static readonly LanguageDetails HO = new("Hiri Motu", "Hiri Motu", "HO");
    public static readonly LanguageDetails HR = new("Croatian", "Hrvatski", "HR");
    public static readonly LanguageDetails HT = new("Haitian", "Kreyòl ayisyen", "HT");
    public static readonly LanguageDetails HU = new("Hungarian", "magyar", "HU");
    public static readonly LanguageDetails HY = new("Armenian", "Հայերեն", "HY");
    public static readonly LanguageDetails HZ = new("Herero", "Otjiherero", "HZ");
    public static readonly LanguageDetails IA = new("Interlingua", "Interlingua", "IA");
    public static readonly LanguageDetails ID = new("Indonesian", "Bahasa Indonesia", "ID");
    public static readonly LanguageDetails IE = new("Interlingue", "Interlingue", "IE");
    public static readonly LanguageDetails IG = new("Igbo", "Asụsụ Igbo", "IG");
    public static readonly LanguageDetails II = new("Nuosu", "ꆈꌠ꒿ Nuosuhxop", "II");
    public static readonly LanguageDetails IK = new("Inupiaq", "Iñupiaq", "IK");
    public static readonly LanguageDetails IO = new("Ido", "Ido", "IO");
    public static readonly LanguageDetails IS = new("Icelandic", "Íslenska", "IS");
    public static readonly LanguageDetails IT = new("Italian", "Italiano", "IT");
    public static readonly LanguageDetails IU = new("Inuktitut", "ᐃᓄᒃᑎᑐᑦ", "IU");
    public static readonly LanguageDetails JA = new("Japanese", "日本語", "JA");
    public static readonly LanguageDetails JV = new("Javanese", "basa Jawa", "JV");
    public static readonly LanguageDetails KA = new("Georgian", "ქართული", "KA");
    public static readonly LanguageDetails KG = new("Kongo", "Kikongo", "KG");
    public static readonly LanguageDetails KI = new("Kikuyu", "Gĩkũyũ", "KI");
    public static readonly LanguageDetails KJ = new("Kwanyama", "Kuanyama", "KJ");
    public static readonly LanguageDetails KK = new("Kazakh", "қазақ тілі", "KK");
    public static readonly LanguageDetails KL = new("Kalaallisut", "kalaallisut", "KL");
    public static readonly LanguageDetails KM = new("Khmer", "ខេមរភាសា", "KM");
    public static readonly LanguageDetails KN = new("Kannada", "ಕನ್ನಡ", "KN");
    public static readonly LanguageDetails KO = new("Korean", "한국어", "KO");
    public static readonly LanguageDetails KR = new("Kanuri", "Kanuri", "KR");
    public static readonly LanguageDetails KS = new("Kashmiri", "कश्मीरी", "KS");
    public static readonly LanguageDetails KU = new("Kurdish", "Kurdî", "KU");
    public static readonly LanguageDetails KV = new("Komi", "коми кыв", "KV");
    public static readonly LanguageDetails KW = new("Cornish", "Kernewek", "KW");
    public static readonly LanguageDetails KY = new("Kyrgyz", "Кыргызча", "KY");
    public static readonly LanguageDetails LA = new("Latin", "latine", "LA");
    public static readonly LanguageDetails LB = new("Luxembourgish", "Lëtzebuergesch", "LB");
    public static readonly LanguageDetails LG = new("Ganda", "Luganda", "LG");
    public static readonly LanguageDetails LI = new("Limburgish", "Limburgs", "LI");
    public static readonly LanguageDetails LN = new("Lingala", "Lingála", "LN");
    public static readonly LanguageDetails LO = new("Lao", "ພາສາລາວ", "LO");
    public static readonly LanguageDetails LT = new("Lithuanian", "lietuvių kalba", "LT");
    public static readonly LanguageDetails LU = new("Luba-Katanga", "Kiluba", "LU");
    public static readonly LanguageDetails LV = new("Latvian", "latviešu valoda", "LV");
    public static readonly LanguageDetails MG = new("Malagasy", "fiteny malagasy", "MG");
    public static readonly LanguageDetails MH = new("Marshallese", "Kajin M̧ajeļ", "MH");
    public static readonly LanguageDetails MI = new("Māori", "te reo Māori", "MI");
    public static readonly LanguageDetails MK = new("Macedonian", "македонски јазик", "MK");
    public static readonly LanguageDetails ML = new("Malayalam", "മലയാളം", "ML");
    public static readonly LanguageDetails MN = new("Mongolian", "Монгол хэл", "MN");
    public static readonly LanguageDetails MR = new("Marathi", "मराठी", "MR");
    public static readonly LanguageDetails MS = new("Malay", "Bahasa Melayu", "MS");
    public static readonly LanguageDetails MT = new("Maltese", "Malti", "MT");
    public static readonly LanguageDetails MY = new("Burmese", "ဗမာစာ", "MY");
    public static readonly LanguageDetails NA = new("Nauru", "Dorerin Naoero", "NA");
    public static readonly LanguageDetails NB = new("Norwegian Bokmål", "Norsk bokmål", "NB");
    public static readonly LanguageDetails ND = new("Northern Ndebele", "isiNdebele", "ND");
    public static readonly LanguageDetails NE = new("Nepali", "नेपाली", "NE");
    public static readonly LanguageDetails NG = new("Ndonga", "Owambo", "NG");
    public static readonly LanguageDetails NL = new("Dutch", "Nederlands", "NL");
    public static readonly LanguageDetails NN = new("Norwegian Nynorsk", "Norsk nynorsk", "NN");
    public static readonly LanguageDetails NO = new("Norwegian", "Norsk", "NO");
    public static readonly LanguageDetails NR = new("Southern Ndebele", "isiNdebele", "NR");
    public static readonly LanguageDetails NV = new("Navajo", "Diné bizaad", "NV");
    public static readonly LanguageDetails NY = new("Chichewa", "chiCheŵa", "NY");
    public static readonly LanguageDetails OC = new("Occitan", "occitan", "OC");
    public static readonly LanguageDetails OJ = new("Ojibwe", "ᐊᓂᔑᓈᐯᒧᐎᓐ", "OJ");
    public static readonly LanguageDetails OM = new("Oromo", "Afaan Oromoo", "OM");
    public static readonly LanguageDetails OR = new("Oriya", "ଓଡ଼ିଆ", "OR");
    public static readonly LanguageDetails OS = new("Ossetian", "ирон æвзаг", "OS");
    public static readonly LanguageDetails PA = new("Panjabi", "ਪੰਜਾਬੀ", "PA");
    public static readonly LanguageDetails PI = new("Pāli", "पाऴि", "PI");
    public static readonly LanguageDetails PL = new("Polish", "Polski", "PL");
    public static readonly LanguageDetails PS = new("Pashto", "پښتو", "PS");
    public static readonly LanguageDetails PT = new("Portuguese", "Português", "PT");
    public static readonly LanguageDetails QU = new("Quechua", "Runa Simi", "QU");
    public static readonly LanguageDetails RM = new("Romansh", "rumantsch grischun", "RM");
    public static readonly LanguageDetails RN = new("Kirundi", "Ikirundi", "RN");
    public static readonly LanguageDetails RO = new("Romanian", "Română", "RO");
    public static readonly LanguageDetails RU = new("Russian", "Русский", "RU");
    public static readonly LanguageDetails RW = new("Kinyarwanda", "Ikinyarwanda", "RW");
    public static readonly LanguageDetails SA = new("Sanskrit", "संस्कृतम्", "SA");
    public static readonly LanguageDetails SC = new("Sardinian", "sardu", "SC");
    public static readonly LanguageDetails SD = new("Sindhi", "सिन्धी", "SD");
    public static readonly LanguageDetails SE = new("Northern Sami", "Davvisámegiella", "SE");
    public static readonly LanguageDetails SG = new("Sango", "yângâ tî sängö", "SG");
    public static readonly LanguageDetails SI = new("Sinhala", "සිංහල", "SI");
    public static readonly LanguageDetails SK = new("Slovak", "slovenčina", "SK");
    public static readonly LanguageDetails SL = new("Slovenian", "slovenščina", "SL");
    public static readonly LanguageDetails SM = new("Samoan", "gagana fa'a Samoa", "SM");
    public static readonly LanguageDetails SN = new("Shona", "chiShona", "SN");
    public static readonly LanguageDetails SO = new("Somali", "Soomaaliga", "SO");
    public static readonly LanguageDetails SQ = new("Albanian", "Shqip", "SQ");
    public static readonly LanguageDetails SR = new("Serbian", "српски језик", "SR");
    public static readonly LanguageDetails SS = new("Swati", "SiSwati", "SS");
    public static readonly LanguageDetails ST = new("Southern Sotho", "Sesotho", "ST");
    public static readonly LanguageDetails SU = new("Sundanese", "Basa Sunda", "SU");
    public static readonly LanguageDetails SV = new("Swedish", "Svenska", "SV");
    public static readonly LanguageDetails SW = new("Swahili", "Kiswahili", "SW");
    public static readonly LanguageDetails TA = new("Tamil", "தமிழ்", "TA");
    public static readonly LanguageDetails TE = new("Telugu", "తెలుగు", "TE");
    public static readonly LanguageDetails TG = new("Tajik", "тоҷикӣ", "TG");
    public static readonly LanguageDetails TH = new("Thai", "ไทย", "TH");
    public static readonly LanguageDetails TI = new("Tigrinya", "ትግርኛ", "TI");
    public static readonly LanguageDetails TK = new("Turkmen", "Türkmençe", "TK");
    public static readonly LanguageDetails TL = new("Tagalog", "Wikang Tagalog", "TL");
    public static readonly LanguageDetails TN = new("Tswana", "Setswana", "TN");
    public static readonly LanguageDetails TO = new("Tonga", "faka Tonga", "TO");
    public static readonly LanguageDetails TR = new("Turkish", "Türkçe", "TR");
    public static readonly LanguageDetails TS = new("Tsonga", "Xitsonga", "TS");
    public static readonly LanguageDetails TT = new("Tatar", "татар теле", "TT");
    public static readonly LanguageDetails TW = new("Twi", "Twi", "TW");
    public static readonly LanguageDetails TY = new("Tahitian", "Reo Tahiti", "TY");
    public static readonly LanguageDetails UG = new("Uyghur", "ئۇيغۇرچە‎", "UG");
    public static readonly LanguageDetails UK = new("Ukrainian", "Українська", "UK");
    public static readonly LanguageDetails UR = new("Urdu", "اردو", "UR");
    public static readonly LanguageDetails UZ = new("Uzbek", "Ўзбек", "UZ");
    public static readonly LanguageDetails VE = new("Venda", "Tshivenḓa", "VE");
    public static readonly LanguageDetails VI = new("Vietnamese", "Tiếng Việt", "VI");
    public static readonly LanguageDetails VO = new("Volapük", "Volapük", "VO");
    public static readonly LanguageDetails WA = new("Walloon", "walon", "WA");
    public static readonly LanguageDetails WO = new("Wolof", "Wollof", "WO");
    public static readonly LanguageDetails XH = new("Xhosa", "isiXhosa", "XH");
    public static readonly LanguageDetails YI = new("Yiddish", "ייִדיש", "YI");
    public static readonly LanguageDetails YO = new("Yoruba", "Yorùbá", "YO");
    public static readonly LanguageDetails ZA = new("Zhuang", "Saɯ cueŋƅ", "ZA");
    public static readonly LanguageDetails ZH = new("Chinese", "中文", "ZH");
    public static readonly LanguageDetails ZU = new("Zulu", "isiZulu", "ZU");

    public static IEnumerable<LanguageDetails> GetAll()
    {
        var fields = typeof(LanguagesList)
            .GetFields(BindingFlags.Static |
                           BindingFlags.Public)
            .Where(x => x.FieldType == typeof(LanguageDetails));
        
        foreach (var field in fields)
        {
            yield return (LanguageDetails)field.GetValue(null)!;
        }
    }
}
