import { Routes } from '@angular/router';

import { authGuard, guestGuard } from './core/auth/auth.guards';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Giriş · Melodia',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login-page/login-page').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    title: 'Kayıt ol · Melodia',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/register-page/register-page').then((m) => m.RegisterPage),
  },
{
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/app-shell/app-shell').then((m) => m.AppShell),
    children: [
      {
        path: '',
        title: 'Melodia',
        loadComponent: () =>
          import('./features/home/home-page/home-page').then((m) => m.HomePage),
      },
            {
        path: 'packages',
        title: 'Paketler · Melodia',
        loadComponent: () =>
          import('./features/packages/packages-page/packages-page').then((m) => m.PackagesPage),
      },
            {
        path: 'discover',
        title: 'Keşfet · Melodia',
        loadComponent: () =>
          import('./features/discover/discover-page/discover-page').then((m) => m.DiscoverPage),
      },
            {
        path: 'history',
        title: 'Geçmiş · Melodia',
        loadComponent: () =>
          import('./features/history/history-page/history-page').then((m) => m.HistoryPage),
      },
            {
        path: 'playlists',
        title: 'Listelerim · Melodia',
        loadComponent: () =>
          import('./features/playlists/playlists-page/playlists-page').then((m) => m.PlaylistsPage),
      },
      {
        path: 'playlists/:id',
        title: 'Liste · Melodia',
        loadComponent: () =>
          import('./features/playlists/playlist-detail-page/playlist-detail-page').then(
            (m) => m.PlaylistDetailPage,
          ),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
