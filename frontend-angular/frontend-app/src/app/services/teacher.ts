import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { EventListResponse } from '../models/modelEvents';
import { ModelListParticipants } from '../models/modelParticipant';

@Injectable({
  providedIn: 'root',
})
export class TeacherService {
  //Not used

  constructor(private http: HttpClient) {}
 
  
}
