import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HomeComponent } from "./home/home.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HomeComponent, MatSidenavModule, MatButtonModule, MatGridListModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'BankOfMyHouse.Client';
  showFiller: boolean = false;
}


import {MatButtonModule} from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';

export class SidenavAutosizeExample {
  showFiller = false;
}
import {MatGridListModule} from '@angular/material/grid-list';

/**
 * @title Basic grid-list
 */
export class GridListOverviewExample { 
}


