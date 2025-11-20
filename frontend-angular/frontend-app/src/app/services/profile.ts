import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ModelUserProfileResponse } from '../models/modelUser';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  //Backend endpoint
  //Get
  private apiUrlGetProfile = environment.apiUrl + 'get/profile';
  //Update
  private apiUrlUpdateProfile = environment.apiUrl + 'update/profile';

  constructor(private http: HttpClient) {}

  //Function getProfile with return type = ModelUserProfile or False
  getProfile(): Observable<ModelUserProfileResponse | false> {
    //Http request get
    return this.http.get<any>(this.apiUrlGetProfile).pipe(
      map((response) => {
        console.log(response)
        if (response?.user && response?.success) {
          return response.user;
        }
        return false as const; 
      })
    );
  }
  //Function updateProfile will update all information of user
  updateProfile(obj: {
    Username: string;
    Email: string;
    Image: string;
    NumberPhone: string;
    Address: string;
    Birthday: string;
    Nationality: string;
    Gender: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdateProfile, obj).pipe(
      map((response) => {
        console.log(response)
        //test this
        return response?.success === true;
      })
    );
  }
}
