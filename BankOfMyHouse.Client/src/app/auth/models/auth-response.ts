import { UserDto } from "./user";

export interface UserLoginRequest {
  username: string;
  password: string;
}

export interface UserLoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string; // Use string for ISO date, or Date if you parse it
  user: UserDto;
}

export interface RegisterUserRequestDto {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  firstName?: string;
  lastName?: string;
}

export interface RegisterUserResponseDto {
  user: UserDto;
  accessToken: string;
  refreshToken: string;
}
