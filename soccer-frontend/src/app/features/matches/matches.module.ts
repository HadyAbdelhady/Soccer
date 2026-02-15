import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatchesRoutingModule } from './matches-routing.module';
import { MatchesListComponent } from './pages/matches-list.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatchesRoutingModule,
    MatchesListComponent
  ]
})
export class MatchesModule {}
