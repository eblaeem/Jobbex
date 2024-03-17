import { Injectable } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { filter, map, mergeMap, Observable } from "rxjs";

import { BrowserStorageService } from "./browser-storage.service";

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  constructor(private browserStorageService: BrowserStorageService,
    private activatedRoute: ActivatedRoute,
    private router: Router) { }

  isEmptyString(value: string): boolean {
    return !value || 0 === value.length;
  }

  getCurrentTabId(): number {
    const tabIdToken = "currentTabId";
    let tabId = this.browserStorageService.getSession(tabIdToken);
    if (tabId) {
      return tabId;
    }
    tabId = Math.random();
    this.browserStorageService.setSession(tabIdToken, tabId);
    return tabId;
  }
  generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const random = Math.random() * 16 | 0;
      const result = c === 'x' ? random : (random & 0x3 | 0x8);
      return result.toString(16);
    });
  }
  getShowNavFooter() {
    //return this.router.events.pipe(filter(event => event instanceof NavigationEnd), map(() => this.activatedRoute),
    //  map(route => {
    //    while (route.firstChild) {
    //      route = route.firstChild;
    //    }
    //    return route;
    //  }),)
    //  .pipe(
    //    filter(route => route.outlet === 'primary'),
    //    mergeMap(route => route.data),
    //  ).subscribe((data: any) => {
    //    if (data == false) {
    //      //this.visible = false;
    //      return false;
    //    }
    //    //this.visible = true;
    //    return true;
    //  });
  }

}
