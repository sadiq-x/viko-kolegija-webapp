import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { EventListResponse } from '../models/modelEvents';
import { ModelListParticipants } from '../models/modelParticipant';

@Injectable({
  providedIn: 'root',
})
export class TeacherService {
  private apiUrlCreateEvent = environment.apiUrl + 'create/events';
  private apiUrlGetEvent = environment.apiUrl + 'get/byId/topics';
  private apiUrlGetParticipants = environment.apiUrl + 'get/participants/';
  private apiUrlInsertGradeParticipant = environment.apiUrl + 'insert/participant/grade';
  private apiUrlUpdateStatusParticipant = environment.apiUrl + 'update/participant/status';
  private apiUrlDeleteEvent = environment.apiUrl + 'delete/event';

  constructor(private http: HttpClient) {}
  //Create event in database
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
  //Get a event with a specific id
  getEventById(obj: { CreateById: number }): Observable<EventListResponse | false> {
    return this.http.post<any>(this.apiUrlGetEvent, obj).pipe(
      map((response) => {
        if (response?.success && response?.events) {
          return response.events;
        }
        return false as const;
      })
    );
  }
  //Get the participants of a specific event
  getParticipantsIndividualEvent(eventId: number): Observable<ModelListParticipants | false> {
    return this.http.get<any>(`${this.apiUrlGetParticipants}${eventId}`).pipe(
      map((response) => {
        if (response?.success && response.participantsEvent) {
          return response.participantsEvent;
        } else if (!response.success) {
          return false as const;
        }
      })
    );
  }
  //Insert grade and if exist comments of a student
  insertParticipantGrade(obj: {
    Id: number;
    EventId: number;
    Grade: string;
    Comments?: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlInsertGradeParticipant, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
  //Update status of a student
  updateParticipantStatus(obj: {
    Id: number;
    EventId: number;
    EntityId: number;
  }): Observable<boolean> {
    return this.http.put<any>(this.apiUrlUpdateStatusParticipant, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
  //Close "delete" the event by id
  deleteEventById(obj: { Id: number; CreateById: number }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlDeleteEvent, obj).pipe(
      map((response) => {
        console.log(response)
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
}
