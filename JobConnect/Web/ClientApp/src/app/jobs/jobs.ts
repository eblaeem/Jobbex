import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Select2Data } from 'ng-select2-component';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { MessageService } from '../common/services/message-service';

@Component({
  selector: 'app-jobs',
  templateUrl: './jobs.html',
})
export class Jobs {
  labels: any = {
    previousLabel: 'قبلی',
    nextLabel: 'بعدی',
  };
  page: number = 1;
  itemsPerPage: number = 10;
  total: number;

  pageLengths: Select2Data = [];
  pageLengthId = 1;
  sortingTypes: Select2Data = [];
  sortingTypeId = 1;

  response = {};
  groups = [];
  latest = [];
  states = [];
  contractTypes = [];
  workExperienceYears = [];
  popularTags = [];
  salaryRequestedTypes = [];
  jobGroupId = 0;
  cityId = 0;
  term = '';

  selectedJobGroups = [];
  selectedContractTypes = [];
  selectedJobCities = [];
  selectedJobSalaryRequesteds = [];
  selectedJobWorkExperienceYears = [];

  constructor(
    private http: HttpClient,
    private messageService: MessageService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig
  ) {
    this.http
      .get(`${this.appConfig.apiEndpoint}/common/setting`)
      .subscribe((response: any) => {
        this.pageLengths = response.pageLengths;
        this.sortingTypes = response.sortingTypes;
      });
  }
  ngOnInit() {
    this.options(1);
  }
  options(i) {
    this.http
      .get(`${this.appConfig.apiEndpoint}/job/options`)
      .subscribe((response: any) => {
        this.groups = response.groups;
        this.latest = response.latest;
        this.states = response.states;
        this.popularTags = response.popularTags;
        this.contractTypes = response.contractTypes;
        this.workExperienceYears = response.workExperienceYears;
        this.salaryRequestedTypes = response.salaryRequesteds;
        if (response.latest) {
          this.total = response.latest[0].totalRowCount;
          this.page = i;
        }
      });
  }
  get() {
    const item = {
      pageNumber: this.page,
      pageSize: 10,
      term: this.term,
      jobGroups: this.selectedJobGroups,
      contractTypes: this.selectedContractTypes,
      cities: this.selectedJobCities,
      salaryRequests: this.selectedJobSalaryRequesteds,
      workExperienceYears: this.selectedJobWorkExperienceYears,
    };
// console.log(item);

    this.http
      .post(`${this.appConfig.apiEndpoint}/job/get`, item)
      .subscribe((response: any) => {
        if (!response) {
          this.latest = [];
          this.total = 0;
          return;
        }
        this.total = response[0].totalRowCount;
        this.latest = response;
      });
  }
  onPageChange(number: number) {
    this.page = number;
    this.get();
    ('');
  }
  setOption(type: number, data: any) {
    if (data.value) {
      switch (type) {
        case 1:
          this.pageLengthId = data.value;
          break;
        case 2:
          this.sortingTypeId = data.value;
          break;
      }
      this.get();
    }
  }
  changeItems(event, items) {
    const value = parseInt(event.target.value);
    if (event.target.checked) {
      if (items.indexOf(value) < 0) {
        items.push(value);
        console.log(value);

      }
    } else {
      if (items.indexOf(value) > -1) {
        items.splice(this.selectedJobGroups.indexOf(value), 1);
      }
    }
    this.get();
  }

  pin(id: number) {
    const item = {
      Id: id,
    };
    this.http
      .post(`${this.appConfig.apiEndpoint}/userPinJobs/Save`, item)
      .subscribe((response: any) => {
        if (response.isValid) {
          return this.messageService.success(response.message);
        } else {
          return this.messageService.error(response.message);
        }
      });
  }
}
