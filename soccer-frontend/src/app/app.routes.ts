import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'signup',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'teams',
    loadChildren: () => import('./features/teams/teams.module').then(m => m.TeamsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'players',
    loadChildren: () => import('./features/players/players.module').then(m => m.PlayersModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'tournaments',
    loadChildren: () => import('./features/tournaments/tournaments.module').then(m => m.TournamentsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'matches',
    loadChildren: () => import('./features/matches/matches.module').then(m => m.MatchesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'groups',
    loadChildren: () => import('./features/groups/groups.module').then(m => m.GroupsModule),
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
