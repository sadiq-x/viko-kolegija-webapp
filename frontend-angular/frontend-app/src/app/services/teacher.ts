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
  //Backend endpoint
  //Get
  private apiUrlGetParticipants = environment.apiUrl + 'get/participants/';
  //Insert
  private apiUrlInsertGradeParticipant = environment.apiUrl + 'insert/participant/grade';
  //Update
  private apiUrlUpdateStatusParticipant = environment.apiUrl + 'update/participant/status';

  constructor(private http: HttpClient) {}
  //Function getParticipantsIndividualEvent will get participants of a specific event
  // getParticipantsIndividualEvent(eventId: number): Observable<ModelListParticipants | false> {
  //   return this.http.get<any>(`${this.apiUrlGetParticipants}${eventId}`).pipe(
  //     map((response) => {
  //       console.log(response)
  //       if (response?.success && response.participantsEvent) {
  //         return response.participantsEvent;
  //       } else if (!response.success) {
  //         return false as const;
  //       }
  //     })
  //   );
  // }
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
