import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { CreateRoleRequest, RoleResponse, UpdateRoleRequest } from '../models/role.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Role`;

  getRoles(companyId?: string): Observable<ApiResponse<RoleResponse[]>> {
    let params = new HttpParams();
    if (companyId?.trim()) {
      params = params.set('companyId', companyId.trim());
    }
    return this.http.get<ApiResponse<RoleResponse[]>>(this.apiUrl, { params });
  }

  getRolesByCompanyId(companyId: string): Observable<ApiResponse<RoleResponse[]>> {
    return this.http.get<ApiResponse<RoleResponse[]>>(`${this.apiUrl}/company/${companyId.trim()}`);
  }

  getRoleById(id: string): Observable<ApiResponse<RoleResponse>> {
    return this.http.get<ApiResponse<RoleResponse>>(`${this.apiUrl}/${id.trim()}`);
  }

  searchRoles(searchTerm: string, companyId?: string): Observable<ApiResponse<RoleResponse[]>> {
    let params = new HttpParams().set('searchTerm', searchTerm.trim());
    if (companyId?.trim()) {
      params = params.set('companyId', companyId.trim());
    }
    return this.http.get<ApiResponse<RoleResponse[]>>(`${this.apiUrl}/search`, { params });
  }

  createRole(payload: CreateRoleRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.apiUrl, payload);
  }

  updateRole(id: string, payload: UpdateRoleRequest): Observable<ApiResponse<RoleResponse>> {
    return this.http.put<ApiResponse<RoleResponse>>(`${this.apiUrl}/${id.trim()}`, payload);
  }

  deleteRole(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id.trim()}`);
  }
}
