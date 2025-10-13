const fs = require("fs");
const path = require("path");
const { spawn } = require("child_process");
const inquirer = require("inquirer");

const APP_JSON_PATH = path.join(process.cwd(), "app.json");
const EAS_JSON_PATH = path.join(process.cwd(), "eas.json");

// Function to validate semver
function isValidSemver(version) {
  const semverRegex = /^\d+\.\d+\.\d+$/;
  return semverRegex.test(version);
}

// Function to run EAS build command
function runEasBuild(platform, profile, local) {
  const command = `eas build --platform ${platform} --profile ${profile}${local ? " --local" : ""}`;
  console.log(`\nExecuting: ${command}\n`);

  const build = spawn(
    "eas",
    ["build", "--platform", platform, "--profile", profile, ...(local ? ["--local"] : [])],
    {
      stdio: "inherit",
      shell: true,
    },
  );

  build.on("error", (error) => {
    console.error("Failed to start EAS build:", error);
  });
}

async function main() {
  try {
    // Check if required files exist
    if (!fs.existsSync(APP_JSON_PATH)) {
      console.error("Error: app.json not found");
      process.exit(1);
    }
    if (!fs.existsSync(EAS_JSON_PATH)) {
      console.error("Error: eas.json not found");
      process.exit(1);
    }

    // Read build profiles from eas.json
    let easConfig;
    try {
      const easJsonContent = fs.readFileSync(EAS_JSON_PATH, "utf8");
      console.log("EAS JSON content:", easJsonContent.substring(0, 100) + "...");
      easConfig = JSON.parse(easJsonContent);
    } catch (error) {
      console.error("Error parsing eas.json:", error);
      console.error("Error position:", error.message);
      process.exit(1);
    }

    const buildProfiles = Object.keys(easConfig.build || {});

    if (buildProfiles.length === 0) {
      console.error("Error: No build profiles found in eas.json");
      process.exit(1);
    }

    // Get user inputs
    const answers = await inquirer.prompt([
      {
        type: "list",
        name: "platform",
        message: "Select platform:",
        choices: ["android", "ios"],
      },
      {
        type: "list",
        name: "profile",
        message: "Select build profile:",
        choices: buildProfiles,
      },
      {
        type: "confirm",
        name: "local",
        message: "Build locally?",
        default: false,
      },
    ]);

    // Handle version update
    let appJson;
    try {
      const appJsonContent = fs.readFileSync(APP_JSON_PATH, "utf8");
      console.log("APP JSON content:", appJsonContent.substring(0, 100) + "...");
      appJson = JSON.parse(appJsonContent);
    } catch (error) {
      console.error("Error parsing app.json:", error);
      console.error("Error position:", error.message);
      process.exit(1);
    }

    const currentVersion = appJson.expo.version;

    const versionAnswer = await inquirer.prompt([
      {
        type: "input",
        name: "version",
        message: `Enter version to use (current: ${currentVersion}):`,
        default: currentVersion,
        validate: (input) => {
          if (!isValidSemver(input)) {
            return "Version must follow semver format (e.g., 1.0.0)";
          }
          return true;
        },
      },
    ]);

    const newVersion = versionAnswer.version;
    appJson.expo.version = newVersion;

    // Update build number/version code
    if (answers.platform === "ios") {
      const currentBuild = parseInt(appJson.expo.ios.buildNumber);
      appJson.expo.ios.buildNumber = String(currentBuild + 1);
      console.log(
        `iOS: Updated version to ${newVersion} and build number to ${appJson.expo.ios.buildNumber}`,
      );
    } else {
      const currentCode = appJson.expo.android.versionCode;
      appJson.expo.android.versionCode = currentCode + 1;
      console.log(
        `Android: Updated version to ${newVersion} and version code to ${appJson.expo.android.versionCode}`,
      );
    }

    // Write back to app.json
    try {
      const jsonString = JSON.stringify(appJson, null, 2);
      fs.writeFileSync(APP_JSON_PATH, jsonString);

      // Verify the written JSON is valid
      const verifyContent = fs.readFileSync(APP_JSON_PATH, "utf8");
      JSON.parse(verifyContent); // This will throw if invalid
      console.log("Successfully wrote and verified app.json");
    } catch (error) {
      console.error("Error writing or verifying app.json:", error);
      console.error("Error details:", error.message);
      process.exit(1);
    }

    // Run EAS build
    runEasBuild(answers.platform, answers.profile, answers.local);
  } catch (error) {
    console.error("Error:", error);
    console.error("Error message:", error.message);
    console.error("Error stack:", error.stack);
    process.exit(1);
  }
}

main();
