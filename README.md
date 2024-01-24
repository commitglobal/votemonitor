# votemonitor

## Technologies & Libraries
* [FastEndpoints](https://fast-endpoints.com/)
* [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
* [Ardalis.Specification](https://github.com/ardalis/specification)
* [Ardalis.SmartEnum](https://github.com/ardalis/SmartEnum)
* [XUnit](https://xunit.net/)
* [TestContainers](https://testcontainers.com/)
* [FluentAssertions](https://fluentassertions.com/)
* [Bogus](https://github.com/bchavez/Bogus)

## Getting started using docker

1. Rename `.env.example` to `.env`
2. Build project
    ```
    docker compose build
    ```
3. Start project
    ```
    docker compose up -d 
    ```
4. Seed platform admin
```sql
    INSERT INTO public."Users" VALUES ('771b0bb0-3f87-47a0-bc64-0878e2070374', 'PlatformAdmin', '<your-username>', '<your-password>', 'PlatformAdmin', 'Active', '00000000-0000-0000-0000-000000000000', '2024-01-23 00:00:00+00', '00000000-0000-0000-0000-000000000000', NULL);
    INSERT INTO public."PlatformAdmins" VALUES ('771b0bb0-3f87-47a0-bc64-0878e2070374')
```
5. Navigate to http://localhost:5000/swagger/index.html
6. Obtain token
```
    POST /api/auth
    {
        "username": "<your-username>",
        "password": "<your-password>"
    }
```
7. Enjoy

## Getting started using VisualStudio
1. start an postgres instance
2. update appsettings.Development.json
3. Build
4. Run and debug
5. Enjoy 
# Polling stations feature
[documentation](documentation/polling-stations/README.md)
