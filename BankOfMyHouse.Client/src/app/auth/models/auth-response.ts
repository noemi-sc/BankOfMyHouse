import { UserDto } from "./user";

export interface UserLoginRequestDto {
  username: string;
  password: string;
}

export interface UserLoginResponseDto {
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
