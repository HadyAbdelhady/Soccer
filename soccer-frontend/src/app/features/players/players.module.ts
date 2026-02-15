import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayersRoutingModule } from './players-routing.module';
import { PlayersListComponent } from './pages/players-list.component';
import { PlayerFormComponent } from './pages/player-form.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    PlayersRoutingModule,
    PlayersListComponent,
    PlayerFormComponent
  ]
})
export class PlayersModule {}
