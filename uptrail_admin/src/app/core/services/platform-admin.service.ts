import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PlatformAdminResponse } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class PlatformAdminService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/PlatformAdmin`;

  getPlatformAdmins(): Observable<ApiResponse<PlatformAdminResponse[]>> {
    return this.http.get<ApiResponse<PlatformAdminResponse[]>>(this.apiUrl);
  }

  searchPlatformAdmins(searchTerm: string): Observable<ApiResponse<PlatformAdminResponse[]>> {
    const params = new HttpParams().set('searchTerm', searchTerm.trim());
    return this.http.get<ApiResponse<PlatformAdminResponse[]>>(`${this.apiUrl}/search`, { params });
  }

  getPlatformAdminById(id: string): Observable<ApiResponse<PlatformAdminResponse>> {
    return this.http.get<ApiResponse<PlatformAdminResponse>>(`${this.apiUrl}/${id}`);
  }

  deletePlatformAdmin(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
