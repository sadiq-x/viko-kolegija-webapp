import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { EventListResponse } from '../models/modelEvents';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class EventService {
  //Backend endpoint
  //Get
  private apiUrlGetEvents = environment.apiUrl + 'get/events';
  private apiUrlGetEventsByTopic = environment.apiUrl + 'get/events/byTopics';
  private apiUrlGetEventById = environment.apiUrl + 'get/events/byId';
  //Create
  private apiUrlCreateEvent = environment.apiUrl + 'create/events';
  //Delete
  private apiUrlDeleteEvent = environment.apiUrl + 'delete/event';

  constructor(private http: HttpClient) {}
  //Function getEvents will get all Events
  getEvents(){
    return this.http.get<any>(this.apiUrlGetEvents).pipe(
      map((response)=>{
        console.log(response)
        if(response?.success && response?.events){
          return response.events
        }
        return false as const
      })
    )
  }

  //Function getEventById will get a Event with a specific EntityId
  getEventById(): Observable<EventListResponse | false> {
    return this.http.get<any>(this.apiUrlGetEventById).pipe(
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
  //Function getEventById will create new Event
  createEvent(obj: {
    Name: string;
    Description: string;
    TopicsId: number;
    CreateById: number;
    DateCreate: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlCreateEvent, obj).pipe(
      map((response) => {
        if (!response?.success) {
          return false as const;
        }
        return true;
      })
    );
  }
  //Function getEventById will close "delete" Event with a specific Id and CreateById
  deleteEventById(obj: { Id: number; CreateById: number }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlDeleteEvent, obj).pipe(
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
