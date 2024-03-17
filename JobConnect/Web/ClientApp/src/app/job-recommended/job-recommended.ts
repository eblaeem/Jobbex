import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Select2Data } from 'ng-select2-component';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';

@Component({
  selector: 'app-job-recommended',
  templateUrl: './job-recommended.html'
})
export class JobRecommended {
  public labels: any = {
    previousLabel: 'قبلی',
    nextLabel: 'بعدی'
  };
  page: number = 1;
  itemsPerPage: number = 10;
  total: number;
  pageLengths: Select2Data = [];
  pageLengthId = 10;

  sortingTypes: Select2Data = [];
  sortingTypeId = 1;

  response = [];
  popularTags = [];

  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.http.get(`${this.appConfig.apiEndpoint}/common/setting`).subscribe((response: any) => {
      this.pageLengths = response.pageLengths;
      this.sortingTypes = response.sortingTypes;
    });
  }
  ngOnInit(): void {
    this.get(1);
    this.options();
  }
  options() {
    this.http.get(`${this.appConfig.apiEndpoint}/softWareSkill/getPopularTags`).subscribe((response: any) => {
      this.popularTags = response;
    });
  }
  get(i) {
    this.http.get(`${this.appConfig.apiEndpoint}/userRecommended/get?pageNumber=${i}&pageSize=${this.itemsPerPage}`).subscribe((response: any) => {
      if (!response) {
        this.response = [];
        this.total = 0;
        return;
      }
      this.total = response[0].totalRowCount;
      this.response = response;

      this.page = i;
    });
  }
  onPageChange(number: number) {
    this.page = number;
    this.get(number);
  }
}
