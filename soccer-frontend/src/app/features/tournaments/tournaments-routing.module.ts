import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TournamentsListComponent } from './pages/tournaments-list.component';
import { TournamentFormComponent } from './pages/tournament-form.component';
import { AddTeamsToTournamentComponent } from './pages/add-teams-to-tournament.component';
import { AuthGuard } from '../../core/guards';

const routes: Routes = [
  {
    path: '',
    component: TournamentsListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'create',
    component: TournamentFormComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'edit',
    component: TournamentFormComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'add-teams',
    component: AddTeamsToTournamentComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TournamentsRoutingModule {}
