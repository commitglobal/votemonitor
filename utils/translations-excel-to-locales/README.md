
```
    _______  ________________        __________
   / ____/ |/ / ____/ ____/ /       /_  __/ __ \
  / __/  |   / /   / __/ / /         / / / / / /
 / /___ /   / /___/ /___/ /___      / / / /_/ /
/_____//_/|_\____/_____/_____/_    /_/__\____/
   / /   / __ \/ ____/   |  / /   / ____/ ___/
  / /   / / / / /   / /| | / /   / __/  \__ \
 / /___/ /_/ / /___/ ___ |/ /___/ /___ ___/ /
/_____/\____/\____/_/  |_/_____/_____//____/
```

# What it does ?
 Transforms flattened translations CSV to locales for mobile app

# How to run ?
`node index.js -l locales.csv`
# How to debug ?
Add .vscode/launch.json
```json
{
  // Use IntelliSense to learn about possible attributes.
  // Hover to view descriptions of existing attributes.
  // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
  "version": "0.2.0",
  "configurations": [
    {
      "type": "node",
      "request": "launch",
      "name": "Generate locales",
      "program": "${workspaceFolder}/index.js",
      "args": [
        "-l",
        "./locales.csv"
      ],
      "runtimeExecutable": "<path-to-node>"
    }
  ]
}
```