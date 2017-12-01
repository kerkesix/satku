# Registration web site

## Local development

Add secrets to your env. Most convenient way is to crete one script and read that before running. On Windows:

```
echo "Setting secrets for local development"
setx storage:ConnectionString "DefaultEndpointsProtocol=..."
setx registration:email:APIKey "..."
refreshenv
```

To enable development mode do: `export ASPNETCORE_ENVIRONMENT=Development` before running `dotnet run`.

## Deployment

Add the secrets in the cloud app settings section. E.g. `registration:happening:id` could have value `satkuxvi`.