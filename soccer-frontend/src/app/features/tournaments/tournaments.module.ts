import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TournamentsRoutingModule } from './tournaments-routing.module';
import { TournamentsListComponent } from './pages/tournaments-list.component';
import { TournamentFormComponent } from './pages/tournament-form.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    TournamentsRoutingModule,
    TournamentsListComponent,
    TournamentFormComponent
  ]
})
export class TournamentsModule {}
