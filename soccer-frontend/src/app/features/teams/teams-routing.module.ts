import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TeamsListComponent } from './pages/teams-list.component';
import { TeamFormComponent } from './pages/team-form.component';
import { AuthGuard } from '../../core/guards';

const routes: Routes = [
  {
    path: '',
    component: TeamsListComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'create',
    component: TeamFormComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'edit',
    component: TeamFormComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeamsRoutingModule {}
