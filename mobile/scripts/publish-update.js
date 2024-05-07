const path = require('path');
const fs = require('fs/promises');
const spawnAsync = require('@expo/spawn-async');
const readline = require('readline');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
});

const usage = () => {
  console.log('Usage: push-update.js ');
  console.log('  Parameters:');
  console.log(
    '    <--message|-m> (message) (required) Sets the message passed into the EAS update command',
  );
  console.log(
    '    <--branch|-b> (required) Sets the branch to send the update to. Must be one of: preview, staging, production (as per eas.json)',
  );
  console.log(
    '    <--critical|-c> (optional) If present, increments the criticalIndex counter to mark this update as critical',
  );
};

const pushUpdateAsync = async (message, branch, critical, projectRoot) => {
  console.log('Modifying app.json...');
  const appJsonPath = path.resolve(projectRoot, 'app.json');
  const appJsonOriginalText = await fs.readFile(appJsonPath, { encoding: 'utf-8' });
  const appJsonOriginal = JSON.parse(appJsonOriginalText);

  const currentCriticalIndex = appJsonOriginal.expo.extra.updateCritical || 0;
  const currentUpdateVersion = appJsonOriginal.expo.extra.updateVersion || 0;

  const appJson = {
    expo: {
      ...appJsonOriginal.expo,
      extra: {
        ...appJsonOriginal.expo.extra,
        message,
        updateCritical: critical ? currentCriticalIndex + 1 : currentCriticalIndex,
        updateVersion: currentUpdateVersion + 1,
      },
    },
  };
  const appJsonText = JSON.stringify(appJson, null, 2);
  await fs.rm(appJsonPath);
  await fs.writeFile(appJsonPath, appJsonText, { encoding: 'utf-8' });

  console.log('Publishing update...');

  await spawnAsync(
    'eas',
    ['update', `--message=${message}`, `--branch=${branch}`, '--clear-cache'],
    {
      stdio: 'inherit',
      path: projectRoot,
    },
  );

  console.log('Done.');
  process.exit(0);
};

const params = process.argv.filter((a, i) => i > 0);
const projectRoot = path.resolve(__dirname, '..');

let message = '';
let critical = false;
let branch = '';

while (params.length) {
  if (params[0] === '--message' || params[0] === '-m') {
    message = params[1];
    params.shift();
  }
  if (params[0] === '--critical' || params[0] === '-c') {
    critical = true;
    params.shift();
  }

  if (params[0] === '--branch' || params[0] === '-b') {
    branch = params[1];
    params.shift();
  }

  params.shift();
}

console.log(message, critical, branch);
console.log(process.env.NODE_ENV);

if (message.length === 0) {
  usage();
  process.exit(0);
}

if (!branch || !['preview', 'staging', 'production'].includes(branch)) {
  usage();
  console.log('Wrong branch');
  process.exit(0);
} else if (branch === 'production') {
  rl.question(
    `
    ======================================================
    ======================================================
    You are about to push updates LIVE, in PRODUCTION. Are you sure?
    ======================================================
    ====================================================== \n
    Type here (Yes/No): `,
    (answ) => {
      console.log(`You said: ${answ}`);
      if (answ !== 'Yes' && answ !== 'Y' && answ !== 'yes') {
        process.exit(0);
      } else {
        pushUpdateAsync(message, branch, critical, projectRoot).catch((error) =>
          console.log(`Error in script: ${error}`),
        );
      }
      rl.close();
    },
  );
} else {
  pushUpdateAsync(message, branch, critical, projectRoot).catch((error) =>
    console.log(`Error in script: ${error}`),
  );
}
