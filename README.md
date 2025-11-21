# Erasmus + Internship

![logo-viko](/logo-viko.png) 

### Project idea:
To create an internal system for an educational institution or project organiser where users can:
```
Register for training/events
View their progress or certificate data
Admins can create new courses, manage users and progress
```
### Functionality
1. User authentication (with roles: participant, instructor, administrator).
2. Training registration:
* Course list (filtering by date, topic, teacher)
* Registration, cancellation.
3. Course tracking / marking:
* List of grades, percentage of participation, upload of test results (simulated).
4. Admin environment:
*Create training, assign trainer, see participants
* Data adjustment.
5. (Optional) Creation of web components, e.g. 
* CourseCardComponent, ProgressBarComponent to show UI skills.
---
# Backend
### Endpoints

### Repositories

### Evironments Settings
1. **IsEncrypted**
2. **AzureWebJobsStorage**
3. **DefaultConnection** or **ConnectionStrings**
4. **CORS**

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "DefaultConnection": ""
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Host": {
    "CORS": "*"
  }
```
---
# Database
### SQL Flowchart

[![flowchart](/Flowcharts/SQL%20Database%20V2/Flowchart%20-%20SQL%20Database.png)](https://github.com/sadiq-x/viko-kolegija-webApp/blob/main/Flowcharts/SQL%20Database%20V2/drawSQL-image-export-2025-11-09.png)

