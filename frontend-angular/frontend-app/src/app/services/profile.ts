import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of, Observable, pipe } from 'rxjs';
import { ModelUserProfile } from '../models/modelUser';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  //Backend endpoint
  private apiUrlGetProfile = environment.apiUrl + 'get/profile';
  private apiUrlUpdateProfile = environment.apiUrl + 'update/profile';

  constructor(private http: HttpClient) { }

  //Function getProfile with return type = ModelUserProfile or False
  getProfile(): Observable<ModelUserProfile | false> {
    //Http request get
    return this.http.get<any>(this.apiUrlGetProfile).pipe(
      map((response) => {

        if (response?.user) {
          console.log("✅ Utilizador recebido do backend:", response.user);
          return response.user; //Return the 
        }

        return of<false>(false);
      }),
      catchError((error) => {
        return of<false>(false);
      })
    );
  }

  updateProfile(obj: { NumberPhone: string, Address: string, }) {

    return this.http.post(this.apiUrlUpdateProfile, obj).pipe(
      map((response) => {

      })
    )
  }
}