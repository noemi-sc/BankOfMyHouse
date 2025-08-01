import { Component } from '@angular/core';
import {
  RouterOutlet,
  RouterLink,
  Router,
  NavigationEnd,
} from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink
],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  currentRoute: string = '';
  title = 'BankOfMyHouse.Client';
  showFiller: boolean = false;

  constructor(private router: Router) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.currentRoute = event.url;
      }
    });
  }
}
