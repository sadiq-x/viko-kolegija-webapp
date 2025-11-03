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
  private apiUrlGetTopics = environment.apiUrl + 'get/topics';

  constructor(private http: HttpClient){}

  getTopics(): Observable<ModelTopicsResponse | false>{
    return this.http.get<any>(this.apiUrlGetTopics).pipe(
      map((response) => {
        if (response?.success && Array.isArray(response.topics)) {
          return response.topics;
        };

        //return false as const;
        return of<false>(false);
      })
    )
  }
}
