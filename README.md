# OneDrive Backup Automation

This is my approach to my daily routine backup.

## Getting Started

- Create a 'apps registration' on Azure o get the following information: (client_id, and tenant_id) in 'Accounts in this organizational directory only...';
- Add authentication for 'Mobile and Desktop apps';
- Redirect URIs 'native client' URLs custom redirect 'http://localhost' allow public client flows;
- API Permission add Microsoft Graph with the following scopes: 'Files.Read'; 
- Change the files 'appsettings_rename.json' and 'automation_rename.bat';
- Rename the files to their original name withou '_rename';

## Running

```bash
# Running only the C# program
dotnet build
dotnet run

# Running with BAT file or you can put this script on schedule tasks
PS C:\..\cloud-automation> ./automation.bat
```