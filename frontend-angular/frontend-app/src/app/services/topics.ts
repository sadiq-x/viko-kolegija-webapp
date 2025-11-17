import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { flatMap, map, Observable, of } from 'rxjs';
import { ModelTopicsResponse } from '../models/modelTopics';

@Injectable({
  providedIn: 'root',
})
export class TopicsService {
  //Backend endpoint
  //Get
  private apiUrlGetTopics = environment.apiUrl + 'get/topics';
  //Insert
  private apiUrlInsertTopics = environment.apiUrl + 'insert/topic';
  //Delete
  private apiUrlDeleteTopics = environment.apiUrl + 'delete/topic';

  constructor(private http: HttpClient) {}
  //Function getTopics will get all existents topics
  getTopics(): Observable<ModelTopicsResponse | false> {
    return this.http.get<any>(this.apiUrlGetTopics).pipe(
      map((response) => {
        if (response?.success && Array.isArray(response.topics)) {
          return response.topics;
        }

        //return false as const;
        return of<false>(false);
      })
    );
  }
  //Function insertTopics will insert new topic
  insertTopics(obj: { Type: string; Description: string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlInsertTopics, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        }

        return false as const;
      })
    );
  }
  //Function deleteTopics will delete a specific topic
  deleteTopics(obj: { Id: number; Type: string; Description: string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlDeleteTopics, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        }

        return false as const;
      })
    );
  }
}
