import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { fades } from './animations';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  animations: [
    fades
  ]
})
export class AppComponent {
  title = 'app'
  constructor() {
  }
  prepareRoute(outlet: RouterOutlet) {
    return outlet && outlet.activatedRouteData;
  }
}
