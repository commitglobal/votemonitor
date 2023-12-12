# Language Generator util

Generates the list of languages in C# starting from json.
Use `npm run start` to regenerate `LanguagesList.cs` starting from `LanguageJson.js`.

### Additional information

- Generated code relies on existing constructor `public LanguageDetails(string name, string nativeName, string iso1)`
- JSON credits: https://github.com/meikidd/iso-639-1/blob/master/src/data.js
