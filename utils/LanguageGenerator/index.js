const LANGUAGES_LIST = require('./LanguagesListJson.js');
const fs = require('fs');

let csharpCode = `public static class LanguagesList\n{\n`;
const sortedLanguages = Object.entries(LANGUAGES_LIST).sort((a, b) => a[0].localeCompare(b[0]));
for (const [key, value] of sortedLanguages) {
  csharpCode += `    public static readonly LanguageDetails ${key.toUpperCase()} = new("${value.name}", "${value.nativeName}", "${key.toUpperCase()}");\n`;
}
csharpCode += `}`;

fs.writeFile('LanguagesListGenerated.cs', csharpCode, (err) => {
  if (err) {
    console.error('Error writing file:', err);
  } else {
    console.log('File written successfully!');
  }
});
