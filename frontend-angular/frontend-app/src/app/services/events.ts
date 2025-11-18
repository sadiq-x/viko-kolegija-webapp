import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import {
  EventListResponse,
  EventParticipantListResponse,
  ModelEventsRequest,
} from '../models/modelEvents';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class EventService {
  //Backend endpoint
  //Get
  private apiUrlGetEvents = environment.apiUrl + 'get/events';
  private apiUrlGetEventsByTopic = environment.apiUrl + 'get/events/byTopics';
  private apiUrlGetEventByCreatedById = environment.apiUrl + 'get/events/byCreatedById';
  private apiUrlGetEventByEntityId = environment.apiUrl + 'get/events/byEntityId';
  private apiUrlGetEventByEventId = environment.apiUrl + 'get/events/byEventId/';
  //Create
  private apiUrlCreateEvent_teacher = environment.apiUrl + 'create/events/teacher';
  private apiUrlCreateEvent_admin = environment.apiUrl + 'create/events/admin';
  //Update
  private apiUrlUpdateEventStatusClose = environment.apiUrl + 'update/event/close';
  private apiUrlUpdateEventStatusOngoing = environment.apiUrl + 'update/event/ongoing';
  private apiUrlUpdateEvent_admin = environment.apiUrl + 'update/event';

  constructor(private http: HttpClient) {}
  //Function getEvents will get all Events
  getEvents(): Observable<EventListResponse | false> {
    return this.http.get<any>(this.apiUrlGetEvents).pipe(
      map((response) => {
        if (response?.success && response?.events) {
          return response.events;
        }
        return false as const;
      })
    );
  }
  //Function getEventById will get a Event with a specific CreatedById
  getEventByCreatedById(): Observable<EventListResponse | false> {
    return this.http.get<any>(this.apiUrlGetEventByCreatedById).pipe(
      map((response) => {
        if (response?.success && response?.events) {
          return response.events;
        }
        return false as const;
      })
    );
  }
  //Function getEventById will get a Event with a specific Topic
  getEventByTopic(obj: { Topic: string }): Observable<EventListResponse | false> {
    return this.http.post<any>(this.apiUrlGetEventsByTopic, obj).pipe(
      map((response) => {
        if (response?.success && response?.events) {
          return response.events;
        }
        return false as const;
      })
    );
  }
  //Function getEventByEntityId will get a Event with a specific EntityId
  getEventByEntityId(): Observable<EventParticipantListResponse | false> {
    return this.http.get<any>(this.apiUrlGetEventByEntityId).pipe(
      map((response) => {
        if (response?.success && response?.events) {
          return response.events;
        }
        return false as const;
      })
    );
  }
  //Function getEventByEventId will get a Event with a specific EventId
  getEventByEventId(eventId: number): Observable<EventListResponse | false> {
    return this.http.get<any>(`${this.apiUrlGetEventByEventId}${eventId}`).pipe(
      map((response) => {
        if (response?.success && response?.eventResponse) {
          return response.eventResponse;
        }
        return false as const;
      })
    );
  }
  //Function createEvent_teacher will create new Event from a specific teacher
  createEvent_teacher(obj: ModelEventsRequest): Observable<boolean> {
    return this.http.post<any>(this.apiUrlCreateEvent_teacher, obj).pipe(
      map((response) => {
        if (!response?.success) {
          return false as const;
        }
        return true;
      })
    );
  }
  //Function createEvent_admin will create new Event from a admin, the admin can select everything (Name + Description + Topic + Teacher)
  createEvent_admin(obj: {
    Name: string;
    Description: string;
    TopicsId: number;
    CreateById: number;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlCreateEvent_admin, obj).pipe(
      map((response) => {
        if (!response?.success) {
          return false as const;
        }
        return true;
      })
    );
  }
  //Function updateEventStatusClose will set status Close in a Event with a specific Id and CreateById
  updateEventStatusClose(obj: { Id: number }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdateEventStatusClose, obj).pipe(
      map((response) => {
        console.log(response);
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
  //Function updateEventStatusOngoing will set status Ongoing in a Event with a specific Id and CreateById
  updateEventStatusOngoing(obj: { Id: number }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdateEventStatusOngoing, obj).pipe(
      map((response) => {
        console.log(response);
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
  //Function updateEvent_admin will update a full event from admin
  updateEvent_admin(obj: {
    Id: number;
    Name: string;
    Description: string;
    Type: string;
    CreateBy: string;
    DateCreate: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdateEvent_admin, obj).pipe(
      map((response) => {
        console.log(response);
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
}
