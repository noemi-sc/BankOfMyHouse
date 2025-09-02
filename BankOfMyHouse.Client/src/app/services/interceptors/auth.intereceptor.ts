import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { AUTH_CONSTANTS } from '../../shared/constants/auth.constants';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const platformId = inject(PLATFORM_ID);

  let token: string | null = null;
  if (isPlatformBrowser(platformId)) {
    token = localStorage.getItem(AUTH_CONSTANTS.TOKEN_KEY); // Match your token key
  }

  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(authReq);
  }

  return next(req);
};