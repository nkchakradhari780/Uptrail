export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface PlatformAdminLoginRequest {
  email: string;
  password: string;
}

export interface PlatformAdminTokenResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface PlatformAdminUser {
  id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  profileImageUrl?: string;
  isActive?: boolean;
  isEmailVerified?: boolean;
}

export interface PlatformAdminResponse {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  profileImageUrl: string;
  isEmailVerified: boolean;
  isActive: boolean;
  lastLoginAt?: string;
  createdAt: string;
  updatedAt?: string;
}
