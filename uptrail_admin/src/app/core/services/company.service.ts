import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { CompanyResponse, CreateCompanyRequest, UpdateCompanyRequest } from '../models/company.models';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Company`;

  getCompanies(): Observable<ApiResponse<CompanyResponse[]>> {
    return this.http.get<ApiResponse<CompanyResponse[]>>(this.apiUrl);
  }

  getCompanyById(id: string): Observable<ApiResponse<CompanyResponse>> {
    return this.http.get<ApiResponse<CompanyResponse>>(`${this.apiUrl}/${id.trim()}`);
  }

  searchCompanies(searchTerm: string): Observable<ApiResponse<CompanyResponse[]>> {
    const params = new HttpParams().set('searchTerm', searchTerm.trim());
    return this.http.get<ApiResponse<CompanyResponse[]>>(`${this.apiUrl}/search`, { params });
  }

  createCompany(payload: CreateCompanyRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.apiUrl, payload);
  }

  updateCompany(id: string, payload: UpdateCompanyRequest): Observable<ApiResponse<CompanyResponse>> {
    return this.http.put<ApiResponse<CompanyResponse>>(`${this.apiUrl}/${id.trim()}`, payload);
  }

  deleteCompany(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id.trim()}`);
  }
}
