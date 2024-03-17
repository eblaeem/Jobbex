import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { MessageService } from '../common/services/message-service';

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.html'
})
export class JobDetail implements OnInit {
  id: number;
  response: any = {};
  jobSkill = [];
  companyAttachments = [];

  jobRequestForm = new FormGroup({
    displayName: new FormControl(''),
    phoneNumber: new FormControl('')
  });

  constructor(private route: ActivatedRoute,
    private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private titleService: Title,
    private messageService: MessageService) {

  }
  ngOnInit() {
    this.route.params.subscribe(params => {
      this.id = params["id"];
      this.get();
    });
  }
  get() {
    this.http.get(`${this.appConfig.apiEndpoint}/job/detail?id=${this.id}`).subscribe((response: any) => {
      this.titleService.setTitle(response.jobTitle);
      this.response = response;
      this.jobSkill = response.jobSkill;
      this.companyAttachments = response.companyAttachments;
    });
  }
  jobRequest(id) {
    const file = (document.getElementById('resume-file-upload') as any).files[0];
    const formData = new FormData();
    formData.append("file", file);
    formData.append("jobId", id);
    formData.append("displayName", this.jobRequestForm.value.displayName);
    formData.append("phoneNumber", this.jobRequestForm.value.phoneNumber);

    this.http.post(`${this.appConfig.apiEndpoint}/jobRequest/save`, formData).subscribe((response: any) => {
      if (response.isValid) {
        this.messageService.success(response.message);
      }
      else {
        this.messageService.error(response.message);
      }
    });
  }
  showImage(img) {
    var image = new Image();
    image.src = img;
    var w = window.open("");
    w.document.write(image.outerHTML);
  }
}
