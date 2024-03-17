import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.html'
})
export class Footer implements OnInit {
  visible = true;
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router) {
    this.visible = true;
  }

  ngOnInit() {

    this.router.events.pipe(filter(event => event instanceof NavigationEnd), map(() => this.activatedRoute),
      map(route => {
        while (route.firstChild) {
          route = route.firstChild;
        }
        return route;
      }),)
      .pipe(
        filter(route => route.outlet === 'primary'),
        mergeMap(route => route.data),
      ).subscribe((data: any) => {
        if (data.visible == false) {
          this.visible = false;
          return;
        }
        this.visible = true;
        return;
      });
  }
}
