# todolist_server
SQL Server Installation
1. Open port 80 & 443 for incoming HTTP request in firewall
2. Install IIS and Websocket from Server Manager (not needed here xdd)
3. Downlaod and install SQL Server
4. Download the .zip file in github and open in visual studio
5. Install .Net Core Hosting Bundle **(remember to download correct version)** https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/hosting-bundle?view=aspnetcore-8.0
6. Open SQL Server Object Explore in Visual Studio, add SQLEXPRESS server and choose True for Trust Server Certificate
7. Get the connection string then revise the ConnectionStrings in appsettings.json
8. Remove Migration Folder and type command in Package Manage Console (Add-Migration InitialCreate -> Update-Database) https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs
9. Open SSMS -> Logins -> New Logins -> Search -> type (LOCAL SERVICE) -> Check Names -> OK
10. Open SSMS -> todolist (database) -> Security -> Users -> New Users -> Login name ... -> User name can be same as Login name -> OK
11. todolist (database) -> Security -> Users -> (NT AUTHORITY\LOCAL SERVICE) -> properties -> membership -> tick db_datareader & db_datawriter & db_owner
12. Publish asp.net core (portable) version in designated folder for IIS
13. Open properties of the designated folder -> Security -> Edit -> type (LOCAL SERVICE) -> add permission for the folder
14. Create a new site in IIS and Change identity of the todolist site to LOCALSERVICE
