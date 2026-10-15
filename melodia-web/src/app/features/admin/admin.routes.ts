import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-layout/admin-layout').then((m) => m.AdminLayout),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'songs' },
      {
        path: 'songs',
        title: 'Admin · Şarkılar',
        loadComponent: () =>
          import('./admin-songs-page/admin-songs-page').then((m) => m.AdminSongsPage),
      },
            {
        path: 'artists',
        title: 'Admin · Sanatçılar',
        loadComponent: () =>
          import('./admin-artists-page/admin-artists-page').then((m) => m.AdminArtistsPage),
      },
      {
        path: 'albums',
        title: 'Admin · Albümler',
        loadComponent: () =>
          import('./admin-albums-page/admin-albums-page').then((m) => m.AdminAlbumsPage),
      },
      {
        path: 'genres',
        title: 'Admin · Türler',
        loadComponent: () =>
          import('./admin-genres-page/admin-genres-page').then((m) => m.AdminGenresPage),
      },
    ],
  },
];
