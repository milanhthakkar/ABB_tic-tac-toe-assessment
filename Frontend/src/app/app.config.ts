import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    // 👈 Changed eventCoalescing to false to make UI clicks register instantly
    provideZoneChangeDetection({ eventCoalescing: false }), 
    provideHttpClient() 
  ]
};
