export interface UserDto {
  username: string;
  email: string;
  createdAt: string; // ISO date string, or Date if you prefer
  lastLoginAt?: string; // ISO date string, or Date, and optional
  isActive: boolean;
  roles: string[];
}
