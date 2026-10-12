import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
} from "@angular/core";
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/auth/auth.interceptor';
import { provideRouter, withComponentInputBinding } from "@angular/router";
import { providePrimeNG } from "primeng/config";
import { MelodiaPreset } from "./core/theme/melodia-preset";

import { routes } from "./app.routes";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    providePrimeNG({
      theme: {
        preset: MelodiaPreset,
        options: {
          darkModeSelector: ".melodia-dark",
        },
      },
    }),
  ],
};
