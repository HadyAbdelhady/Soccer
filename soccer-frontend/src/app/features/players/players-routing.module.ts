import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlayersListComponent } from './pages/players-list.component';
import { PlayerFormComponent } from './pages/player-form.component';
import { AuthGuard } from '../../core/guards';

const routes: Routes = [
  {
    path: '',
    component: PlayersListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'create',
    component: PlayerFormComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'edit',
    component: PlayerFormComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PlayersRoutingModule {}
