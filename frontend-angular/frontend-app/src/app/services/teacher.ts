import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { EventListResponse } from '../models/modelEvents';
import { ModelListParticipants } from '../models/modelParticipant';
import { ModelTeacherResponse } from '../models/modelTeacher';

@Injectable({
  providedIn: 'root',
})
export class TeacherService {
  //Backend endpoints
  //Get
  private apiUrlGetTeachers = environment.apiUrl + 'get/teachers';

  constructor(private http: HttpClient) {}

  //Function getTeachers will get all existents teachers
  getTeachers(): Observable<ModelTeacherResponse | false> {
    return this.http.get<any>(this.apiUrlGetTeachers).pipe(
      map((response) => {
        console.log(response.teachers);
        if (response?.success && response?.teachers) {
          return response.teachers;
        }

        return false as const;
      })
    );
  }
}
