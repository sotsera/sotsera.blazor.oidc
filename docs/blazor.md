## Security
- https://docs.microsoft.com/en-us/aspnet/core/security/blazor/?view=aspnetcore-3.0&tabs=visual-studio
- Redirect if not authorized
  - https://gist.github.com/SteveSandersonMS/175a08dcdccb384a52ba760122cd2eda#gistcomment-2943728
- Notes
  - If you add any authorization attribute the authorization logic in the router will kick in and the templates
  put in the router configuration will be shown in the main layout body.