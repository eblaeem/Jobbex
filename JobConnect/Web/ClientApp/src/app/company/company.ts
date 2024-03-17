import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Select2Data } from 'ng-select2-component';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';

@Component({
  selector: 'app-company',
  templateUrl: './company.html'
})
export class Company implements OnInit {
  id: number;
  response: any = {};
  pageLengths: Select2Data = [];
  pageLengthId = 5;

  types: Select2Data = [];
  typeId = 1;

  activeJobs = [];
  expiredJobs = [];

  constructor(private route: ActivatedRoute,
    private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private titleService: Title) {

  }
  ngOnInit() {
    this.route.params.subscribe(params => {
      this.id = params["id"];
    });
    this.get();
  }
  get() {
    this.http.get(`${this.appConfig.apiEndpoint}/company/detail?id=${this.id}`).subscribe((response: any) => {
      this.titleService.setTitle(response.name);
      this.response = response;
      this.activeJobs = response.activeJobs;
      this.expiredJobs = response.expiredJobs;
    });
  }
}
