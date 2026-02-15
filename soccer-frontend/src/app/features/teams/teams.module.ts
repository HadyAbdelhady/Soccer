import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamsRoutingModule } from './teams-routing.module';
import { TeamsListComponent } from './pages/teams-list.component';
import { TeamFormComponent } from './pages/team-form.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    TeamsRoutingModule,
    TeamsListComponent,
    TeamFormComponent
  ]
})
export class TeamsModule {}
