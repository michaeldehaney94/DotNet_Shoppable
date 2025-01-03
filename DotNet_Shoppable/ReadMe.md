Install the following NuGet packages: <br/>
------------------------------------------------ <br/>
Microsoft.EntityFrameworkCore <br/>
Microsoft.EntityFrameworkCore.Tools <br/>
Npgsql.EntityFrameworkCore.PostgreSQL (NOTE. If you are using PostgreSQL) <br/>
Microsoft.EntityFrameworkCore.SqlServer (NOTE. If you are using SQL Server) <br/>
Microsoft.AspNetCore.Identity.EntityFrameworkCore (NOTE. This package creates IAM tables and functions automatically)<br/>
sib_api_v3_sdk (Brevo email automation package)<br/>
<br/><br/>
Run Migration<br/>
----------------<br/>
In Package Manager CLI: 
1. run 'Add-Migration FirstMigration' <br/>
2. run Update-Database <br/>

<br/><br/>
<div class="text-center">
	<img src="/DotNet_Shoppable/wwwroot/shoppable-logo.jpg" />
</div><br/><br/>

How to Send Automated Email?<br/>
-------------------------<br/>
Go to 'https://www.brevo.com/' and create an account <br/>
==== Test Account ===<br/>
Email: michael.dehaney94@outlook.com <br/>
Password: *t3ZggS#*5 <br/><br/>

On the free tier you can send up to 300 emails per day for $0 /month. <br/>
After setting up account, go to dashboard 'https://app.brevo.com/'. <br/>
Go to Transactional > Settings > Configuration > click 'Get your API key' > <br/> 
click 'Generate a new API key' > then copy & save the key in 'appsettings.json'.<br/>
Go to Brevo documentation to get the C# API config code "https://developers.brevo.com/reference/sendtransacemail". <br/>
Add 'Configuration.Default.ApiKey.Add("api-key", "YOUR API KEY");' to 'Program.cs' to access api key in appsettings <br/>
Create a new class in 'Data' Folder to configure the Email Automation service <br/>
You can view Brevo statistics and logs of automated email attempts sent, delivered or failed from 'https://app-smtp.brevo.com/statistics' dashboard.<br/>
<br/>


What is Shoppable? <br/>
Shoppable is a ecommerce .NET web application built to sell good and services for clients and sellers, <br/>
who wish to start their online business or grow their existing business to reach a wider target market. 

