# Erasmus + Internship

<img src="logo-viko.png" width="200">

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
* Create training, assign trainer, see participants
* Data adjustment.
5. (Optional) Creation of web components, e.g. 
* CourseCardComponent, ProgressBarComponent to show UI skills.
---
# Tools used
* DrawSQL - Flowcharts SQL.
* Lucidchart - Flowchart of the whole application. 
* Figma - Design the logic of the entire application.
* VsCode - Software to develop.
* Docker - Software to up database.
# Programming Languages
* Backend - C# - Azure Functions.
* Frontend - Typescrypt with Angular and TailwindCss.
* Database - SQL with Docker composer.
---
# Backend
## Available Endpoints
>Auth
* auth/update/password
* auth/register
* auth/login
>Profile 
* get/users
* get/profile
* update/profile
>Events
* get/events
* get/events/byCreatedById
* get/events/byTopics
* get/events/byEntityId
* get/events/byEventId/{eventId:int}
* create/events/teacher
* create/events/admin
* update/event
* update/event/close
* update/event/ongoing
>Topics
* get/topics
* insert/topic
* delete/topic
>Roles
* auth/roles
* update/role
>Participants
* get/participants/{eventId:int}
* get/participants/teacher/{eventId:int}
* insert/participant/description
* insert/participant/event
* insert/participant/grade
* update/participant/status
* update/participant/cancelEvent
>Teachers
* get/teachers


## Evironments Settings
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

![flowchart](/Flowcharts/SQL%20Database%20V3/Flowchart%20-%20SQL%20Database.png)

## Users

|   Campo    |   Tipo   |
|:----------:|:--------:|
|     id     |    int   |
| entity_id  |    int   |
|   email    | varchar  |
| passwordHash | varchar |

---

## Entity

|     Campo     |   Tipo   |
|:-------------:|:--------:|
|      id       |    int   |
|     name      | varchar  |
|     email     | varchar  |
|     image     | varchar  |
|  numberPhone  | varchar  |
|    address    | varchar  |
|   birthday    | varchar  |
| nationality   | varchar  |
|    gender     | varchar  |
|    role_id    |    int   |

---

## Roles

| Campo |  Tipo   |
|:-----:|:-------:|
|  id   |   int   |
| type  | varchar |

---

## Events

|     Campo      |   Tipo   |
|:--------------:|:--------:|
|       id       |    int   |
|      name      | varchar  |
|  description   | varchar  |
|    topic_id    |    int   |
| create_by_id   |    int   |
|  date_create   | varchar  |
|  date_close    | varchar  |
|   status_id    |    int   |

---

## Topics

|    Campo     |   Tipo   |
|:------------:|:--------:|
|      id      |    int   |
|     type     | varchar  |
| description  | varchar  |

---

## StatusEvent

| Campo |  Tipo   |
|:-----:|:-------:|
|  id   |   int   |
| type  | varchar |

---

## ParticipantsEvents

|         Campo          |   Tipo   |
|:----------------------:|:--------:|
|          id            |    int   |
|       event_id         |    int   |
|      entity_id         |    int   |
|        status          | boolean  |
|         grade          | varchar  |
|       comments         | varchar  |
| participantDescription | varchar  |