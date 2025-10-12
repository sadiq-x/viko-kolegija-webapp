import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of, Observable } from 'rxjs';
import { ModelUserProfile } from '../models/modelUser';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private apiUrlGetProfile = environment.apiUrl + 'get/profile';

  constructor(private http: HttpClient) {}

  // ✅ declara o tipo de retorno
  getProfile(): Observable<ModelUserProfile | false> {
    const storedUser = localStorage.getItem('authUser');

    if (!storedUser) {
      console.warn('⚠️ Nenhum utilizador no localStorage');
      return of<false>(false);
    }

    let parsed: any;
    try {
      parsed = JSON.parse(storedUser);
    } catch (error) {
      console.error('❌ Erro ao fazer parse do localStorage:', error);
      return of<false>(false);
    }

    if (!parsed?.entityId || !parsed?.username) {
      console.error('❌ Dados no localStorage inválidos:', parsed);
      return of<false>(false);
    }

    const obj: { EntityId: number; Username: string } = {
      EntityId: Number(parsed.entityId),
      Username: String(parsed.username),
    };

    console.log("📤 Payload enviado ao backend:", obj);

    // ✅ tipa a resposta esperada do backend
    return this.http.post<any>(this.apiUrlGetProfile, obj).pipe(
      map((response) => {

        if (response?.user) {
          console.log("✅ Utilizador recebido do backend:", response.user);
          return response.user;
        }

        console.warn("⚠️ Nenhum utilizador devolvido pelo backend");
        return false as const;
      }),
      catchError((error) => {
        console.error('❌ getProfile error:', error);
        return of<false>(false);
      })
    );
  }
}