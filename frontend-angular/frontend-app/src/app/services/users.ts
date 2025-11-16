import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { map, Observable } from 'rxjs';


import { HttpClient } from '@angular/common/http';
import { ModelEntities } from '../models/modelEntity';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  //Backend endpoints
  //Get
  private apiUrlGetUsers = environment.apiUrl + 'get/users';

  constructor(private http: HttpClient) {}

  //Function getUsers will get all users
  getUsers(): Observable<ModelEntities | false> {
    return this.http.get<any>(this.apiUrlGetUsers).pipe(
      map((response) => {
        if (response?.success && response?.usersResponse) {
          return response.usersResponse;
        } else if (!response.success) {
          return false as const;
        }
      })
    );
  }
}
