export type PackageName = 'Basic' | 'Gold' | 'Premium' | 'Elit';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}

export interface JwtPayload {
  sub: string;
  email: string;
  package: PackageName;
  role?: string | string[];
  exp: number;
}
