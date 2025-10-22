import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { map, Observable, of } from 'rxjs';
import { EventListResponse } from '../models/modelEvents';

@Injectable({
  providedIn: 'root',
})
export class TeacherService {
  private apiUrlCreateEvent = environment.apiUrl + 'create/events';
  private apiUrlGetEvent = environment.apiUrl + 'get/byId/topics';

  constructor(private http: HttpClient) {}

  createEvent(obj: {
    Name: string;
    Description: string;
    TopicsId: number;
    CreateById: number;
    DateCreate: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlCreateEvent, obj).pipe(
      map((response) => {
        console.log(response)
        if (!response?.success) {
          return false;
        }
        return true;
      })
    );
  }

  getEventById(obj: { CreateById: number }): Observable<EventListResponse | false> {
    return this.http.post<any>(this.apiUrlGetEvent, obj).pipe(
      map((response) => {
        console.log(response)
        if (response?.success && response?.events) {
          return response.events;
        }
        return of<false>(false);
      })
    );
  }
}
