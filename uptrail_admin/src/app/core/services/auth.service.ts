import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  PlatformAdminLoginRequest,
  PlatformAdminTokenResponse,
  PlatformAdminUser,
  RefreshTokenRequest
} from '../models/auth.models';

const ACCESS_TOKEN_KEY = 'uptrail_access_token';
const REFRESH_TOKEN_KEY = 'uptrail_refresh_token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly authApiUrl = `${environment.apiUrl}/PlatformAdminAuth`;

  readonly accessToken = signal<string | null>(this.getStoredAccessToken());
  readonly refreshToken = signal<string | null>(this.getStoredRefreshToken());
  readonly currentUser = signal<PlatformAdminUser | null>(this.decodeUserFromToken(this.getStoredAccessToken()));

  readonly isAuthenticated = computed(() => {
    const token = this.accessToken();
    return !!token && !this.isTokenExpired(token);
  });

  login(credentials: PlatformAdminLoginRequest): Observable<ApiResponse<PlatformAdminTokenResponse>> {
    return this.http.post<ApiResponse<PlatformAdminTokenResponse>>(`${this.authApiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setTokens(response.data.accessToken, response.data.refreshToken);
        }
      })
    );
  }

  refreshSession(): Observable<ApiResponse<PlatformAdminTokenResponse>> {
    const token = this.getStoredRefreshToken();
    if (!token) {
      this.clearSession();
      return throwError(() => new Error('No refresh token available'));
    }

    const payload: RefreshTokenRequest = { refreshToken: token };
    return this.http.post<ApiResponse<PlatformAdminTokenResponse>>(`${this.authApiUrl}/refresh`, payload).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setTokens(response.data.accessToken, response.data.refreshToken);
        }
      }),
      catchError(err => {
        this.clearSession();
        return throwError(() => err);
      })
    );
  }

  logout(): void {
    const token = this.getStoredRefreshToken();
    if (token) {
      const payload: RefreshTokenRequest = { refreshToken: token };
      this.http.post(`${this.authApiUrl}/logout`, payload).pipe(
        catchError(() => of(null))
      ).subscribe();
    }

    this.clearSession();
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return this.accessToken();
  }

  getRefreshToken(): string | null {
    return this.refreshToken();
  }

  private setTokens(access: string, refresh: string): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, access);
    localStorage.setItem(REFRESH_TOKEN_KEY, refresh);
    this.accessToken.set(access);
    this.refreshToken.set(refresh);
    this.currentUser.set(this.decodeUserFromToken(access));
  }

  private clearSession(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    this.accessToken.set(null);
    this.refreshToken.set(null);
    this.currentUser.set(null);
  }

  private getStoredAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  private getStoredRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  private isTokenExpired(token: string): boolean {
    const payload = this.parseJwt(token);
    if (!payload || !payload.exp) {
      return true;
    }
    const expiryTime = payload.exp * 1000;
    return Date.now() >= expiryTime;
  }

  private decodeUserFromToken(token: string | null): PlatformAdminUser | null {
    if (!token) return null;
    const payload = this.parseJwt(token);
    if (!payload) return null;

    const id = payload.sub || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || '';
    const name = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.name || 'Admin';
    const email = payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '';
    const isActive = payload.is_active === 'true' || payload.is_active === true;
    const isEmailVerified = payload.email_verified === 'true' || payload.email_verified === true;

    return {
      id,
      name,
      email,
      isActive,
      isEmailVerified
    };
  }

  private parseJwt(token: string): any {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      const base64Url = parts[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch {
      return null;
    }
  }
}
