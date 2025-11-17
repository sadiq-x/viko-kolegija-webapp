import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RolesService {
  //Update
  private apiUrlUpdateRole = environment.apiUrl + 'update/role';

  constructor(private http: HttpClient) {}

  //Function updateParticipantStatus will update role of a specific user
  updateParticipantStatus(obj: { Id: number; Email: string; Type: string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdateRole, obj).pipe(
      map((response) => {
        if (response?.success) {
          return true as const;
        }
        return false as const;
      })
    );
  }
}
