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
  private apiUrlGetParticipants = environment.apiUrl + 'get/participants/';
  //Insert
  private apiUrlInsertParticipantInEvent = environment.apiUrl + 'insert/participant/event';
  private apiUrlInsertParticipantParticipantDescription = environment.apiUrl + 'insert/participant/description';


  constructor(private http: HttpClient) {}

  //Function getParticipantsIndividualEvent will get participants of a specific event
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
  //Function insertParticipantsInEvent will insert participant of a specific event
  insertParticipantsInEvent(obj: { eventId: number }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlInsertParticipantInEvent, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        } else{
          return false as const;
        }
      })
    );
  }
  //Function insertParticipantParticipantDescription will insert description at a specific participant
  insertParticipantParticipantDescription(obj: { eventId: number, participantDescription: string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlInsertParticipantParticipantDescription, obj).pipe(
      map((response) => {
        console.log(response)
        if (response?.success) {
          return true as const;
        } else{
          return false as const;
        }
      })
    );
  }
}
