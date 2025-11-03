import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of, Observable, pipe } from 'rxjs';
import { ModelUserProfileResponse } from '../models/modelUser';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  //Backend endpoint
  private apiUrlGetProfile = environment.apiUrl + 'get/profile';
  private apiUrlUpdateProfile = environment.apiUrl + 'update/profile';

  constructor(private http: HttpClient) { }

  //Function getProfile with return type = ModelUserProfile or False
  getProfile(): Observable<ModelUserProfileResponse | false> {
    //Http request get
    return this.http.get<any>(this.apiUrlGetProfile).pipe(
      map((response) => {
        if (response?.user) {
          return response.user;
        }
        return false as const;
      })
    );
  }

  updateProfile(obj: { EntityId: number, Username: string, Email: string, Image: string, NumberPhone: string, Address: string, Birthday: string, Nationality: string, Gender:string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdateProfile, obj).pipe(
      map((response) => {
        return response?.success === true;
      })
    )
  }
}