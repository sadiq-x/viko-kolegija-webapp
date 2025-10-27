import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { EventListResponse } from '../models/modelEvents';
import { ModelListParticipants } from '../models/modelParticipant';
import { errorContext } from 'rxjs/internal/util/errorContext';

@Injectable({
  providedIn: 'root',
})
export class TeacherService {
  private apiUrlCreateEvent = environment.apiUrl + 'create/events';
  private apiUrlGetEvent = environment.apiUrl + 'get/byId/topics';
  private apiUrlParticipants = environment.apiUrl + 'get/participants/';

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
    return this.http.get<any>(`${this.apiUrlParticipants}${eventId}`).pipe(
      map((response) => {
        if (response.success) {
          return response.participantsEvent;
        }else if(!response.success){
          return false as const;
        }
      })
    );
  }
}
