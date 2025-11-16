import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { ModelListParticipants } from '../models/modelParticipant';
import { catchError, map, Observable } from 'rxjs';
import { errorContext } from 'rxjs/internal/util/errorContext';

@Injectable({
  providedIn: 'root',
})
export class ParticipantsService {
  //Backend endpoint
  //Get
  private apiUrlGetParticipants_user = environment.apiUrl + 'get/participants/';
  private apiUrlGetParticipants_teacher = environment.apiUrl + 'get/participants/teacher/';
  //Insert
  private apiUrlInsertParticipantInEvent = environment.apiUrl + 'insert/participant/event';
  private apiUrlInsertParticipantParticipantDescription = environment.apiUrl + 'insert/participant/description';
  private apiUrlInsertGradeParticipant = environment.apiUrl + 'insert/participant/grade';
  //Update
  private apiUrlUpdateStatusParticipant = environment.apiUrl + 'update/participant/status';

  constructor(private http: HttpClient) {}

  //Function getParticipantsIndividualEvent_user will get participants of a specific event
  getParticipantsIndividualEvent_user(eventId: number): Observable<ModelListParticipants | false> {
    return this.http.get<any>(`${this.apiUrlGetParticipants_user}${eventId}`).pipe(
      map((response) => {
        if (response?.success && response?.participantsEvent) {
          return response.participantsEvent;
        } else if (!response.success) {
          return false as const;
        }
      })
    );
  }
  //Function getParticipantsIndividualEvent_teacher will get participants of a specific event
  getParticipantsIndividualEvent_teacher(eventId: number): Observable<ModelListParticipants | false> {
    return this.http.get<any>(`${this.apiUrlGetParticipants_teacher}${eventId}`).pipe(
      map((response) => {
        if (response?.success && response?.participantsEvent) {
          return response.participantsEvent;
        } else if (!response.success) {
          return false as const;
        }
      })
    );
  }
  //Function insertParticipantsInEvent will insert participant of a specific event
  insertParticipantsInEvent(obj: { eventId: number }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlInsertParticipantInEvent, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        } else {
          return false as const;
        }
      })
    );
  }
  //Function insertParticipantParticipantDescription will insert description at a specific participant
  insertParticipantParticipantDescription(obj: {
    eventId: number;
    participantDescription: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlInsertParticipantParticipantDescription, obj).pipe(
      map((response) => {
        console.log(response);
        if (response?.success) {
          return true as const;
        } else {
          return false as const;
        }
      })
    );
  }
  //Function insertParticipantGrade will insert grade and if exist comments of a participant
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
  //Function updateParticipantStatus will update status of a participant
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
}
