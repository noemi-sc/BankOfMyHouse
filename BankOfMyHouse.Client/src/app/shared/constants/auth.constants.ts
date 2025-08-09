export const AUTH_CONSTANTS = {
  TOKEN_KEY: 'authToken',
  REFRESH_TOKEN_KEY: 'refreshToken',
  EXPIRES_AT_KEY: 'expiresAt',
  USER_KEY: 'currentUser', // Optional: if you want to store user data
} as const;

// Alternative: individual exports
export const TOKEN_KEY = 'authToken';
export const REFRESH_TOKEN_KEY = 'refreshToken';
export const EXPIRES_AT_KEY = 'expiresAt';